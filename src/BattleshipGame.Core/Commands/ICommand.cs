namespace BattleshipGame.Core.Commands;

public interface ICommand
{
    bool CanApply(string str);

    void Apply(string str);

    string Execute();
}

