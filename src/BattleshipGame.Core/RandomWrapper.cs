namespace BattleshipGame.Core;

internal class RandomWrapper : IRandomWrapper
{
    private readonly Random _random;

    public RandomWrapper()
    {
        _random = new Random();
    }

    public BoardPosition NextPosition(BoardCellsSize boardCellsSize) =>
        new(_random.Next(0, boardCellsSize.Width + 1), _random.Next(0, boardCellsSize.Height + 1));

    public T NextEnum<T>()
        where T : struct, Enum
    {
        var enumValues = Enum.GetValues<T>();
        var randomIndex = _random.Next(0, enumValues.Length);
        return enumValues[randomIndex];
    }

    public T Next<T>(T[] arr) => arr[_random.Next(0, arr.Length)];

}
