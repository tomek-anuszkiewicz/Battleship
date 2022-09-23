namespace BattleshipGame.Core.Commands;

public class PrintBoardCommand : ICommand
{
    public const string Name = "print";

    private readonly IBoard _board;

    private readonly IBoardDrawEngine _boardDrawEngine;

    private readonly IConfig _config;

    public PrintBoardCommand(IBoard board, IBoardDrawEngine drawEngine, IConfig config)
    {
        _board = board;
        _boardDrawEngine = drawEngine;
        _config = config;
    }

    public bool CanApply(string str) => str == Name;

    public void Apply(string str) { }

    public string Execute()
    {
        _boardDrawEngine.Init(IBoardDrawEngine.BorderWidth + _config.BoardCellsSize.Width, IBoardDrawEngine.BorderHeight + _config.BoardCellsSize.Height);

        _boardDrawEngine.DrawCoordinates(_board.Width, _board.Height);

        for (var x = 0; x < _board.Width; x++)
        {
            for (var y = 0; y < _board.Height; y++)
            {
                var cellStatus = _board[x, y];

                var p = new ScreenPoint(x + IBoardDrawEngine.BorderWidth, y + IBoardDrawEngine.BorderHeight);

                switch (cellStatus)
                {
                    case CellStatus.Ship: _boardDrawEngine.DrawShip(p, p); break;
                    case CellStatus.ShipHit: _boardDrawEngine.DrawShipHit(p); break;
                    case CellStatus.Water: _boardDrawEngine.DrawWater(p); break;
                    case CellStatus.WaterHit: _boardDrawEngine.DrawWaterHit(p); break;
                }
            }
        }

        _boardDrawEngine.Print();

        return "";
    }
}

