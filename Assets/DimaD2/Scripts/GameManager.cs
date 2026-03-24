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
            Time.timeScale = 0f;
            Debug.Log("[GameManager] Time's up! FAIL.");
        }

        private void HandleObjectivesComplete()
        {
            if (State != GameState.Playing) return;
            State = GameState.Win;
            levelTimer?.Stop();
            Time.timeScale = 0f;
            Debug.Log("[GameManager] All objectives complete! WIN.");
        }

        private void OnGUI()
        {
            if (State == GameState.Playing) return;

            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 72,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            style.normal.textColor = State == GameState.Win ? Color.green : Color.red;

            GUI.Label(new Rect(0, 0, Screen.width, Screen.height),
                State == GameState.Win ? "WIN" : "FAIL", style);
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
