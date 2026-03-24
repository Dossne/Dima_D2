using UnityEngine;

namespace DimaD2
{
    public class HoleController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float touchSensitivity = 12f;

        [Header("Bounds")]
        [SerializeField] private bool clampToPlayArea = true;
        [SerializeField] private Vector2 playAreaExtents = new Vector2(13.5f, 13.5f);

        private void Update()
        {
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

        private Vector2 ReadMovementInput()
        {
            Vector2 axisInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (axisInput.sqrMagnitude > 1f)
            {
                axisInput.Normalize();
            }

            if (axisInput.sqrMagnitude > 0.0001f)
            {
                return axisInput;
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
