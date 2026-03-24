using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DimaD2
{
    [DefaultExecutionOrder(-100)]
    public class LevelConfig : MonoBehaviour
    {
        [Serializable]
        private class LevelObjectiveConfig
        {
            public string itemType = "Collectible";
            public int requiredCount = 1;
        }

        [Header("Presentation")]
        [SerializeField] private string levelTitle = "Prototype Level 01";
        [SerializeField] private string levelLabel = "Level 1";

        [Header("Scene References")]
        [SerializeField] private TimerSystem timerSystem;
        [SerializeField] private ObjectiveSystem objectiveSystem;
        [SerializeField] private HoleSizeSystem holeSizeSystem;

        [Header("Timer")]
        [SerializeField] private float timerDuration = 30f;

        [Header("Objectives")]
        [SerializeField] private List<LevelObjectiveConfig> objectives = new List<LevelObjectiveConfig>
        {
            new LevelObjectiveConfig()
        };

        [Header("Hole Growth")]
        [SerializeField] private float holeStartSize = 1f;
        [SerializeField] private float holeBaseThreshold = 1.5f;
        [SerializeField] private float holeSizePerLevel = 0.5f;

        public string LevelTitle => levelTitle;
        public string LevelLabel => string.IsNullOrWhiteSpace(levelLabel) ? levelTitle : levelLabel;

        private void Awake()
        {
            if (timerSystem == null)
            {
                timerSystem = FindObjectOfType<TimerSystem>();
            }

            if (objectiveSystem == null)
            {
                objectiveSystem = FindObjectOfType<ObjectiveSystem>();
            }

            if (holeSizeSystem == null)
            {
                holeSizeSystem = FindObjectOfType<HoleSizeSystem>();
            }

            timerSystem?.Configure(timerDuration);
            holeSizeSystem?.Configure(holeBaseThreshold, holeStartSize, holeSizePerLevel);
            objectiveSystem?.Configure(BuildObjectiveEntries());
        }

        public string GetStartObjectiveSummary()
        {
            if (objectives.Count == 0)
            {
                return "Press Start to begin.";
            }

            if (objectives.Count == 1)
            {
                LevelObjectiveConfig objective = objectives[0];
                string itemLabel = GetDisplayLabel(objective.itemType, objective.requiredCount);
                return $"Collect {objective.requiredCount} {itemLabel} before time runs out.";
            }

            StringBuilder builder = new StringBuilder();
            builder.Append("Complete objectives before time runs out:\n");

            for (int i = 0; i < objectives.Count; i++)
            {
                LevelObjectiveConfig objective = objectives[i];
                builder.Append("- ");
                builder.Append(objective.requiredCount);
                builder.Append(" ");
                builder.Append(GetDisplayLabel(objective.itemType, objective.requiredCount));

                if (i < objectives.Count - 1)
                {
                    builder.Append("\n");
                }
            }

            return builder.ToString();
        }

        private IEnumerable<ObjectiveEntry> BuildObjectiveEntries()
        {
            List<ObjectiveEntry> configuredObjectives = new List<ObjectiveEntry>();

            foreach (LevelObjectiveConfig objective in objectives)
            {
                if (objective == null || string.IsNullOrWhiteSpace(objective.itemType))
                {
                    continue;
                }

                configuredObjectives.Add(new ObjectiveEntry
                {
                    itemType = objective.itemType,
                    requiredCount = Mathf.Max(0, objective.requiredCount),
                    currentCount = 0
                });
            }

            return configuredObjectives;
        }

        private static string GetDisplayLabel(string itemType, int count)
        {
            string trimmed = string.IsNullOrWhiteSpace(itemType) ? "item" : itemType.Trim();

            if (count == 1)
            {
                return trimmed;
            }

            return trimmed.EndsWith("s", StringComparison.OrdinalIgnoreCase) ? trimmed : trimmed + "s";
        }
    }
}
