using System.Collections;
using UnityEngine;

namespace DimaD2
{
    public class HoleSizeSystem : MonoBehaviour
    {
        [SerializeField] private float baseThreshold = 5f;
        [SerializeField] private float startSize = 1f;
        [SerializeField] private float sizePerLevel = 0.5f;

        [Header("Feedback")]
        [SerializeField] private float absorbPulseMultiplier = 1.08f;
        [SerializeField] private float blockedPulseMultiplier = 1.04f;
        [SerializeField] private float levelUpPulseMultiplier = 1.16f;
        [SerializeField] private float pulseInDuration = 0.06f;
        [SerializeField] private float pulseOutDuration = 0.1f;

        public int SizeLevel { get; private set; }
        public float Progress { get; private set; }

        private HoleController holeController;
        private SphereCollider sphereCollider;
        private Vector3 initialLocalScale;
        private float initialColliderRadius;
        private Vector3 baseLocalScale;
        private float activePulseMultiplier = 1f;
        private bool initialized;
        private Coroutine scalePulseCoroutine;

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

        private void OnDisable()
        {
            StopFeedback();
        }

        public void Configure(float threshold, float initialSize, float growthPerLevel)
        {
            baseThreshold = Mathf.Max(0.01f, threshold);
            startSize = Mathf.Max(0.01f, initialSize);
            sizePerLevel = Mathf.Max(0f, growthPerLevel);
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
                Debug.Log($"[HoleSizeSystem] LEVEL UP! Reached size level {SizeLevel}.", gameObject);
                PlayScalePulse(levelUpPulseMultiplier);
                threshold = GetThreshold(SizeLevel);
            }
        }

        public void PlayAbsorbFeedback()
        {
            PlayScalePulse(absorbPulseMultiplier);
        }

        public void PlayBlockedFeedback()
        {
            PlayScalePulse(blockedPulseMultiplier);
        }

        public void StopFeedback()
        {
            if (scalePulseCoroutine != null)
            {
                StopCoroutine(scalePulseCoroutine);
                scalePulseCoroutine = null;
            }

            activePulseMultiplier = 1f;

            if (initialized)
            {
                ApplyVisualScale(activePulseMultiplier);
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
            baseLocalScale = new Vector3(
                initialLocalScale.x * growthMultiplier,
                initialLocalScale.y,
                initialLocalScale.z * growthMultiplier);

            ApplyVisualScale(activePulseMultiplier);

            if (sphereCollider != null)
            {
                sphereCollider.radius = initialColliderRadius * growthMultiplier;
            }

            Debug.Log($"[HoleSizeSystem] Level={SizeLevel}, holeSize={sizeValue:0.##}, progress={Progress:0.##}/{GetThreshold(SizeLevel):0.##}", gameObject);
        }

        public void ResetSystem()
        {
            CacheInitialState();
            StopFeedback();
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
            baseLocalScale = initialLocalScale;
            initialized = true;
        }

        private void ApplyVisualScale(float multiplier)
        {
            transform.localScale = new Vector3(
                baseLocalScale.x * multiplier,
                baseLocalScale.y,
                baseLocalScale.z * multiplier);
        }

        private void PlayScalePulse(float peakMultiplier)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            if (scalePulseCoroutine != null)
            {
                StopCoroutine(scalePulseCoroutine);
            }

            scalePulseCoroutine = StartCoroutine(ScalePulseRoutine(Mathf.Max(1f, peakMultiplier)));
        }

        private IEnumerator ScalePulseRoutine(float peakMultiplier)
        {
            float elapsed = 0f;
            float startMultiplier = activePulseMultiplier;

            while (elapsed < pulseInDuration)
            {
                elapsed += Time.deltaTime;
                float t = pulseInDuration <= 0f ? 1f : Mathf.Clamp01(elapsed / pulseInDuration);
                activePulseMultiplier = Mathf.Lerp(startMultiplier, peakMultiplier, t);
                ApplyVisualScale(activePulseMultiplier);
                yield return null;
            }

            elapsed = 0f;

            while (elapsed < pulseOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = pulseOutDuration <= 0f ? 1f : Mathf.Clamp01(elapsed / pulseOutDuration);
                activePulseMultiplier = Mathf.Lerp(peakMultiplier, 1f, t);
                ApplyVisualScale(activePulseMultiplier);
                yield return null;
            }

            activePulseMultiplier = 1f;
            ApplyVisualScale(activePulseMultiplier);
            scalePulseCoroutine = null;
        }
    }
}
