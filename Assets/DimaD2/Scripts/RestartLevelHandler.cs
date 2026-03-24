using UnityEngine;
using UnityEngine.SceneManagement;

namespace DimaD2
{
    public class RestartLevelHandler : MonoBehaviour
    {
        private bool restarting;

        public void OnRetryButtonClicked()
        {
            Debug.Log("[RestartLevelHandler] Retry button clicked.", gameObject);
            RestartCurrentScene();
        }

        public void OnExitToMainMenuClicked()
        {
            Debug.Log("[RestartLevelHandler] Exit to main menu clicked.", gameObject);
            ExitToMainMenuState();
        }

        public void RestartCurrentScene()
        {
            if (restarting)
            {
                Debug.Log("[RestartLevelHandler] Restart already in progress, ignoring duplicate request.", gameObject);
                return;
            }

            Debug.Log("[RestartLevelHandler] RestartCurrentScene called.", gameObject);
            restarting = true;

            Scene activeScene = SceneManager.GetActiveScene();
            Debug.Log($"[RestartLevelHandler] Scene reload requested for '{activeScene.name}'.", gameObject);
            SceneManager.LoadScene(activeScene.name);
        }

        public void ExitToMainMenuState()
        {
            if (restarting)
            {
                Debug.Log("[RestartLevelHandler] Exit request ignored because a reload is already in progress.", gameObject);
                return;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            Debug.Log($"[RestartLevelHandler] Returning to pre-start state by reloading '{activeScene.name}'.", gameObject);
            restarting = true;
            SceneManager.LoadScene(activeScene.name);
        }
    }
}
