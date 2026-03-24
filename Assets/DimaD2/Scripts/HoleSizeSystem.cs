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
        private Vector3 initialLocalScale;
        private float initialColliderRadius;
        private bool initialized;

        private void Awake()
        {
            holeController = GetComponent<HoleController>();
            sphereCollider = GetComponent<SphereCollider>();
            CacheInitialState();
        }

        private void Start()
        {
            ResetSystem();
        }

        public void AddProgress(float amount)
        {
            if (amount <= 0f)
            {
                return;
            }

            Progress += amount;
            Debug.Log($"[HoleSizeSystem] Progress +{amount:0.##} => {Progress:0.##}/{GetThreshold(SizeLevel):0.##}", gameObject);

            float threshold = GetThreshold(SizeLevel);
            while (Progress >= threshold)
            {
                Progress -= threshold;
                SizeLevel++;
                ApplySize();
                threshold = GetThreshold(SizeLevel);
            }
        }

        private float GetThreshold(int level)
        {
            return baseThreshold * Mathf.Pow(2f, level);
        }

        private void ApplySize()
        {
            CacheInitialState();

            float sizeValue = startSize + (SizeLevel * sizePerLevel);
            holeController?.SetSize(sizeValue);

            float growthMultiplier = Mathf.Sqrt(Mathf.Max(0.01f, sizeValue / Mathf.Max(0.01f, startSize)));
            transform.localScale = new Vector3(
                initialLocalScale.x * growthMultiplier,
                initialLocalScale.y,
                initialLocalScale.z * growthMultiplier);

            if (sphereCollider != null)
            {
                sphereCollider.radius = initialColliderRadius * growthMultiplier;
            }

            Debug.Log($"[HoleSizeSystem] Level={SizeLevel}, holeSize={sizeValue:0.##}, progress={Progress:0.##}/{GetThreshold(SizeLevel):0.##}", gameObject);
        }

        public void ResetSystem()
        {
            CacheInitialState();
            SizeLevel = 0;
            Progress = 0f;
            ApplySize();
        }

        private void CacheInitialState()
        {
            if (initialized)
            {
                return;
            }

            initialLocalScale = transform.localScale;
            initialColliderRadius = sphereCollider != null ? sphereCollider.radius : 0.5f;
            initialized = true;
        }
    }
}
