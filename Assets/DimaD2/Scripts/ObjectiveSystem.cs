using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DimaD2
{
    [Serializable]
    public class ObjectiveEntry
    {
        public string itemType;
        public int requiredCount;
        [HideInInspector] public int currentCount;
    }

    public class ObjectiveSystem : MonoBehaviour
    {
        [SerializeField] private List<ObjectiveEntry> objectives = new List<ObjectiveEntry>();

        public event Action OnAllObjectivesComplete;

        public bool IsCompleted => completed;

        private bool completed;

        private void Start()
        {
            ResetSystem();
        }

        public void RegisterAbsorb(string itemType)
        {
            if (completed || string.IsNullOrEmpty(itemType))
            {
                return;
            }

            foreach (ObjectiveEntry entry in objectives)
            {
                if (entry.itemType == itemType && entry.currentCount < entry.requiredCount)
                {
                    entry.currentCount++;
                    Debug.Log($"[ObjectiveSystem] {itemType}: {entry.currentCount}/{entry.requiredCount}", gameObject);
                    CheckCompletion();
                    return;
                }
            }
        }

        public string GetProgressSummary()
        {
            if (objectives.Count == 0)
            {
                return "Objectives: -";
            }

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < objectives.Count; i++)
            {
                ObjectiveEntry entry = objectives[i];

                if (i > 0)
                {
                    builder.Append("\n");
                }

                builder.Append(entry.itemType);
                builder.Append(": ");
                builder.Append(entry.currentCount);
                builder.Append("/");
                builder.Append(entry.requiredCount);
            }

            return builder.ToString();
        }

        private void CheckCompletion()
        {
            foreach (ObjectiveEntry entry in objectives)
            {
                if (entry.currentCount < entry.requiredCount)
                {
                    return;
                }
            }

            completed = true;
            Debug.Log("[ObjectiveSystem] All objectives complete!", gameObject);
            OnAllObjectivesComplete?.Invoke();
        }

        public void ResetSystem()
        {
            completed = false;

            foreach (ObjectiveEntry entry in objectives)
            {
                entry.currentCount = 0;
            }
        }
    }
}
