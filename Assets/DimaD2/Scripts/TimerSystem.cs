using UnityEngine;

namespace DimaD2
{
    public class TimerSystem : MonoBehaviour
    {
        [SerializeField] private float startDuration = 30f;
        [SerializeField] private bool startOnPlay = true;
        [SerializeField] private GameStateSystem gameStateSystem;

        public float RemainingTime => remainingTime;
        public bool IsRunning => isRunning;
        public bool IsPaused => paused;

        private float remainingTime;
        private bool isRunning;
        private bool paused;

        private void Awake()
        {
            if (gameStateSystem == null)
            {
                gameStateSystem = FindObjectOfType<GameStateSystem>();
            }
        }

        private void Start()
        {
            ResetTimer(startOnPlay);
        }

        public void Configure(float duration)
        {
            startDuration = Mathf.Max(0f, duration);
        }

        public void ResetTimer(bool startRunning)
        {
            remainingTime = Mathf.Max(0f, startDuration);
            isRunning = startRunning;
            paused = false;
        }

        public void StartCountdown()
        {
            if (gameStateSystem != null && gameStateSystem.IsSessionEnded)
            {
                return;
            }

            isRunning = true;
            paused = false;
        }

        public void PauseCountdown()
        {
            if (!isRunning)
            {
                return;
            }

            isRunning = false;
            paused = true;
        }

        public void ResumeCountdown()
        {
            if (!paused || remainingTime <= 0f)
            {
                return;
            }

            if (gameStateSystem != null && gameStateSystem.IsSessionEnded)
            {
                paused = false;
                return;
            }

            isRunning = true;
            paused = false;
        }

        private void Update()
        {
            if (!isRunning)
            {
                return;
            }

            if (gameStateSystem != null && gameStateSystem.IsSessionEnded)
            {
                isRunning = false;
                paused = false;
                return;
            }

            remainingTime -= Time.deltaTime;

            if (remainingTime > 0f)
            {
                return;
            }

            remainingTime = 0f;
            isRunning = false;
            paused = false;
            Debug.Log("[TimerSystem] Time is up!", gameObject);
            gameStateSystem?.TryLose();
        }
    }
}
