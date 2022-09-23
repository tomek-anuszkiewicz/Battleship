namespace BattleshipGame.Core.Commands;

internal class CommandsFactory : ICommandsFactory
{
    private readonly IEnumerable<ICommand> _commands;

    public CommandsFactory(IEnumerable<ICommand> commands)
    {
        _commands = commands;
    }

    public bool CanCreate(string str) => _commands.Count(el => el.CanApply(str.ToLower().Trim())) == 1;

    public ICommand Create(string str)
    {
        str = str.ToLower().Trim();
        var command = _commands.Single(el => el.CanApply(str));
        command.Apply(str);
        return command;
    }
}

