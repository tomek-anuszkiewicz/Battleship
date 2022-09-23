namespace BattleshipGame.Console;

internal class ConsoleColorMapper : IConsoleColorMapper
{
    public ConsoleColor MapColor(GameColor gameColor) =>
        gameColor switch
        {
            GameColor.Text => ConsoleColor.White,
            GameColor.Error => ConsoleColor.Red,
            GameColor.Background => ConsoleColor.Black,
            GameColor.Ship => ConsoleColor.White,
            GameColor.Water => ConsoleColor.Blue,
            GameColor.WaterLight => ConsoleColor.DarkCyan,
            GameColor.Hit => ConsoleColor.Red,
            GameColor.Border => ConsoleColor.White,
            GameColor.BorderBackground => ConsoleColor.DarkGreen,
            _ => throw new NotImplementedException(),
        };
}