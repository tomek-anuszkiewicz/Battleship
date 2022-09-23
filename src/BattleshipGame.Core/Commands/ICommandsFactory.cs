namespace BattleshipGame.Core.Commands;

public interface ICommandsFactory
{
    bool CanCreate(string str);

    ICommand Create(string str);
}

