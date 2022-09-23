namespace BattleshipGame.Core.Commands;

public class ExitCommand : ICommand
{
    public const string Name = "exit";

    private readonly IExitRequested _exitRequested;

    public ExitCommand(IExitRequested exitRequested)
    {
        _exitRequested = exitRequested;
    }

    public void Apply(string str) { }

    public bool CanApply(string str) => str.Trim() == Name;

    public string Execute()
    {
        _exitRequested.IsExitRequested = true;
        return "Thanks for using me. Bye";
    }
}