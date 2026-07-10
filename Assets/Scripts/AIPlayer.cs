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
            var choice = emptyCells[Random.Range(0, emptyCells.Count)];
            GridEvents.RaiseCellClicked(choice.row, choice.col);
        }
    }
}
