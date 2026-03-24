using System;
using UnityEngine;
using UnityEngine.UI;

namespace DimaD2
{
    public class StartPanelUI : MonoBehaviour
    {
        [SerializeField] private GameObject startPanel;
        [SerializeField] private Text levelLabelText;
        [SerializeField] private Text objectiveSummaryText;
        [SerializeField] private HoleController holeController;
        [SerializeField] private TimerSystem timerSystem;
        [SerializeField] private LevelConfig levelConfig;

        public event Action OnSessionStarted;

        public bool SessionStarted => sessionStarted;

        private bool sessionStarted;

        private void Awake()
        {
            if (holeController == null)
            {
                holeController = FindObjectOfType<HoleController>();
            }

            if (timerSystem == null)
            {
                timerSystem = FindObjectOfType<TimerSystem>();
            }

            if (levelConfig == null)
            {
                levelConfig = FindObjectOfType<LevelConfig>();
            }

            RefreshTexts();

            if (startPanel != null)
            {
                startPanel.SetActive(true);
            }
        }

        private void Start()
        {
            holeController?.SetGameplayEnabled(false);
            timerSystem?.ResetTimer(false);
        }

        public void OnStartButtonClicked()
        {
            if (sessionStarted)
            {
                Debug.Log("[StartPanelUI] Start already requested, ignoring duplicate click.", gameObject);
                return;
            }

            Debug.Log("[StartPanelUI] Start button clicked.", gameObject);
            sessionStarted = true;

            if (startPanel != null)
            {
                startPanel.SetActive(false);
            }

            holeController?.SetGameplayEnabled(true);
            timerSystem?.StartCountdown();
            OnSessionStarted?.Invoke();
            Debug.Log("[StartPanelUI] Gameplay enabled and timer started.", gameObject);
        }

        private void RefreshTexts()
        {
            if (levelLabelText != null)
            {
                levelLabelText.text = levelConfig != null ? levelConfig.LevelLabel : "Level 1";
            }

            if (objectiveSummaryText != null)
            {
                objectiveSummaryText.text = levelConfig != null
                    ? levelConfig.GetStartObjectiveSummary()
                    : "Press Start to begin.";
            }
        }
    }
}
