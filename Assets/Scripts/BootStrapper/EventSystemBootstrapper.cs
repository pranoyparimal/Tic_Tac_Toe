using UnityEngine;
using UnityEngine.EventSystems;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

/// <summary>
/// Guarantees exactly one EventSystem exists in the scene, without needing
/// to be manually attached to a GameObject or called from another script.
///
/// UI elements (Buttons, GraphicRaycaster, EventTrigger, etc.) cannot
/// receive pointer/click events without an EventSystem present somewhere
/// in the scene. This is easy to miss when canvases/UI are created entirely
/// at runtime, since nothing in the scene guarantees one already exists.
///
/// [RuntimeInitializeOnLoadMethod] runs this automatically right before the
/// first scene loads, so no setup is required beyond having this script
/// anywhere in the project (it doesn't need to be on a GameObject).
/// </summary>
public static class EventSystemBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureEventSystemExists()
    {
        if (Object.FindAnyObjectByType<EventSystem>() != null)
            return; // one already exists (e.g. placed manually in the scene) - don't create a duplicate

        GameObject eventSystemGO = new("EventSystem");

        eventSystemGO.AddComponent<EventSystem>();

#if ENABLE_INPUT_SYSTEM
        // Project is using the newer Input System package.
        eventSystemGO.AddComponent<InputSystemUIInputModule>();
#else
        // Project is using the legacy Input Manager (default for most projects).
        eventSystemGO.AddComponent<StandaloneInputModule>();
#endif

        // Persist across scene loads so it's never re-created or duplicated
        // if you have multiple scenes (e.g. a menu scene + gameplay scene).
        Object.DontDestroyOnLoad(eventSystemGO);
    }
}
