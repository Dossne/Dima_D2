using UnityEngine;

namespace DimaD2
{
    public class VictoryUI : MonoBehaviour
    {
        [SerializeField] private GameStateSystem gameStateSystem;
        [SerializeField] private GameObject victoryPanel;

        private bool shown;

        private void Awake()
        {
            if (gameStateSystem == null)
            {
                gameStateSystem = FindObjectOfType<GameStateSystem>();
            }

            if (victoryPanel == null)
            {
                Transform victoryPanelTransform = transform.Find("VictoryPanel");
                victoryPanel = victoryPanelTransform != null ? victoryPanelTransform.gameObject : null;
            }

            HideVictory();
        }

        private void OnEnable()
        {
            if (gameStateSystem != null)
            {
                gameStateSystem.OnWin += HandleWin;

                if (gameStateSystem.HasWon)
                {
                    ShowVictory();
                }
            }
        }

        private void OnDisable()
        {
            if (gameStateSystem != null)
            {
                gameStateSystem.OnWin -= HandleWin;
            }
        }

        private void HandleWin()
        {
            ShowVictory();
        }

        private void HideVictory()
        {
            shown = false;

            if (victoryPanel != null)
            {
                victoryPanel.SetActive(false);
            }
        }

        private void ShowVictory()
        {
            if (shown)
            {
                return;
            }

            shown = true;

            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
                Debug.Log("[VictoryUI] Victory panel shown.", victoryPanel);
            }
            else
            {
                Debug.LogWarning("[VictoryUI] Win triggered but no VictoryPanel reference is available.", gameObject);
            }
        }
    }
}
