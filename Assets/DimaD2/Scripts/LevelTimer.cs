using System;
using UnityEngine;

namespace DimaD2
{
    public class LevelTimer : MonoBehaviour
    {
        [SerializeField] private float duration = 60f;

        public event Action OnTimeUp;

        public float TimeRemaining { get; private set; }
        public bool IsRunning { get; private set; }

        private void Start()
        {
            TimeRemaining = duration;
            IsRunning = true;
        }

        private void Update()
        {
            if (!IsRunning) return;

            TimeRemaining -= Time.deltaTime;
            if (TimeRemaining <= 0f)
            {
                TimeRemaining = 0f;
                IsRunning = false;
                OnTimeUp?.Invoke();
            }
        }

        public void Stop() => IsRunning = false;

        public void Restart()
        {
            TimeRemaining = duration;
            IsRunning = true;
        }
    }
}
