namespace BattleshipGame.Core.GameBoard;

public record ShipOnBoard(IBoard Board, BoardPosition PositionLeftOrTop, ShipOrientation ShipOrientation, int Length) : 
    Ship(PositionLeftOrTop, ShipOrientation, Length)
{
    public bool IsSunk() => GetCellStatusList().All(el => el == ShipCellStatus.Hit);

    public bool IsHit() => GetCellStatusList().Any(el => el == ShipCellStatus.Hit);

    public IEnumerable<ShipCellStatus> GetCellStatusList() =>
        GetShipCells().Select(el => (Board[el] == CellStatus.ShipHit) ? ShipCellStatus.Hit : ShipCellStatus.Intact);
}
