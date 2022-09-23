namespace BattleshipGame.Core.DrawLogic;

public interface IBoardDrawEngine
{
    public const int BorderWidth = 2;

    public const int BorderHeight = 1;

    void Init(int width, int height);

    void Print();

    void DrawCoordinates(int width, int height, GameColor textColor = GameColor.Border, GameColor backgroundColor = GameColor.BorderBackground);

    void DrawShipHit(ScreenPoint point);

    void DrawWater(ScreenPoint point);

   void DrawWaterHit(ScreenPoint point);

    void DrawShip(ScreenPoint startPoint, ScreenPoint endPoint, GameColor color = GameColor.Ship);
}

