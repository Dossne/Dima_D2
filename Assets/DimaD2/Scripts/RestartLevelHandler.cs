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
    }
}
