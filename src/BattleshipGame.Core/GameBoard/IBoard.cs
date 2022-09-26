namespace BattleshipGame.Core.GameBoard;

public interface IBoard
{
    CellStatus this[BoardPosition position] { get; }

    CellStatus this[int x, int y] { get; }

    int Width { get; }

    int Height { get; }

    int FireCounter { get; }

    void AddShip(Ship ship);

    CanAddShipResult CanAddShip(Ship ship);

    HitResult Fire(BoardPosition position);

    bool IsAllSunk();

    void Clear();

    void ArrangeShipsOnComputerBoard(IReadOnlyList<int> shipLengthList);
}
