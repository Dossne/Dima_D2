using UnityEngine;
using UnityEngine.SceneManagement;

namespace DimaD2
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private LevelTimer levelTimer;
        [SerializeField] private ObjectiveSystem objectiveSystem;

        public enum GameState { Playing, Win, Fail }
        public GameState State { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            State = GameState.Playing;

            if (levelTimer != null)
                levelTimer.OnTimeUp += HandleTimeUp;

            if (objectiveSystem != null)
                objectiveSystem.OnAllObjectivesComplete += HandleObjectivesComplete;
        }

        private void OnDestroy()
        {
            if (levelTimer != null)
                levelTimer.OnTimeUp -= HandleTimeUp;

            if (objectiveSystem != null)
                objectiveSystem.OnAllObjectivesComplete -= HandleObjectivesComplete;
        }

        private void HandleTimeUp()
        {
            if (State != GameState.Playing) return;
            State = GameState.Fail;
            Debug.Log("[GameManager] Time's up! FAIL.");
        }

        private void HandleObjectivesComplete()
        {
            if (State != GameState.Playing) return;
            State = GameState.Win;
            levelTimer?.Stop();
            Debug.Log("[GameManager] All objectives complete! WIN.");
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
