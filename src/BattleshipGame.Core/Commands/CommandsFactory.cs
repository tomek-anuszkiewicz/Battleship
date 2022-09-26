using System.Linq;

namespace BattleshipGame.Core.Commands;

internal class CommandsFactory : ICommandsFactory
{
    private readonly IEnumerable<ICommand> _commands;

    public CommandsFactory(IEnumerable<ICommand> commands)
    {
        _commands = commands;
    }

    public CommandValidationResult Validate(string str)
    {
        var validationResults = _commands.Select(el => el.Validate(str.ToLower().Trim())).ToList();

        var knownSomething = validationResults.SingleOrDefault(el => el.Status is CommandValidationStatus.KnownAndCanExecute or CommandValidationStatus.KnownButError);
        if (knownSomething != null)
            return knownSomething;

        return CommandValidationResult.Unknown;
    }
    
    public ICommand Create(string str)
    {
        str = str.ToLower().Trim();
        var command = _commands.Single(el => el.Validate(str).Status == CommandValidationStatus.KnownAndCanExecute);
        command.Apply(str);
        return command;
    }
}

