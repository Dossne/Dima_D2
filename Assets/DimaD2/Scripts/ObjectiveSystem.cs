using System;
using System.Collections.Generic;
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

        private bool completed;

        public void RegisterAbsorb(string itemType)
        {
            if (completed || string.IsNullOrEmpty(itemType)) return;

            foreach (ObjectiveEntry entry in objectives)
            {
                if (entry.itemType == itemType && entry.currentCount < entry.requiredCount)
                {
                    entry.currentCount++;
                    Debug.Log($"[ObjectiveSystem] {itemType}: {entry.currentCount}/{entry.requiredCount}");
                    CheckCompletion();
                    return;
                }
            }
        }

        private void CheckCompletion()
        {
            foreach (ObjectiveEntry entry in objectives)
            {
                if (entry.currentCount < entry.requiredCount) return;
            }
            completed = true;
            Debug.Log("[ObjectiveSystem] All objectives complete!");
            OnAllObjectivesComplete?.Invoke();
        }

        public void ResetSystem()
        {
            completed = false;
            foreach (ObjectiveEntry entry in objectives)
                entry.currentCount = 0;
        }
    }
}
