using System;
//using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(GridController))]
public class TurnManager : MonoBehaviour
{
    public enum PlayerId { Player1, Player2 }
    public PlayerId CurrentPlayer { get; private set; }

    [SerializeField] private GridController gridController;

    [Tooltip("If false, Player1 = O and Player2 = X (matches GDD default). If true, marks are coin-flipped at game start.")]
    [SerializeField] private bool randomizeMarks = false;

    public CurrentStatus CurrentMark => GetMarkFor(CurrentPlayer);

    private CurrentStatus player1Mark;
    private CurrentStatus player2Mark;

    private IPlayer[] players = new IPlayer[2];

    /*public event Action<PlayerId, CurrentStatus> OnPlayerAssigned;
    public event Action<PlayerId> OnTurnChanged;
    public event Action<int, int, CurrentStatus> OnMarkPlaced;*/

    private void Awake()
    {
        if (gridController == null)
            gridController = GetComponent<GridController>();

        // Default to two human players if none are assigned.
        // In a real game, these might be configured via UI.
        players[0] = gameObject.AddComponent<HumanPlayer>();
        players[1] = gameObject.AddComponent<AIPlayer>(); 

        TurnManagerEvents.OnMarkPlaced += gridController.HandleMarkPlaced;
        GridControllerEvents.OnGameOver += HandleOnGameOver;
    }


    private void OnEnable() => GridEvents.CellClicked += HandleCellSelected;
    private void OnDisable() => GridEvents.CellClicked -= HandleCellSelected;

    private void OnDestroy()
    {
        TurnManagerEvents.OnMarkPlaced -= gridController.HandleMarkPlaced;
        GridControllerEvents.OnGameOver -= HandleOnGameOver;
    }

    private void Start() => StartNewGame();

    public void StartNewGame()
    {
        bool player1IsO = !randomizeMarks || UnityEngine.Random.value < 0.5f;

        player1Mark = player1IsO ? CurrentStatus.O : CurrentStatus.X;
        player2Mark = player1IsO ? CurrentStatus.X : CurrentStatus.O;

        CurrentPlayer = PlayerId.Player1; // O conventionally moves first

        players[0].Initialize(PlayerId.Player1, player1Mark);
        players[1].Initialize(PlayerId.Player2, player2Mark);

        TurnManagerEvents.RaiseOnPlayerAssigned(PlayerId.Player1, player1Mark);
        //OnPlayerAssigned?.Invoke(PlayerId.Player1, player1Mark);

        TurnManagerEvents.RaiseOnPlayerAssigned(PlayerId.Player2, player2Mark);
        //OnPlayerAssigned?.Invoke(PlayerId.Player2, player2Mark);

        TurnManagerEvents.RaiseOnTurnChanged(CurrentPlayer);
        //OnTurnChanged?.Invoke(CurrentPlayer);

        gridController.ResetGameState();

        // Start the first turn
        players[0].StartTurn();
    }

    private void HandleOnGameOver()
    {
        Debug.Log($"A new game started.");
        StartNewGame();
    }
    private void HandleCellSelected(int row, int col)
    {
        
        var cell = gridController.GetCell(row, col);
        if (cell == null || cell.Status != CurrentStatus.Empty) return; // redundant safety net

        TurnManagerEvents.RaiseOnMarkPlaced(row, col, CurrentMark);

        // Halt the loop if that move ended the game
        if (gridController.IsGameOver) return;

        CurrentPlayer = CurrentPlayer == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1;
        TurnManagerEvents.RaiseOnTurnChanged(CurrentPlayer);
        //OnTurnChanged?.Invoke(CurrentPlayer);

        // Start next player's turn
        int nextPlayerIndex = CurrentPlayer == PlayerId.Player1 ? 0 : 1;
        players[nextPlayerIndex].StartTurn();
    }

    public CurrentStatus GetMarkFor(PlayerId player) =>
        player == PlayerId.Player1 ? player1Mark : player2Mark;
}