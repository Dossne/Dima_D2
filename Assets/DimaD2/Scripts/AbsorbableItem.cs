using UnityEngine;

namespace DimaD2
{
    public class AbsorbableItem : MonoBehaviour
    {
        [SerializeField] private float itemSize = 1f;
        [SerializeField] private string itemType = "";
        [SerializeField] private bool deactivateOnAbsorb = true;

        public float ItemSize => itemSize;
        public string ItemType => itemType;

        public bool CanBeAbsorbedBy(float holeSize)
        {
            return itemSize <= holeSize;
        }

        public void Absorb()
        {
            if (deactivateOnAbsorb)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
