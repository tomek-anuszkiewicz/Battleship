namespace BattleshipGame.Core.Commands;

public record CommandValidationResult(CommandValidationStatus Status, string Error)
{
    public static readonly CommandValidationResult Success = new(CommandValidationStatus.KnownAndCanExecute, "");

    public static readonly CommandValidationResult Unknown = new(CommandValidationStatus.Unknown, "");

    public static CommandValidationResult SuccessOrUnknown(bool result) => result ? Success : Unknown;

    public static CommandValidationResult KnownButError(string error) => new(CommandValidationStatus.KnownButError, error);
}