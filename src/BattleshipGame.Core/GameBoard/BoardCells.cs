namespace BattleshipGame.Core.GameBoard;

public class BoardCells
{
    protected readonly CellStatus[,] _board;

    public int Width { get; }

    public int Height { get; }

    public BoardCellsSize BoardCellsSize  { get; }

    public BoardCells(CellStatus[,] board)
    {
        Width = board.GetLength(0);
        Height = board.GetLength(1);
        BoardCellsSize = new BoardCellsSize(Width, Height);
        _board = board;
    }

    public BoardCells(int width, int height)
    {
        Width = width;
        Height = height;
        BoardCellsSize = new BoardCellsSize(Width, Height);
        _board = new CellStatus[width, height];
    }

    public CellStatus this[BoardPosition position]
    {
        get => _board[position.X, position.Y];
        set => _board[position.X, position.Y] = value;
    }

    public CellStatus this[int x, int y]
    {
        get => _board[x, y];
        set => _board[x, y] = value;
    }

    public BoardCells Clone() => new((CellStatus[,])_board.Clone());

    public void Clear() => Array.Clear(_board);

    public IEnumerable<(BoardPosition Position, CellStatus CellStatus)> AsEnumerable()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                yield return (new BoardPosition(x, y), _board[x, y]);
            }
        }
    }
}

