namespace BattleshipGame.Core;

public interface IConfig
{
    BoardCellsSize BoardCellsSize { get; }

    IReadOnlyList<int> ShipLengthList { get; }
}
