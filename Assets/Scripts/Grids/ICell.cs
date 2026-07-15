public interface ICell
{
    int Row { get; }
    int Column { get; }
    CurrentStatus Status { get; }
    void Initialize(int row, int col);
    void SetMark(CurrentStatus mark);
    void HighLight();
    void NormalizeCell();
}
