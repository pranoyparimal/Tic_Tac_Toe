using System;
using UnityEngine;
using static TurnManager;

public static class TurnManagerEvents
{
    public static event Action<PlayerId, CurrentStatus> OnPlayerAssigned;
    public static void RaiseOnPlayerAssigned (PlayerId _playerId, CurrentStatus _status) => OnPlayerAssigned?.Invoke(_playerId, _status);

    public static event Action<PlayerId, CurrentStatus, bool> OnTurnChanged;
    public static void  RaiseOnTurnChanged(PlayerId _playerId, CurrentStatus _status, bool markerPlaced) => OnTurnChanged?.Invoke(_playerId, _status, markerPlaced);

    public static event Action<int, int, CurrentStatus> OnMarkPlaced;
    public static void RaiseOnMarkPlaced(int _row, int _col,  CurrentStatus _status) => OnMarkPlaced(_row, _col, _status);


    // Handy for domain-reload safety in the editor (script recompiles, etc.)
    public static void ClearAllListeners()
    {
        OnPlayerAssigned = null;
        OnTurnChanged = null;
        OnMarkPlaced = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetOnBoot()
    {
        Debug.Log($"All Turn Manager Events Listeners Cleared.");
        ClearAllListeners();
    }
}
