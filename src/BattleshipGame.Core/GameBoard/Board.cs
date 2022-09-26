namespace BattleshipGame.Core.GameBoard;

internal class Board : IBoard
{
    protected readonly CellStatus[,] _cells;

    private readonly List<ShipOnBoard> _shipOnBoardList = new();

    protected readonly IConfig _config;

    private readonly IRandomWrapper _randomWrapper;

    public int Width { get; }

    public int Height { get; }

    public int FireCounter { get; private set; }

    public IReadOnlyList<ShipOnBoard> ShipOnBoardList => _shipOnBoardList;

    public Board(IConfig config, IRandomWrapper randomWrapper)
    {
        _config = config;
        _randomWrapper = randomWrapper;
        Width = _config.BoardCellsSize.Width;
        Height = _config.BoardCellsSize.Height;
        _cells = new CellStatus[Width, Height];
    }

    public CellStatus this[BoardPosition position]
    {
        get => _cells[position.X, position.Y];
        private set => _cells[position.X, position.Y] = value;
    }

    public CellStatus this[int x, int y]
    {
        get => _cells[x, y];
        private set => _cells[x, y] = value;
    }

    public bool IsAllSunk() => ShipOnBoardList.All(el => el.IsSunk());

    public CanAddShipResult CanAddShip(Ship ship)
    {
        if (!ship.FitOnBoard(_config.BoardCellsSize))
            return CanAddShipResult.Outside;

        if (ship.GetShipCells().Any(el => this[el] == CellStatus.Ship))
            return CanAddShipResult.OnAnotherShip;

        if (ship.GetSurroundingWaterCells(_config.BoardCellsSize).Any(el => this[el] == CellStatus.Ship))
            return CanAddShipResult.NextToAnother;

        return CanAddShipResult.Ok;
    }

    public void AddShip(Ship ship)
    {
        if (CanAddShip(ship) != CanAddShipResult.Ok)
            throw new InvalidOperationException();

        _shipOnBoardList.Add(new ShipOnBoard(this, ship.PositionLeftOrTop, ship.ShipOrientation, ship.Length));
        foreach (var cell in ship.GetShipCells())
            this[cell] = CellStatus.Ship;
    }

    public HitResult Fire(BoardPosition position)
    {
        HitResult WaterHit()
        {
            this[position] = CellStatus.WaterHit;
            return HitResult.Water;
        }

        HitResult ShipHit()
        {
            var shipStatus = ShipOnBoardList.First(el => el.Contains(position));

            this[position] = CellStatus.ShipHit;

            if (shipStatus.IsSunk())
            {
                foreach (var cell in shipStatus.GetSurroundingWaterCells(_config.BoardCellsSize))
                    this[cell] = CellStatus.WaterHit;
            }

            if (IsAllSunk())
                return HitResult.AllSunk;

            return shipStatus.IsSunk() ? HitResult.ShipSunk : HitResult.Ship;
        }

        FireCounter++;

        return this[position] switch
        {
            CellStatus.WaterHit => HitResult.WaterAgain,
            CellStatus.ShipHit => HitResult.ShipAgain,
            CellStatus.Water => WaterHit(),
            CellStatus.Ship => ShipHit(),
        };
    }

    public void Clear()
    {
        _shipOnBoardList.Clear();
        Array.Clear(_cells);
    }

    public void ArrangeShipsOnComputerBoard(IReadOnlyList<int> shipLengthList)
    {
        bool TryPutShip(int shipLength)
        {
            var position = _randomWrapper.NextPosition(_config.BoardCellsSize);
            var shipOrientation = _randomWrapper.NextEnum<ShipOrientation>();
            var ship = new Ship(position, shipOrientation, shipLength);

            if (CanAddShip(ship) == CanAddShipResult.Ok)
            {
                AddShip(ship);
                return true;
            }

            return false;
        }

        for (; ; )
        {
            Clear();

            foreach (var shipLength in shipLengthList)
            {
                if (!TryPutShip(shipLength))
                    break;
            }

            if (ShipOnBoardList.Count == shipLengthList.Count)
                break;
        }
    }
}
