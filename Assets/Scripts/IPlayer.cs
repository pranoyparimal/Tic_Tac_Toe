public interface IPlayer
{
    TurnManager.PlayerId Id { get; }
    CurrentStatus Mark { get; }
    void StartTurn();
    void Initialize(TurnManager.PlayerId id, CurrentStatus mark);
}
