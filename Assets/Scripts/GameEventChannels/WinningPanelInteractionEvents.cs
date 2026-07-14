using System;
using UnityEngine;

public static class WinningPanelInteractionEvents
{
    public static event Action OnGameRestarted;
    public static void RaiseOnGameRestarted() => OnGameRestarted?.Invoke();

    // Handy for domain-reload safety in the editor (script recompiles, etc.)
    public static void ClearAllListeners()
    {
        OnGameRestarted = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetOnBoot()
    {
        Debug.Log($"User Interaction based Events Listeners Cleared.");
        ClearAllListeners();
    }
}
