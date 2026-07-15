using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour, IPlayer
{
    public TurnManager.PlayerId Id { get; private set; }
    public CurrentStatus Mark { get; private set; }

    private GridController gridController;

    public void Initialize(TurnManager.PlayerId id, CurrentStatus mark)
    {
        Id = id;
        Mark = mark;
        gridController = FindAnyObjectByType<GridController>();
    }

    public void StartTurn()
    {
        StartCoroutine(TakeTurnRoutine());
    }

    private IEnumerator TakeTurnRoutine()
    {
        // Simulate thinking delay
        yield return new WaitForSeconds(0.5f);

        if (gridController == null)
            yield break;

        // Find an empty cell
        List<(int row, int col)> emptyCells = new List<(int, int)>();
        for (int r = 0; r < gridController.Rows; r++)
        {
            for (int c = 0; c < gridController.Columns; c++)
            {
                if (gridController.IsInBounds(r, c))
                {
                    var cell = gridController.GetCell(r, c);
                    if (cell != null && cell.Status == CurrentStatus.Empty)
                    {
                        emptyCells.Add((r, c));
                    }
                }
            }
        }

        if (emptyCells.Count > 0)
        {
            (int row, int col) choice = (-1, -1);
            CurrentStatus opponentMark = Mark == CurrentStatus.O ? CurrentStatus.X : CurrentStatus.O;

            // 1. Check if AI can win on this turn
            var winMove = FindWinningMove(Mark, emptyCells);
            
            // 2. Check if Opponent can win on their next turn (Block them)
            var blockMove = FindWinningMove(opponentMark, emptyCells);

            if (winMove.row != -1)
            {
                choice = winMove; // Take the win!
            }
            else if (blockMove.row != -1)
            {
                choice = blockMove; // Block the opponent!
            }
            else
            {
                // 3. Otherwise, pick random
                choice = emptyCells[Random.Range(0, emptyCells.Count)];
            }

            GridEvents.RaiseCellClicked(choice.row, choice.col);
        }
    }

    private (int row, int col) FindWinningMove(CurrentStatus markToTest, List<(int row, int col)> emptyCells)
    {
        foreach (var emptyCell in emptyCells)
        {
            int r = emptyCell.row;
            int c = emptyCell.col;

            // Check Row
            bool rowWin = true;
            for (int i = 0; i < gridController.Columns; i++)
            {
                var cellStatus = (i == c) ? markToTest : gridController.GetCell(r, i)?.Status ?? CurrentStatus.Empty;
                if (cellStatus != markToTest) { rowWin = false; break; }
            }
            if (rowWin) return (r, c);

            // Check Column
            bool colWin = true;
            for (int i = 0; i < gridController.Rows; i++)
            {
                var cellStatus = (i == r) ? markToTest : gridController.GetCell(i, c)?.Status ?? CurrentStatus.Empty;
                if (cellStatus != markToTest) { colWin = false; break; }
            }
            if (colWin) return (r, c);

            // Check Diagonals
            int minDim = Mathf.Min(gridController.Rows, gridController.Columns);
            if (minDim > 1)
            {
                if (r == c) // Top-Left to Bottom-Right
                {
                    bool diag1Win = true;
                    for (int i = 0; i < minDim; i++)
                    {
                        var cellStatus = (i == r && i == c) ? markToTest : gridController.GetCell(i, i)?.Status ?? CurrentStatus.Empty;
                        if (cellStatus != markToTest) { diag1Win = false; break; }
                    }
                    if (diag1Win) return (r, c);
                }

                if (r + c == gridController.Columns - 1) // Top-Right to Bottom-Left
                {
                    bool diag2Win = true;
                    for (int i = 0; i < minDim; i++)
                    {
                        var cellStatus = (i == r && (gridController.Columns - 1 - i) == c) ? markToTest : gridController.GetCell(i, gridController.Columns - 1 - i)?.Status ?? CurrentStatus.Empty;
                        if (cellStatus != markToTest) { diag2Win = false; break; }
                    }
                    if (diag2Win) return (r, c);
                }
            }
        }
        
        return (-1, -1);
    }
}
