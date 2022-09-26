namespace BattleshipGame.Console;

public interface IConsoleWrapper
{
    ConsoleColor ForegroundColor { get; set; }

    ConsoleColor BackgroundColor { get; set; }

    string ReadLine();

    void Write(string value);

    void WriteLine(string value);
}