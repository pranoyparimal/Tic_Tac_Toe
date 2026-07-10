using System;
using UnityEngine;

public static class GridEvents
{
    public static event Action<int, int> CellClicked;
    public static void RaiseCellClicked(int row, int col) => CellClicked?.Invoke(row, col);

    public static event Action<int, int> CellClickedUI;
    public static void RaiseCellClickedUI(int row, int col) => CellClickedUI?.Invoke(row, col);

    // Handy for domain-reload safety in the editor (script recompiles, etc.)
    public static void ClearAllListeners()
    {
        CellClicked = null;
        CellClickedUI = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetOnBoot()
    {
       Debug.Log($"All Grid Events Listeners Cleared.");
       ClearAllListeners();
    }
}


