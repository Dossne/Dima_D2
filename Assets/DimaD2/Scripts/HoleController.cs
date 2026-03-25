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
        [SerializeField] private AudioSource absorbAudioSource;
        [SerializeField] private AudioClip absorbClip;
        [SerializeField] [Range(0f, 1f)] private float absorbVolume = 0.85f;
        [SerializeField] private string absorbClipResourcePath = "Audio/AbsorbSuction";

        [Header("Bounds")]
        [SerializeField] private bool clampToPlayArea = true;
        [SerializeField] private Vector2 playAreaExtents = new Vector2(13.5f, 13.5f);

        private HoleSizeSystem holeSizeSystem;
        private ObjectiveSystem objectiveSystem;
        private TouchJoystickUI touchJoystickUI;
        private bool gameplayEnabled = true;
        private bool joystickMovementLogged;

        private void Awake()
        {
            holeSizeSystem = GetComponent<HoleSizeSystem>();
            absorbAudioSource = absorbAudioSource != null ? absorbAudioSource : GetComponent<AudioSource>();

            if (absorbAudioSource == null)
            {
                absorbAudioSource = gameObject.AddComponent<AudioSource>();
            }

            absorbAudioSource.playOnAwake = false;
            absorbAudioSource.loop = false;
            absorbAudioSource.spatialBlend = 0f;
            absorbAudioSource.volume = absorbVolume;

            if (absorbClip == null && !string.IsNullOrWhiteSpace(absorbClipResourcePath))
            {
                absorbClip = Resources.Load<AudioClip>(absorbClipResourcePath);
            }
        }

        private void Start()
        {
            objectiveSystem = FindObjectOfType<ObjectiveSystem>();
            touchJoystickUI = FindObjectOfType<TouchJoystickUI>();
            gameplayEnabled = true;
        }

        public void SetSize(float newSize)
        {
            currentSize = newSize;
        }

        public void SetGameplayEnabled(bool isEnabled)
        {
            gameplayEnabled = isEnabled;
            joystickMovementLogged = false;

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
                PlayAbsorbSound();
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
                joystickMovementLogged = false;
                return keyboardInput;
            }

            if (touchJoystickUI == null)
            {
                touchJoystickUI = FindObjectOfType<TouchJoystickUI>();
            }

            if (touchJoystickUI != null)
            {
                Vector2 joystickInput = Vector2.ClampMagnitude(touchJoystickUI.CurrentInput, 1f);

                if (joystickInput.sqrMagnitude > 0.0001f)
                {
                    if (!joystickMovementLogged)
                    {
                        Debug.Log($"[HoleController] Reading joystick movement vector {joystickInput}.", gameObject);
                        joystickMovementLogged = true;
                    }
                }
                else
                {
                    joystickMovementLogged = false;
                }

                return joystickInput;
            }

            joystickMovementLogged = false;
            return Vector2.zero;
        }

        private void PlayAbsorbSound()
        {
            if (absorbClip == null && !string.IsNullOrWhiteSpace(absorbClipResourcePath))
            {
                absorbClip = Resources.Load<AudioClip>(absorbClipResourcePath);
            }

            if (absorbAudioSource == null || absorbClip == null)
            {
                return;
            }

            absorbAudioSource.volume = absorbVolume;
            absorbAudioSource.PlayOneShot(absorbClip, absorbVolume);
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
