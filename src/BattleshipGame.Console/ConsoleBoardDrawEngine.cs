namespace BattleshipGame.Console;

internal class ConsoleBoardDrawEngine : IBoardDrawEngine
{
    private record ColorfullChar(char Char, ConsoleColor TextColor = ConsoleColor.White, ConsoleColor BackgroundColor = ConsoleColor.Black)
    {
        public static ColorfullChar Solid(ConsoleColor color = ConsoleColor.Black) => new(' ', color, color);
    }

    private readonly IConsoleWrapper _consoleWrapper;

    private readonly IConsoleColorMapper _consoleColorMapper;

    private ColorfullChar[,] _screen = new ColorfullChar[0, 0];

    private int _width = 0;

    private int _height = 0;

    public ConsoleBoardDrawEngine(IConsoleWrapper consoleWrapper, IConsoleColorMapper consoleColorMapper)
    {
        _consoleWrapper = consoleWrapper;
        _consoleColorMapper = consoleColorMapper;
    }

    private void DrawChar(ScreenPoint point, ColorfullChar colorfullChar) =>
        DrawChar(point.X, point.Y, colorfullChar);

    private void DrawChar(int x, int y, ColorfullChar colorfullChar)
    {
        if (x < 0 || y < 0 || x >= _width || y >= _height)
            return;

        _screen[x, y] = colorfullChar;
    }

    private void FillRect(int x1, int y1, int x2, int y2, ColorfullChar colorfullChar)
    {
        for (var x = x1; x <= x2; x++)
        {
            for (var y = y1; y <= y2; y++)
            {
                DrawChar(x, y, colorfullChar);
            }
        }
    }

    private void FillRect(ScreenPoint leftTop, ScreenPoint rightBottom, ColorfullChar colorfullChar) =>
        FillRect(leftTop.X, leftTop.Y, rightBottom.X, rightBottom.Y, colorfullChar);

    private void DrawText(ScreenPoint point, string text, ConsoleColor textColor, ConsoleColor backgroundColor) =>
        DrawText(point.X, point.Y, text, textColor, backgroundColor);

    private void DrawText(int x, int y, string text, ConsoleColor textColor, ConsoleColor backgroundColor)
    {
        for (var i = 0; i < text.Length; i++)
            DrawChar(x + i, y, new ColorfullChar(text[i], textColor, backgroundColor));
    }

    public void DrawChar(ScreenPoint point, char c, GameColor textColor = GameColor.Text, GameColor backgroundColor = GameColor.Background) =>
        DrawChar(point, new ColorfullChar(c, _consoleColorMapper.MapColor(textColor), _consoleColorMapper.MapColor(backgroundColor)));

    public void FillChar(ScreenPoint point, GameColor backgroundColor = GameColor.Background) =>
        DrawChar(point, ColorfullChar.Solid(_consoleColorMapper.MapColor(backgroundColor)));

    public void DrawText(ScreenPoint point, string message, GameColor textColor = GameColor.Text, GameColor backgroundColor = GameColor.Background) =>
        DrawText(point, message, _consoleColorMapper.MapColor(textColor), _consoleColorMapper.MapColor(backgroundColor));

    public void FillRect(ScreenPoint startPoint, ScreenPoint endPoint, GameColor color) =>
        FillRect(startPoint, endPoint, ColorfullChar.Solid(_consoleColorMapper.MapColor(color)));

    public void DrawCoordinates(int width, int height, GameColor textColor = GameColor.Border, GameColor backgroundColor = GameColor.Background)
    {
        var consoleTextColor = _consoleColorMapper.MapColor(textColor);
        var consoleBackgroundColor = _consoleColorMapper.MapColor(backgroundColor);

        for (int x = 0; x < width; x++)
            DrawChar(x + IBoardDrawEngine.BorderWidth, 0, new ColorfullChar((char)('a' + x), consoleTextColor, consoleBackgroundColor));

        for (int y = 0; y < height; y++)
            DrawText(0, y + IBoardDrawEngine.BorderHeight, (y + 1).ToString().PadLeft(2, ' '), consoleTextColor, consoleBackgroundColor);
    }

    public void Init(int width, int height)
    {
        _screen = new ColorfullChar[width, height];
        _height = height;
        _width = width;

        FillRect(
            new ScreenPoint(0, 0),
            new ScreenPoint(_width - 1, _height - 1),
             ColorfullChar.Solid(ConsoleColor.Black));
    }

    public void Print()
    {
        _consoleWrapper.WriteLine("");

        var originalForegroundColor = _consoleWrapper.ForegroundColor;
        var originalBackgroundColor = _consoleWrapper.BackgroundColor;

        for (var y = 0; y < _height; y++)
        {
            var sb = new StringBuilder(_width);
            var textColor = _screen[0, y].TextColor;
            var backgroundColor = _screen[0, y].BackgroundColor;

            void WriteLinePart()
            {
                _consoleWrapper.ForegroundColor = textColor;
                _consoleWrapper.BackgroundColor = backgroundColor;
                _consoleWrapper.Write(sb.ToString());
            }

            for (var x = 0; x < _width; x++)
            {
                if (_screen[x, y].BackgroundColor != backgroundColor || _screen[x, y].TextColor != textColor)
                {
                    WriteLinePart();

                    sb.Clear();
                    textColor = _screen[x, y].TextColor;
                    backgroundColor = _screen[x, y].BackgroundColor;
                }

                sb.Append(_screen[x, y].Char);
            }

            WriteLinePart();

            _consoleWrapper.BackgroundColor = originalBackgroundColor;
            _consoleWrapper.WriteLine("");
        }

        _consoleWrapper.ForegroundColor = originalForegroundColor;
        _consoleWrapper.BackgroundColor = originalBackgroundColor;
    }

    public void DrawShipHit(ScreenPoint point) =>
            DrawChar(point, 'X', GameColor.Hit, GameColor.Ship);
    public void DrawWater(ScreenPoint point) =>
        FillChar(point, (point.X % 2 == 1) && (point.Y % 2 == 1) ? GameColor.WaterLight : GameColor.Water);

    public void DrawWaterHit(ScreenPoint point) =>
        DrawText(point, "X", GameColor.Hit, GameColor.Water);

    public void DrawShip(ScreenPoint startPoint, ScreenPoint endPoint, GameColor color = GameColor.Ship) =>
        FillRect(startPoint, endPoint, color);
}
