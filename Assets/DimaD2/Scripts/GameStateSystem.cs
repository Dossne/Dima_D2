using System;
using UnityEngine;

namespace DimaD2
{
    public class GameStateSystem : MonoBehaviour
    {
        [SerializeField] private ObjectiveSystem objectiveSystem;
        [SerializeField] private HoleController holeController;

        public event Action OnWin;
        public event Action OnLose;

        public bool HasWon => hasWon;
        public bool HasLost => hasLost;
        public bool IsSessionEnded => hasWon || hasLost;

        private bool hasWon;
        private bool hasLost;

        private void Awake()
        {
            if (objectiveSystem == null)
            {
                objectiveSystem = FindObjectOfType<ObjectiveSystem>();
            }

            if (holeController == null)
            {
                holeController = FindObjectOfType<HoleController>();
            }
        }

        private void OnEnable()
        {
            if (objectiveSystem != null)
            {
                objectiveSystem.OnAllObjectivesComplete += HandleObjectivesComplete;
            }
        }

        private void OnDisable()
        {
            if (objectiveSystem != null)
            {
                objectiveSystem.OnAllObjectivesComplete -= HandleObjectivesComplete;
            }
        }

        public bool TryLose()
        {
            if (IsSessionEnded)
            {
                return false;
            }

            hasLost = true;
            holeController?.SetGameplayEnabled(false);
            Debug.Log("[GameStateSystem] Lose state reached. Movement and absorption are now disabled.", gameObject);
            OnLose?.Invoke();
            return true;
        }

        private void HandleObjectivesComplete()
        {
            if (IsSessionEnded)
            {
                return;
            }

            hasWon = true;
            holeController?.SetGameplayEnabled(false);
            Debug.Log("[GameStateSystem] Win state reached. Movement and absorption are now disabled.", gameObject);
            OnWin?.Invoke();
        }
    }
}
