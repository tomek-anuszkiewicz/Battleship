namespace BattleshipGame.Console;

public interface IConsoleColorMapper
{
    ConsoleColor MapColor(GameColor gameColor);
}