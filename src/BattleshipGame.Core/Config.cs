namespace BattleshipGame.Core;

internal class Config : IConfig
{
    public BoardCellsSize BoardCellsSize { get; } = new(10, 10);

    public IReadOnlyList<int> ShipLengthList { get; } = new int[] { 4, 4, 5 };
}