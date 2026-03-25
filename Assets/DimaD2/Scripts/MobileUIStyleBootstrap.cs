using UnityEngine;
using UnityEngine.UI;

namespace DimaD2
{
    public class MobileUIStyleBootstrap : MonoBehaviour
    {
        private Sprite panelSprite;
        private Sprite primaryButtonSprite;
        private Sprite secondaryButtonSprite;
        private Sprite dangerButtonSprite;
        private Sprite circleButtonSprite;

        private void Awake()
        {
            LoadSprites();
            ApplyTheme();
        }

        private void LoadSprites()
        {
            panelSprite = CreateSpriteFromResource("UIGenerated/PanelRounded");
            primaryButtonSprite = CreateSpriteFromResource("UIGenerated/ButtonPrimary");
            secondaryButtonSprite = CreateSpriteFromResource("UIGenerated/ButtonSecondary");
            dangerButtonSprite = CreateSpriteFromResource("UIGenerated/ButtonDanger");
            circleButtonSprite = CreateSpriteFromResource("UIGenerated/ButtonCircle");
        }

        private void ApplyTheme()
        {
            StylePanel("StartPanel", panelSprite, new Color(0.12f, 0.36f, 0.2f, 1f));
            StylePanel("PausePanel", panelSprite, new Color(0.53f, 0.45f, 0.94f, 1f));
            StylePanel("VictoryPanel", panelSprite, new Color(0.12f, 0.36f, 0.2f, 1f));
            StylePanel("LosePanel", panelSprite, new Color(0.56f, 0.18f, 0.18f, 1f));
            StylePanel("HUDBackdrop", panelSprite, new Color(0.12f, 0.74f, 0.22f, 0.92f));

            StyleButton("StartButton", primaryButtonSprite, Color.white, 460f, 128f, 48, Color.white);
            StyleButton("ResumeButton", primaryButtonSprite, Color.white, 420f, 104f, 42, Color.white);
            StyleButton("PauseRestartButton", secondaryButtonSprite, Color.white, 420f, 104f, 40, new Color(0.27f, 0.18f, 0.02f, 1f));
            StyleButton("ExitToMainMenuButton", dangerButtonSprite, Color.white, 420f, 104f, 30, Color.white);
            StyleButton("VictoryRetryButton", primaryButtonSprite, Color.white, 340f, 104f, 42, Color.white);
            StyleButton("LoseRetryButton", primaryButtonSprite, Color.white, 340f, 104f, 42, Color.white);
            StyleButton("PauseButton", circleButtonSprite, Color.white, 118f, 118f, 44, Color.white);

            StyleText("StartLevelText", 68, Color.white, true);
            StyleText("StartObjectiveText", 38, new Color(1f, 0.98f, 0.92f, 1f), true);
            StyleText("PauseTitleText", 58, Color.white, true);
            StyleText("VictoryText", 68, Color.white, true);
            StyleText("LoseText", 68, Color.white, true);
            StyleText("TimerText", 42, new Color(1f, 0.98f, 0.92f, 1f), true);
            StyleText("ObjectiveText", 36, Color.white, true);
            StyleText("PauseButtonText", 44, Color.white, true);
            StyleText("StartButtonText", 48, Color.white, true);
            StyleText("ResumeButtonText", 42, Color.white, true);
            StyleText("PauseRestartText", 40, new Color(0.27f, 0.18f, 0.02f, 1f), true);
            StyleText("ExitToMainMenuText", 30, Color.white, true);
            StyleText("VictoryRetryText", 42, Color.white, true);
            StyleText("LoseRetryText", 42, Color.white, true);
        }

        private void StylePanel(string objectName, Sprite sprite, Color tint)
        {
            Image image = FindImage(objectName);
            if (image == null || sprite == null)
            {
                return;
            }

            image.sprite = sprite;
            image.type = Image.Type.Simple;
            image.color = tint;
        }

        private void StyleButton(string objectName, Sprite sprite, Color tint, float width, float height, int fontSize, Color textColor)
        {
            Image image = FindImage(objectName);
            if (image == null)
            {
                return;
            }

            if (sprite != null)
            {
                image.sprite = sprite;
                image.type = Image.Type.Simple;
            }

            image.color = tint;
            image.preserveAspect = false;
            image.rectTransform.sizeDelta = new Vector2(width, height);

            Text childText = image.GetComponentInChildren<Text>(true);
            if (childText != null)
            {
                childText.fontSize = fontSize;
                childText.color = textColor;
                childText.alignment = TextAnchor.MiddleCenter;
                EnsureOutline(childText, new Color(0f, 0f, 0f, 0.35f), new Vector2(4f, -4f));
                EnsureShadow(childText, new Color(0f, 0f, 0f, 0.22f), new Vector2(1.5f, -1.5f));
            }
        }

        private void StyleText(string objectName, int fontSize, Color color, bool outline)
        {
            foreach (Text candidate in FindObjectsOfType<Text>(true))
            {
                if (candidate.name != objectName)
                {
                    continue;
                }

                candidate.fontSize = fontSize;
                candidate.color = color;

                if (outline)
                {
                    EnsureOutline(candidate, new Color(0f, 0f, 0f, 0.45f), new Vector2(4f, -4f));
                    EnsureShadow(candidate, new Color(0f, 0f, 0f, 0.22f), new Vector2(1.5f, -1.5f));
                }

                break;
            }
        }

        private static void EnsureOutline(Graphic graphic, Color color, Vector2 distance)
        {
            Outline outline = graphic.GetComponent<Outline>();
            if (outline == null)
            {
                outline = graphic.gameObject.AddComponent<Outline>();
            }

            outline.effectColor = color;
            outline.effectDistance = distance;
        }

        private static void EnsureShadow(Graphic graphic, Color color, Vector2 distance)
        {
            Shadow shadow = graphic.GetComponent<Shadow>();
            if (shadow == null)
            {
                shadow = graphic.gameObject.AddComponent<Shadow>();
            }

            shadow.effectColor = color;
            shadow.effectDistance = distance;
        }

        private Image FindImage(string objectName)
        {
            foreach (Image image in FindObjectsOfType<Image>(true))
            {
                if (image.name == objectName)
                {
                    return image;
                }
            }

            return null;
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
