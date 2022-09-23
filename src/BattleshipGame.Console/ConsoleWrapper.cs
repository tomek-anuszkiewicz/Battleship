namespace BattleshipGame.Console;

internal class ConsoleWrapper : IConsoleWrapper
{
    public ConsoleColor ForegroundColor
    {
        get => System.Console.ForegroundColor;
        set => System.Console.ForegroundColor = value;
    }

    public ConsoleColor BackgroundColor
    {
        get => System.Console.BackgroundColor;
        set => System.Console.BackgroundColor = value;
    }

    public string ReadLine() => System.Console.ReadLine() ?? "";

    public void WriteLine(string value) => System.Console.WriteLine(value);

    public void Write(string value) => System.Console.Write(value);
}
