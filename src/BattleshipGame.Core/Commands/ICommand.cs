namespace BattleshipGame.Core.Commands;

public interface ICommand
{
    CommandValidationResult Validate(string str);

    void Apply(string str);

    string Execute();
}

