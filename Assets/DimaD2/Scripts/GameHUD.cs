using UnityEngine;
using UnityEngine.UI;

namespace DimaD2
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField] private TimerSystem timerSystem;
        [SerializeField] private ObjectiveSystem objectiveSystem;
        [SerializeField] private GameStateSystem gameStateSystem;
        [SerializeField] private Text timerText;
        [SerializeField] private Text objectiveText;
        [SerializeField] private GameObject losePanel;

        private bool loseShown;

        private void Awake()
        {
            if (timerSystem == null)
            {
                timerSystem = FindObjectOfType<TimerSystem>();
            }

            if (objectiveSystem == null)
            {
                objectiveSystem = FindObjectOfType<ObjectiveSystem>();
            }

            if (gameStateSystem == null)
            {
                gameStateSystem = FindObjectOfType<GameStateSystem>();
            }

            HideLosePanel();
            RefreshTexts();
        }

        private void OnEnable()
        {
            if (gameStateSystem != null)
            {
                gameStateSystem.OnLose += HandleLose;

                if (gameStateSystem.HasLost)
                {
                    ShowLosePanel();
                }
            }
        }

        private void OnDisable()
        {
            if (gameStateSystem != null)
            {
                gameStateSystem.OnLose -= HandleLose;
            }
        }

        private void Update()
        {
            RefreshTexts();
        }

        private void HandleLose()
        {
            ShowLosePanel();
        }

        private void RefreshTexts()
        {
            if (timerText != null && timerSystem != null)
            {
                timerText.text = $"Time: {Mathf.CeilToInt(timerSystem.RemainingTime)}";
            }

            if (objectiveText != null && objectiveSystem != null)
            {
                objectiveText.text = objectiveSystem.GetProgressSummary();
            }
        }

        private void HideLosePanel()
        {
            loseShown = false;

            if (losePanel != null)
            {
                losePanel.SetActive(false);
            }
        }

        private void ShowLosePanel()
        {
            if (loseShown)
            {
                return;
            }

            loseShown = true;

            if (losePanel != null)
            {
                losePanel.SetActive(true);
            }
        }
    }
}
