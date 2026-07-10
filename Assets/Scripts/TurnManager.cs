using System;
using UnityEngine;

[RequireComponent(typeof(GridController))]
public class TurnManager : MonoBehaviour
{
    public enum PlayerId { Player1, Player2 }

    [SerializeField] private GridController gridController;

    [Tooltip("If false, Player1 = O and Player2 = X (matches GDD default). If true, marks are coin-flipped at game start.")]
    [SerializeField] private bool randomizeMarks = false;

    public PlayerId CurrentPlayer { get; private set; }
    public CellGrid.CurrentStatus CurrentMark => GetMarkFor(CurrentPlayer);

    private CellGrid.CurrentStatus player1Mark;
    private CellGrid.CurrentStatus player2Mark;

    public event Action<PlayerId, CellGrid.CurrentStatus> OnPlayerAssigned;
    public event Action<PlayerId> OnTurnChanged;

    private void Awake()
    {
        if (gridController == null)
            gridController = GetComponent<GridController>();
    }

    private void OnEnable() => gridController.OnCellSelected += HandleCellSelected;
    private void OnDisable() => gridController.OnCellSelected -= HandleCellSelected;

    private void Start() => StartNewGame();

    public void StartNewGame()
    {
        bool player1IsO = !randomizeMarks || UnityEngine.Random.value < 0.5f;

        player1Mark = player1IsO ? CellGrid.CurrentStatus.O : CellGrid.CurrentStatus.X;
        player2Mark = player1IsO ? CellGrid.CurrentStatus.X : CellGrid.CurrentStatus.O;

        CurrentPlayer = PlayerId.Player1; // O conventionally moves first

        OnPlayerAssigned?.Invoke(PlayerId.Player1, player1Mark);
        OnPlayerAssigned?.Invoke(PlayerId.Player2, player2Mark);
        OnTurnChanged?.Invoke(CurrentPlayer);

        gridController.ResetGameState();
    }

    private void HandleCellSelected(CellGrid cell)
    {
        if (cell.Status != CellGrid.CurrentStatus.Empty) return; // redundant safety net

        cell.SetMark(CurrentMark);

        CurrentPlayer = CurrentPlayer == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1;
        OnTurnChanged?.Invoke(CurrentPlayer);
    }

    public CellGrid.CurrentStatus GetMarkFor(PlayerId player) =>
        player == PlayerId.Player1 ? player1Mark : player2Mark;
}