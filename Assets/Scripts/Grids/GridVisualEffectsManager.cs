using System;
using UnityEngine;

public class GridVisualEffectsManager : MonoBehaviour
{
    private ICell[] cells;
    private void Awake()
    {
        cells = GetComponentsInChildren<ICell>(true);

        GridControllerEvents.OnWinningGridsAvailable += HandleGridEffectsAfterWinning;
        WinningPanelInteractionEvents.OnGameRestarted += HandleGameRestart;
    }

    private void OnDestroy()
    {
        GridControllerEvents.OnWinningGridsAvailable -= HandleGridEffectsAfterWinning;
        WinningPanelInteractionEvents.OnGameRestarted -= HandleGameRestart;
    }


    private void HandleGridEffectsAfterWinning(ICell[] _cells)
    {
        foreach (ICell cell in _cells)
        {
            cell.HighLight();
        }
    }
    private void HandleGameRestart()
    {
        foreach (ICell cell in cells)
        {
            cell.NormalizeCell();
        }
    }
}
