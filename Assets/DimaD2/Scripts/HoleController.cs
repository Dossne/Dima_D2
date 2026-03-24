using UnityEngine;

namespace DimaD2
{
    public class HoleController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float touchSensitivity = 12f;

        [Header("Absorption")]
        [SerializeField] private float currentSize = 1f;

        [Header("Bounds")]
        [SerializeField] private bool clampToPlayArea = true;
        [SerializeField] private Vector2 playAreaExtents = new Vector2(13.5f, 13.5f);

        private HoleSizeSystem holeSizeSystem;
        private ObjectiveSystem objectiveSystem;
        private bool gameplayEnabled = true;

        private void Awake()
        {
            holeSizeSystem = GetComponent<HoleSizeSystem>();
        }

        private void Start()
        {
            objectiveSystem = FindObjectOfType<ObjectiveSystem>();
            gameplayEnabled = true;
        }

        public void SetSize(float newSize)
        {
            currentSize = newSize;
        }

        public void SetGameplayEnabled(bool isEnabled)
        {
            gameplayEnabled = isEnabled;

            if (!gameplayEnabled)
            {
                holeSizeSystem?.StopFeedback();
            }
        }

        private void Update()
        {
            if (!gameplayEnabled)
            {
                return;
            }

            Vector2 input = ReadMovementInput();
            Vector3 movement = new Vector3(input.x, 0f, input.y) * (moveSpeed * Time.deltaTime);

            Vector3 nextPosition = transform.position + movement;

            if (clampToPlayArea)
            {
                nextPosition.x = Mathf.Clamp(nextPosition.x, -playAreaExtents.x, playAreaExtents.x);
                nextPosition.z = Mathf.Clamp(nextPosition.z, -playAreaExtents.y, playAreaExtents.y);
            }

            nextPosition.y = transform.position.y;
            transform.position = nextPosition;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!gameplayEnabled)
            {
                return;
            }

            AbsorbableItem absorbableItem = other.GetComponent<AbsorbableItem>();

            if (absorbableItem == null || !absorbableItem.gameObject.activeInHierarchy)
            {
                return;
            }

            if (!absorbableItem.IsAvailableForAbsorb)
            {
                return;
            }

            if (absorbableItem.CanBeAbsorbedBy(currentSize))
            {
                Debug.Log($"[HoleController] Absorbed '{absorbableItem.gameObject.name}' (itemSize={absorbableItem.ItemSize:0.##}, holeSize={currentSize:0.##})", absorbableItem.gameObject);
                holeSizeSystem?.PlayAbsorbFeedback();
                absorbableItem.Absorb(transform);
                holeSizeSystem?.AddProgress(absorbableItem.ItemSize);
                objectiveSystem?.RegisterAbsorb(absorbableItem.ItemType);
                return;
            }

            holeSizeSystem?.PlayBlockedFeedback();
            Debug.Log($"[HoleController] '{absorbableItem.gameObject.name}' is too large to absorb (itemSize={absorbableItem.ItemSize:0.##}, holeSize={currentSize:0.##})", absorbableItem.gameObject);
        }

        private Vector2 ReadMovementInput()
        {
            Vector2 keyboardInput = ReadKeyboardInput();

            if (keyboardInput.sqrMagnitude > 0f)
            {
                return keyboardInput;
            }

            if (Input.touchCount == 0)
            {
                return Vector2.zero;
            }

            Touch touch = Input.GetTouch(0);

            if (touch.phase != TouchPhase.Moved && touch.phase != TouchPhase.Stationary)
            {
                return Vector2.zero;
            }

            float screenScale = Mathf.Max(1f, Mathf.Min(Screen.width, Screen.height));
            Vector2 normalizedDelta = touch.deltaPosition / screenScale;
            Vector2 touchInput = normalizedDelta * touchSensitivity;

            return Vector2.ClampMagnitude(touchInput, 1f);
        }

        private static Vector2 ReadKeyboardInput()
        {
            float horizontal = 0f;
            float vertical = 0f;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                horizontal -= 1f;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                horizontal += 1f;
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                vertical -= 1f;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                vertical += 1f;
            }

            Vector2 input = new Vector2(horizontal, vertical);
            return input.sqrMagnitude > 1f ? input.normalized : input;
        }

        private void OnDrawGizmosSelected()
        {
            if (!clampToPlayArea)
            {
                return;
            }

            Gizmos.color = new Color(0.15f, 0.6f, 1f, 0.4f);
            Vector3 size = new Vector3(playAreaExtents.x * 2f, 0.1f, playAreaExtents.y * 2f);
            Gizmos.DrawWireCube(new Vector3(0f, transform.position.y, 0f), size);
        }
    }
}
