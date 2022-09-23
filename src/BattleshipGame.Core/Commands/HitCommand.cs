namespace BattleshipGame.Core.Commands;

public class HitCommand : ICommand
{
    private readonly IBoard _board;

    private readonly IConfig _config;

    private BoardPosition _boardPosition = new (0, 0);

    public HitCommand(IBoard board, IConfig config)
    {
        _board = board;
        _config = config;
    }

    private (int X, int Y, bool Valid) Decode(string str)
    {
        Regex regex = new Regex("([a-z])([1-9][0-9]*)");
        var match = regex.Match(str);
        if (!match.Success)
            return (0, 0, false);
        var letter = match.Groups[1].Value;
        var number = match.Groups[2].Value;

        var x = letter[0] - 'a';
        var y = int.Parse(number) - 1;

        if (x >= _config.BoardCellsSize.Width)
            return (0, 0, false);

        if (y >= _config.BoardCellsSize.Height)
            return (0, 0, false);

        return (x, y, true);
    }

    public bool CanApply(string str) => Decode(str).Valid;

    public void Apply(string str)
    {
        var (x, y, _) = Decode(str);
        _boardPosition = new BoardPosition(x, y);
    }

    public string Execute()
    {
        var hitResult = _board.Fire(_boardPosition);

        var hitMessage = hitResult switch
        {
            HitResult.Ship => "Ship hit, but still operating!",
            HitResult.ShipAgain => "Oh no! You waste your turn.",
            HitResult.ShipSunk => "Ship sunk! But still others operating.",
            HitResult.Water => "Miss!",
            HitResult.WaterAgain => "Oh no! You waste your turn.",
            HitResult.AllSunk => $"You win in {_board.FireCounter} moves.",
        };

        return hitMessage;
    }
}

