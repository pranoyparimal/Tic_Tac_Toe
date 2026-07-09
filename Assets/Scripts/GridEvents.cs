using System;
using UnityEngine;

public static class GridEvents
{
    public static event Action<CellGrid> CellClicked;
    public static void RaiseCellClicked(CellGrid cell) => CellClicked?.Invoke(cell);

    // Handy for domain-reload safety in the editor (script recompiles, etc.)
    public static void ClearAllListeners() => CellClicked = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetOnBoot()
    {
        Debug.Log($"All Listeners Cleared.");
       ClearAllListeners();
    }
}


