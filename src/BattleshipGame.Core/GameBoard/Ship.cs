namespace BattleshipGame.Core.GameBoard;

public record Ship(BoardPosition PositionLeftOrTop, ShipOrientation ShipOrientation, int Length)
{
    public IEnumerable<BoardPosition> GetShipCells() => AsEnumerable();

    public IEnumerable<BoardPosition> GetSurroundingWaterCells(BoardCellsSize boardCellsSize) =>
        GetShipCells().SelectMany(el => el.GetSurroundingCells(boardCellsSize)).Distinct().Except(GetShipCells());

    public bool FitOnBoard(BoardCellsSize boardCellsSize) => GetShipCells().All(el => el.IsValid(boardCellsSize));

    public IEnumerable<BoardPosition> AsEnumerable() =>
         Enumerable.Range(0, Length).AsEnumerable().Select(l =>
             (ShipOrientation == ShipOrientation.Vertical)
                ? PositionLeftOrTop.Add(0, l)
                : PositionLeftOrTop.Add(l, 0));

    public bool Contains(BoardPosition position) => GetShipCells().Any(el => el == position);
}
