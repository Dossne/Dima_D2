using UnityEngine;

namespace DimaD2
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject pauseButton;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private HoleController holeController;
        [SerializeField] private TimerSystem timerSystem;
        [SerializeField] private GameStateSystem gameStateSystem;
        [SerializeField] private StartPanelUI startPanelUI;
        [SerializeField] private RestartLevelHandler restartLevelHandler;

        private bool paused;

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

            if (gameStateSystem == null)
            {
                gameStateSystem = FindObjectOfType<GameStateSystem>();
            }

            if (startPanelUI == null)
            {
                startPanelUI = FindObjectOfType<StartPanelUI>();
            }

            if (restartLevelHandler == null)
            {
                restartLevelHandler = FindObjectOfType<RestartLevelHandler>();
            }

            SetPausePanelVisible(false);
            SetPauseButtonVisible(startPanelUI != null && startPanelUI.SessionStarted && !HasEnded());
        }

        private void OnEnable()
        {
            if (startPanelUI != null)
            {
                startPanelUI.OnSessionStarted += HandleSessionStarted;
            }

            if (gameStateSystem != null)
            {
                gameStateSystem.OnWin += HandleSessionEnded;
                gameStateSystem.OnLose += HandleSessionEnded;
            }
        }

        private void OnDisable()
        {
            if (startPanelUI != null)
            {
                startPanelUI.OnSessionStarted -= HandleSessionStarted;
            }

            if (gameStateSystem != null)
            {
                gameStateSystem.OnWin -= HandleSessionEnded;
                gameStateSystem.OnLose -= HandleSessionEnded;
            }
        }

        public void OnPauseButtonClicked()
        {
            if (paused || HasEnded() || startPanelUI == null || !startPanelUI.SessionStarted)
            {
                return;
            }

            paused = true;
            holeController?.SetGameplayEnabled(false);
            timerSystem?.PauseCountdown();
            SetPausePanelVisible(true);
            SetPauseButtonVisible(false);
            Debug.Log("[PauseMenuUI] Gameplay paused.", gameObject);
        }

        public void OnResumeButtonClicked()
        {
            if (!paused)
            {
                return;
            }

            paused = false;
            SetPausePanelVisible(false);

            if (!HasEnded() && startPanelUI != null && startPanelUI.SessionStarted)
            {
                holeController?.SetGameplayEnabled(true);
                timerSystem?.ResumeCountdown();
                SetPauseButtonVisible(true);
            }

            Debug.Log("[PauseMenuUI] Gameplay resumed.", gameObject);
        }

        public void OnExitToMainMenuClicked()
        {
            if (restartLevelHandler == null)
            {
                return;
            }

            Debug.Log("[PauseMenuUI] Exit to main menu requested.", gameObject);
            restartLevelHandler.OnExitToMainMenuClicked();
        }

        private void HandleSessionStarted()
        {
            paused = false;
            SetPausePanelVisible(false);
            SetPauseButtonVisible(!HasEnded());
        }

        private void HandleSessionEnded()
        {
            paused = false;
            SetPausePanelVisible(false);
            SetPauseButtonVisible(false);
        }

        private bool HasEnded()
        {
            return gameStateSystem != null && gameStateSystem.IsSessionEnded;
        }

        private void SetPauseButtonVisible(bool isVisible)
        {
            if (pauseButton != null)
            {
                pauseButton.SetActive(isVisible);
            }
        }

        private void SetPausePanelVisible(bool isVisible)
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(isVisible);
            }
        }
    }
}
