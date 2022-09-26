namespace BattleshipGame.Core.Commands;

public enum CommandValidationStatus
{
    Unknown,
    KnownAndCanExecute,
    KnownButError
}
