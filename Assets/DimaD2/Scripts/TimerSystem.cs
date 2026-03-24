using UnityEngine;

namespace DimaD2
{
    public class TimerSystem : MonoBehaviour
    {
        [SerializeField] private float startDuration = 30f;
        [SerializeField] private GameStateSystem gameStateSystem;

        public float RemainingTime => remainingTime;
        public bool IsRunning => isRunning;

        private float remainingTime;
        private bool isRunning;

        private void Awake()
        {
            if (gameStateSystem == null)
            {
                gameStateSystem = FindObjectOfType<GameStateSystem>();
            }
        }

        private void Start()
        {
            remainingTime = Mathf.Max(0f, startDuration);
            isRunning = true;
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
                return;
            }

            remainingTime -= Time.deltaTime;

            if (remainingTime > 0f)
            {
                return;
            }

            remainingTime = 0f;
            isRunning = false;
            Debug.Log("[TimerSystem] Time is up!", gameObject);
            gameStateSystem?.TryLose();
        }
    }
}
