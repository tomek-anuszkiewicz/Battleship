namespace BattleshipGame.Core.GameBoard;

public record BoardPosition(int X, int Y)
{
    public IEnumerable<BoardPosition> GetSurroundingCells(BoardCellsSize boardCellsSize)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                var position = new BoardPosition(X + x, Y + y);
                if ((position != this) && position.IsValid(boardCellsSize))
                    yield return position;
            }
        }
    }

    public IEnumerable<BoardPosition> GetStraightCells(BoardCellsSize boardCellsSize) => 
        GetSurroundingCells(boardCellsSize).Where(el => (el.X == X) || (el.Y == Y));

    public BoardPosition Add(int dx, int dy) => new(X + dx, Y + dy);

    public bool IsValid(BoardCellsSize boardCellsSize) => (X >= 0) && (X < boardCellsSize.Width) && (Y >= 0) && (Y < boardCellsSize.Height);

}
