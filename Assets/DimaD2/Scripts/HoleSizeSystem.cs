using UnityEngine;

namespace DimaD2
{
    public class HoleSizeSystem : MonoBehaviour
    {
        [SerializeField] private float baseThreshold = 5f;
        [SerializeField] private float startSize = 1f;
        [SerializeField] private float sizePerLevel = 0.5f;

        public int SizeLevel { get; private set; }
        public float Progress { get; private set; }

        private HoleController holeController;
        private SphereCollider sphereCollider;

        private void Awake()
        {
            holeController = GetComponent<HoleController>();
            sphereCollider = GetComponent<SphereCollider>();
        }

        public void AddProgress(float amount)
        {
            Progress += amount;

            float threshold = GetThreshold(SizeLevel);
            while (Progress >= threshold)
            {
                Progress -= threshold;
                SizeLevel++;
                threshold = GetThreshold(SizeLevel);
                ApplySize();
            }
        }

        // Threshold doubles each level: baseThreshold * 2^level
        private float GetThreshold(int level) => baseThreshold * Mathf.Pow(2f, level);

        private void ApplySize()
        {
            float newSize = startSize + SizeLevel * sizePerLevel;
            holeController.SetSize(newSize);

            if (sphereCollider != null)
                sphereCollider.radius = newSize;

            transform.localScale = Vector3.one * newSize;

            Debug.Log($"[HoleSizeSystem] Level up → SizeLevel={SizeLevel}, holeSize={newSize:0.##}");
        }

        public void ResetSystem()
        {
            SizeLevel = 0;
            Progress = 0f;
            ApplySize();
        }
    }
}
