using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DimaD2
{
    public class TouchJoystickUI : MonoBehaviour
    {
        [SerializeField] private float joystickRadius = 120f;
        [SerializeField] [Range(0f, 0.5f)] private float deadZone = 0.12f;
        [SerializeField] private float inputResponsiveness = 14f;
        [SerializeField] private string baseTexturePath = "UIGenerated/JoystickBase";
        [SerializeField] private string knobTexturePath = "UIGenerated/JoystickKnob";

        public Vector2 CurrentInput => currentInput;
        public bool IsActive => activePointerId != NoPointerId;

        private const int MousePointerId = -100;
        private const int NoPointerId = int.MinValue;

        private RectTransform canvasRect;
        private RectTransform joystickRoot;
        private RectTransform joystickBase;
        private RectTransform joystickKnob;
        private Canvas canvas;
        private StartPanelUI startPanelUI;
        private GameStateSystem gameStateSystem;
        private int activePointerId = NoPointerId;
        private Vector2 joystickStartPosition;
        private Vector2 currentInput;
        private Vector2 targetInput;
        private Vector2 lastLoggedInput;

        private void Awake()
        {
            canvasRect = transform as RectTransform;
            canvas = GetComponent<Canvas>();
            startPanelUI = FindObjectOfType<StartPanelUI>();
            gameStateSystem = FindObjectOfType<GameStateSystem>();
            BuildJoystickUi();
            Debug.Log($"[TouchJoystickUI] Joystick created. Root valid={joystickRoot != null}, Base valid={joystickBase != null}, Knob valid={joystickKnob != null}", gameObject);
            SetJoystickVisible(false);
        }

        private void Update()
        {
            if (!CanUseGameplayTouch())
            {
                ResetJoystick();
                return;
            }

            if (activePointerId == MousePointerId)
            {
                UpdateMousePointer();
                return;
            }

            if (activePointerId != NoPointerId)
            {
                UpdateTouchPointer();
                return;
            }

            if (TryBeginMousePointer())
            {
                return;
            }

            TryBeginTouchPointer();
        }

        private bool TryBeginMousePointer()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return false;
            }

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return false;
            }

            BeginPointer(MousePointerId, Input.mousePosition, "mouse");
            return true;
        }

        private void TryBeginTouchPointer()
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase != TouchPhase.Began)
                {
                    continue;
                }

                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    continue;
                }

                BeginPointer(touch.fingerId, touch.position, $"touch:{touch.fingerId}");
                return;
            }
        }

        private void BeginPointer(int pointerId, Vector2 screenPosition, string source)
        {
            activePointerId = pointerId;
            joystickStartPosition = ScreenToAnchoredPosition(screenPosition);
            currentInput = Vector2.zero;
            targetInput = Vector2.zero;
            lastLoggedInput = Vector2.zero;

            if (joystickRoot != null)
            {
                joystickRoot.anchoredPosition = joystickStartPosition;
            }

            if (joystickKnob != null)
            {
                joystickKnob.anchoredPosition = Vector2.zero;
            }

            SetJoystickVisible(true);
            Debug.Log($"[TouchJoystickUI] Joystick start from {source} at {screenPosition}.", gameObject);
        }

        private void UpdateMousePointer()
        {
            if (!Input.GetMouseButton(0))
            {
                Debug.Log("[TouchJoystickUI] Joystick release from mouse.", gameObject);
                ResetJoystick();
                return;
            }

            UpdatePointerPosition(Input.mousePosition);
        }

        private void UpdateTouchPointer()
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.fingerId != activePointerId)
                {
                    continue;
                }

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    Debug.Log($"[TouchJoystickUI] Joystick release from touch:{touch.fingerId}.", gameObject);
                    ResetJoystick();
                    return;
                }

                UpdatePointerPosition(touch.position);
                return;
            }

            ResetJoystick();
        }

        private void UpdatePointerPosition(Vector2 screenPosition)
        {
            Vector2 localPosition = ScreenToAnchoredPosition(screenPosition);
            Vector2 delta = localPosition - joystickStartPosition;
            Vector2 clampedDelta = Vector2.ClampMagnitude(delta, joystickRadius);
            Vector2 rawInput = joystickRadius > 0f ? clampedDelta / joystickRadius : Vector2.zero;
            float rawMagnitude = Mathf.Clamp01(rawInput.magnitude);

            if (rawMagnitude <= deadZone)
            {
                targetInput = Vector2.zero;
            }
            else
            {
                float adjustedMagnitude = Mathf.InverseLerp(deadZone, 1f, rawMagnitude);
                targetInput = rawInput.normalized * adjustedMagnitude;
            }

            currentInput = Vector2.MoveTowards(currentInput, targetInput, inputResponsiveness * Time.unscaledDeltaTime);

            if (joystickKnob != null)
            {
                joystickKnob.anchoredPosition = currentInput * joystickRadius;
            }

            if ((currentInput - lastLoggedInput).sqrMagnitude > 0.04f)
            {
                lastLoggedInput = currentInput;
                Debug.Log($"[TouchJoystickUI] Joystick drag vector {currentInput}.", gameObject);
            }
        }

        private bool CanUseGameplayTouch()
        {
            if (startPanelUI == null)
            {
                startPanelUI = FindObjectOfType<StartPanelUI>();
            }

            if (gameStateSystem == null)
            {
                gameStateSystem = FindObjectOfType<GameStateSystem>();
            }

            if (startPanelUI == null || !startPanelUI.SessionStarted)
            {
                return false;
            }

            if (gameStateSystem != null && gameStateSystem.IsSessionEnded)
            {
                return false;
            }

            return Time.timeScale > 0.01f;
        }

        private void BuildJoystickUi()
        {
            GameObject root = new GameObject("TouchJoystickRoot", typeof(RectTransform), typeof(CanvasGroup));
            root.transform.SetParent(transform, false);
            joystickRoot = root.GetComponent<RectTransform>();
            joystickRoot.anchorMin = new Vector2(0f, 0f);
            joystickRoot.anchorMax = new Vector2(0f, 0f);
            joystickRoot.pivot = new Vector2(0.5f, 0.5f);
            joystickRoot.sizeDelta = new Vector2(joystickRadius * 2.2f, joystickRadius * 2.2f);

            CanvasGroup rootCanvasGroup = root.GetComponent<CanvasGroup>();
            rootCanvasGroup.blocksRaycasts = false;
            rootCanvasGroup.interactable = false;

            GameObject baseObject = new GameObject("TouchJoystickBase", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            baseObject.transform.SetParent(joystickRoot, false);
            joystickBase = baseObject.GetComponent<RectTransform>();
            joystickBase.anchorMin = new Vector2(0.5f, 0.5f);
            joystickBase.anchorMax = new Vector2(0.5f, 0.5f);
            joystickBase.pivot = new Vector2(0.5f, 0.5f);
            joystickBase.sizeDelta = new Vector2(joystickRadius * 2f, joystickRadius * 2f);
            Image baseImage = baseObject.GetComponent<Image>();
            baseImage.raycastTarget = false;
            baseImage.sprite = CreateSpriteFromResource(baseTexturePath);
            baseImage.color = Color.white;

            GameObject knobObject = new GameObject("TouchJoystickKnob", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            knobObject.transform.SetParent(joystickBase, false);
            joystickKnob = knobObject.GetComponent<RectTransform>();
            joystickKnob.anchorMin = new Vector2(0.5f, 0.5f);
            joystickKnob.anchorMax = new Vector2(0.5f, 0.5f);
            joystickKnob.pivot = new Vector2(0.5f, 0.5f);
            joystickKnob.sizeDelta = new Vector2(joystickRadius, joystickRadius);
            Image knobImage = knobObject.GetComponent<Image>();
            knobImage.raycastTarget = false;
            knobImage.sprite = CreateSpriteFromResource(knobTexturePath);
            knobImage.color = Color.white;
        }

        private Vector2 ScreenToAnchoredPosition(Vector2 screenPosition)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, canvas != null ? canvas.worldCamera : null, out Vector2 localPoint);
            return localPoint;
        }

        private void ResetJoystick()
        {
            activePointerId = NoPointerId;
            currentInput = Vector2.zero;
            targetInput = Vector2.zero;
            lastLoggedInput = Vector2.zero;

            if (joystickKnob != null)
            {
                joystickKnob.anchoredPosition = Vector2.zero;
            }

            SetJoystickVisible(false);
        }

        private void SetJoystickVisible(bool visible)
        {
            if (joystickRoot != null)
            {
                joystickRoot.gameObject.SetActive(visible);
            }
        }

        private static Sprite CreateSpriteFromResource(string resourcePath)
        {
            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            if (texture == null)
            {
                return null;
            }

            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
