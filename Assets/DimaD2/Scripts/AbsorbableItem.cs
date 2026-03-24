using System.Collections;
using UnityEngine;

namespace DimaD2
{
    public class AbsorbableItem : MonoBehaviour
    {
        [SerializeField] private float itemSize = 1f;
        [SerializeField] private string itemType = "";
        [SerializeField] private bool deactivateOnAbsorb = true;
        [SerializeField] private float absorbSinkDuration = 0.18f;

        public float ItemSize => itemSize;
        public string ItemType => itemType;
        public bool IsAvailableForAbsorb => !isAbsorbing && gameObject.activeInHierarchy;

        private Collider cachedCollider;
        private Vector3 initialScale;
        private bool isAbsorbing;

        private void Awake()
        {
            cachedCollider = GetComponent<Collider>();
            initialScale = transform.localScale;
        }

        public bool CanBeAbsorbedBy(float holeSize)
        {
            return !isAbsorbing && itemSize <= holeSize;
        }

        public void Absorb(Transform sinkTarget)
        {
            if (isAbsorbing)
            {
                return;
            }

            isAbsorbing = true;

            if (cachedCollider != null)
            {
                cachedCollider.enabled = false;
            }

            if (sinkTarget == null)
            {
                CompleteAbsorb();
                return;
            }

            StartCoroutine(AbsorbSinkRoutine(sinkTarget));
        }

        private IEnumerator AbsorbSinkRoutine(Transform sinkTarget)
        {
            Vector3 startPosition = transform.position;
            Vector3 startScale = transform.localScale;
            float duration = Mathf.Max(0.01f, absorbSinkDuration);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                transform.position = Vector3.Lerp(startPosition, sinkTarget.position, t);
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
                yield return null;
            }

            CompleteAbsorb();
        }

        private void CompleteAbsorb()
        {
            transform.localScale = Vector3.zero;

            if (deactivateOnAbsorb)
            {
                gameObject.SetActive(false);
                return;
            }

            Destroy(gameObject);
        }

        private void OnDisable()
        {
            if (transform.localScale == Vector3.zero)
            {
                transform.localScale = initialScale;
            }

            isAbsorbing = false;

            if (cachedCollider != null)
            {
                cachedCollider.enabled = true;
            }
        }
    }
}
