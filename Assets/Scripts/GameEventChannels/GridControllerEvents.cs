using UnityEngine;
using System;

public static class GridControllerEvents
{
    /// <summary>Event Fired when a grid is selected on the board /// </summary>
    public static event Action<ICell> OnCellSelected; // re-broadcast for TurnManager, ScoreTracker, etc.
    public static void RaiseOnCellSelected (ICell cell) => OnCellSelected?.Invoke (cell);
    /// <summary>Event Fired when a player wins the game  /// </summary>
    public static event Action<CurrentStatus> OnGameWon;
    public static void RaiseOnGameWon(CurrentStatus _status) => OnGameWon?.Invoke (_status);
    /// <summary>Event fired when the game is drawn & no player wins the game  /// </summary>
    public static event Action OnGameDraw;
    public static void RaiseOnGameDraw() => OnGameDraw?.Invoke ();

    /// <summary>Fired when the grid is cleared back to all-Empty (restart flow).</summary>
    public static event Action OnGridReset;
    public static void RaiseOnGridReset() => OnGridReset?.Invoke ();

    /// <summary> Invoked by GridController when the game over, to Start a new game or etc. /// </summary>
    public static event Action OnGameOver;
    public static void RaiseOnGameOver() => OnGameOver?.Invoke ();


    // Handy for domain-reload safety in the editor (script recompiles, etc.)
    public static void ClearAllListeners()
    {
        OnCellSelected = null;
        OnGameWon = null;
        OnGameDraw = null;
        OnGameOver = null;
        OnGridReset = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetOnBoot()
    {
        Debug.Log($"All Grid Controller Events Listeners Cleared.");
        ClearAllListeners();
    }
}
