namespace BattleshipGame.Core.Commands;

public interface ICommandsFactory
{
    CommandValidationResult Validate(string str);

    ICommand Create(string str);
}

