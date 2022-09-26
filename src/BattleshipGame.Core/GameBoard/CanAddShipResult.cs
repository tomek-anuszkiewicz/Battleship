namespace BattleshipGame.Core.GameBoard;

public enum CanAddShipResult
{
    Ok,
    Outside,
    OnAnotherShip,
    NextToAnother
}
