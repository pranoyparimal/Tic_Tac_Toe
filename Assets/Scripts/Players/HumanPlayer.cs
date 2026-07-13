using UnityEngine;

public class HumanPlayer : MonoBehaviour, IPlayer
{
    public TurnManager.PlayerId Id { get; private set; }
    public CurrentStatus Mark { get; private set; }

    private bool isMyTurn;

    public void Initialize(TurnManager.PlayerId id, CurrentStatus mark)
    {
        Id = id;
        Mark = mark;
    }

    public void StartTurn()
    {
        isMyTurn = true;
        GridEvents.CellClickedUI += HandleUISelection;
    }

    private void HandleUISelection(int row, int col)
    {
        if (!isMyTurn) return;

        // Stop listening once a valid selection is made for this turn
        isMyTurn = false;
        GridEvents.CellClickedUI -= HandleUISelection;
        
        // Pass it to the logical layer
        GridEvents.RaiseCellClicked(row, col);
    }

    private void OnDisable()
    {
        GridEvents.CellClickedUI -= HandleUISelection;
    }
}
