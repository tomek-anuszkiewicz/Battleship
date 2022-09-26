namespace BattleshipGame.Core;

public interface IRandomWrapper
{
    T Next<T>(T[] arr);

    T NextEnum<T>() where T : struct, Enum;

    BoardPosition NextPosition(BoardCellsSize boardCellsSize);
}