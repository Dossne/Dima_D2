using UnityEngine;

namespace DimaD2
{
    public class TopDownCameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 14f, 0f);
        [SerializeField] private float smoothTime = 0.12f;

        private Vector3 _velocity;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 desiredPosition = target.position + offset;
            Vector3 nextPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, smoothTime);

            transform.position = nextPosition;
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}
