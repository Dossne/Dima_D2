using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem.UI;
#else
using UnityEngine.EventSystems;
#endif

namespace DimaD2
{
    public class UIEventSystemBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            EventSystem eventSystem = GetComponent<EventSystem>();
            if (eventSystem == null)
            {
                eventSystem = gameObject.AddComponent<EventSystem>();
            }

            Component legacyHelper = GetComponent("MMAutoInputModule");
            if (legacyHelper != null)
            {
                Destroy(legacyHelper);
            }

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            foreach (BaseInputModule module in GetComponents<BaseInputModule>())
            {
                if (module is InputSystemUIInputModule)
                {
                    continue;
                }

                Destroy(module);
            }

            InputSystemUIInputModule inputModule = GetComponent<InputSystemUIInputModule>();
            if (inputModule == null)
            {
                inputModule = gameObject.AddComponent<InputSystemUIInputModule>();
                inputModule.AssignDefaultActions();
            }
#else
            foreach (BaseInputModule module in GetComponents<BaseInputModule>())
            {
                if (module is StandaloneInputModule)
                {
                    continue;
                }

                Destroy(module);
            }

            if (GetComponent<StandaloneInputModule>() == null)
            {
                gameObject.AddComponent<StandaloneInputModule>();
            }
#endif
        }
    }
}
