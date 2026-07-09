using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Single source of truth for the Tic Tac Toe grid's data. Holds no visual/UI
/// logic itself — GridCreator (or any other spawner) populates it via
/// Initialize()/RegisterCellObject(), and gameplay systems (turn manager,
/// win-check, restart flow, etc.) read/write through this controller.
/// </summary>
public class GridController : MonoBehaviour
{
    [Header("Grid Dimensions")]
    [SerializeField] private int rows = 3;
    [SerializeField] private int columns = 3;

    public int Rows => rows;
    public int Columns => columns;

    [SerializeField] private List<CellGrid> cells_grids = new();

    public bool IsGameOver { get; private set; }

    private readonly Dictionary<string, GameObject> cellObjectsById = new();

    /// <summary>Event Fired when a grid is selected on the board /// </summary>
    public event Action<CellGrid> OnCellSelected; // re-broadcast for TurnManager, ScoreTracker, etc.
    /// <summary>Event Fired when a player wins the game  /// </summary>
    public event Action<CellGrid.CurrentStatus> OnGameWon;
    /// <summary>Event fired when the game is drawn & no player wins the game  /// </summary>
    public event Action OnGameDraw;

    /// <summary>Fired when the grid is cleared back to all-Empty (restart flow).</summary>
    public event Action OnGridReset;

    private void OnEnable() => GridEvents.CellClicked += HandleCellClicked;

    private void OnDisable() => GridEvents.CellClicked -= HandleCellClicked;

    /// <summary>
    /// Allocates fresh cell data for an NxN (or NxM) grid. Call this once
    /// before registering any cell GameObjects.
    /// </summary>
    public void Initialize(int newRows, int newColumns)
    {
        rows = newRows;
        columns = newColumns;
        
        cellObjectsById.Clear();
        cells_grids.Clear();
    }

    /// <summary>Links a spawned visual cell GameObject to its data entry.</summary>
    public void RegisterCellObject(int row, int col, GameObject cellObject)
    {
        string id = GetCellId(row, col);
        cellObjectsById[id] = cellObject;

        CellGrid grid = cellObject.GetComponent<CellGrid>();
        if (cellObject != null && grid != null)
        {
            cells_grids.Add(grid);
        }
        else 
        {
            Debug.LogError($"Either cellObject is null for the CellGrid component on the cellObject is null");
        }
    }

   /* private void HandleCellClicked(CellGrid grid)
    {
        // safety: ignore clicks from cells that aren't part of *this* grid instance
        if (!cells_grids.Contains(grid)) return;

        Debug.Log($"The Grid is selcted: <color=red> ({grid.Row}, {grid.Column})</color>");
        OnCellSelected?.Invoke(grid);
        CheckWinLogic(grid);
    }*/

    private void HandleCellClicked(CellGrid cell)
    {
        if (IsGameOver) return;
        if (!cells_grids.Contains(cell)) return;

        OnCellSelected?.Invoke(cell);
        CheckWinCondition();
    }

    public void CheckWinCondition()
    {
        // 8 possible lines for a 3x3 — generalizable for NxN later if you want
        int[][] lines = {
            new[]{0,1,2}, new[]{3,4,5}, new[]{6,7,8}, // rows
            new[]{0,3,6}, new[]{1,4,7}, new[]{2,5,8}, // cols
            new[]{0,4,8}, new[]{2,4,6}                // diagonals
        };

        foreach (var line in lines)
        {
            var a = cells_grids[line[0]].status;
            var b = cells_grids[line[1]].status;
            var c = cells_grids[line[2]].status;

            if (a != CellGrid.CurrentStatus.Empty && a == b && b == c)
            {
                IsGameOver = true;
                OnGameWon?.Invoke(a);
                return;
            }
        }

        if (cells_grids.TrueForAll(cell => cell.status != CellGrid.CurrentStatus.Empty))
        {
            IsGameOver = true;
            OnGameDraw?.Invoke();
        }
    }

    /// <summary>Call from a restart button/flow to unlock the board for a new game.</summary>
    public void ResetGameState() => IsGameOver = false;

    /// <summary>Cell ID convention from the GDD, e.g. row 1, col 2 -> "C12".</summary>
    public static string GetCellId(int row, int col) => $"C{row}{col}";

    public GameObject GetCellObject(int row, int col) =>
        cellObjectsById.TryGetValue(GetCellId(row, col), out var obj) ? obj : null;
    

    public bool IsInBounds(int row, int col) => row >= 0 && row < rows && col >= 0 && col < columns;

}
