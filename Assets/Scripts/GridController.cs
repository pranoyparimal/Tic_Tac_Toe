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

    private readonly Dictionary<(int, int), CellGrid> cells_grids = new();

    public bool IsGameOver { get; private set; }

    //private readonly Dictionary<string, GameObject> cellObjectsById = new();

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
        
        cells_grids.Clear();
    }

    /// <summary>Links a spawned visual cell GameObject to its data entry.</summary>
    public void RegisterCellObject(int row, int col, GameObject cellObject)
    {
        string id = GetCellId(row, col);

        CellGrid grid = cellObject.GetComponent<CellGrid>();
        if (cellObject != null && grid != null)
        {
            cells_grids[(row, col)] = grid;
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
        if (!cells_grids.ContainsValue(cell)) return;

        OnCellSelected?.Invoke(cell);
        CheckWinCondition();
    }

    public void CheckWinCondition()
    {
        CellGrid.CurrentStatus GetStatus(int r, int c)
        {
            if (cells_grids.TryGetValue((r, c), out var cell))
                return cell.Status;
            return CellGrid.CurrentStatus.Empty;
        }

        // Check rows
        for (int r = 0; r < rows; r++)
        {
            var first = GetStatus(r, 0);
            if (first == CellGrid.CurrentStatus.Empty) continue;

            bool win = true;
            for (int c = 1; c < columns; c++)
            {
                if (GetStatus(r, c) != first)
                {
                    win = false;
                    break;
                }
            }
            if (win)
            {
                IsGameOver = true;
                OnGameWon?.Invoke(first);
                return;
            }
        }

        // Check columns
        for (int c = 0; c < columns; c++)
        {
            var first = GetStatus(0, c);
            if (first == CellGrid.CurrentStatus.Empty) continue;

            bool win = true;
            for (int r = 1; r < rows; r++)
            {
                if (GetStatus(r, c) != first)
                {
                    win = false;
                    break;
                }
            }
            if (win)
            {
                IsGameOver = true;
                OnGameWon?.Invoke(first);
                return;
            }
        }

        // Check diagonals
        int minDim = Mathf.Min(rows, columns);
        if (minDim > 1)
        {
            // Diagonal 1 (Top-Left to Bottom-Right)
            var firstDiag1 = GetStatus(0, 0);
            if (firstDiag1 != CellGrid.CurrentStatus.Empty)
            {
                bool win = true;
                for (int i = 1; i < minDim; i++)
                {
                    if (GetStatus(i, i) != firstDiag1)
                    {
                        win = false;
                        break;
                    }
                }
                if (win)
                {
                    IsGameOver = true;
                    OnGameWon?.Invoke(firstDiag1);
                    return;
                }
            }

            // Diagonal 2 (Top-Right to Bottom-Left)
            var firstDiag2 = GetStatus(0, columns - 1);
            if (firstDiag2 != CellGrid.CurrentStatus.Empty)
            {
                bool win = true;
                for (int i = 1; i < minDim; i++)
                {
                    if (GetStatus(i, columns - 1 - i) != firstDiag2)
                    {
                        win = false;
                        break;
                    }
                }
                if (win)
                {
                    IsGameOver = true;
                    OnGameWon?.Invoke(firstDiag2);
                    return;
                }
            }
        }

        // Check for draw (if all cells are filled)
        bool allFilled = true;
        foreach (var cell in cells_grids.Values)
        {
            if (cell.Status == CellGrid.CurrentStatus.Empty)
            {
                allFilled = false;
                break;
            }
        }
        
        if (allFilled && cells_grids.Count > 0)
        {
            IsGameOver = true;
            OnGameDraw?.Invoke();
        }
    }

    /// <summary>Call from a restart button/flow to unlock the board for a new game.</summary>
    public void ResetGameState() => IsGameOver = false;

    /// <summary>Cell ID convention from the GDD, e.g. row 1, col 2 -> "C12".</summary>
    public static string GetCellId(int row, int col) => $"C{row}{col}";
    
    public bool IsInBounds(int row, int col) => row >= 0 && row < rows && col >= 0 && col < columns;

}
