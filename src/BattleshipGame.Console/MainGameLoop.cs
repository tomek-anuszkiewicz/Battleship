namespace BattleshipGame.Console;

internal class MainGameLoop : IMainGameLoop
{
    private readonly IBoard _board;

    private readonly IConsoleWrapper _consoleWrapper;

    private readonly IConfig _config;

    private readonly ICommandsFactory _commandsFactory;

    private readonly IExitRequested _exitRequested;

    private readonly IConsoleColorMapper _consoleColorMapper;

    public MainGameLoop(
        IBoard board,
        IConsoleWrapper consoleWrapper,
        IConfig config,
        ICommandsFactory commandsFactory,
        IExitRequested exitRequested, 
        IConsoleColorMapper consoleColorMapper)
    {
        _board = board;
        _consoleWrapper = consoleWrapper;
        _config = config;
        _commandsFactory = commandsFactory;
        _exitRequested = exitRequested;
        _consoleColorMapper = consoleColorMapper;
    }

    public void RunInLoop()
    {
        SetBackgroundColor(GameColor.Background);
        SetTextColor(GameColor.Text);

        SetTextColor(GameColor.WaterLight);

        _consoleWrapper.WriteLine("");
        _consoleWrapper.WriteLine(@" ______         _    _    _             _      _        ");
        _consoleWrapper.WriteLine(@" | ___ \       | |  | |  | |           | |    (_)       ");
        _consoleWrapper.WriteLine(@" | |_/ /  __ _ | |_ | |_ | |  ___  ___ | |__   _  _ __  ");
        _consoleWrapper.WriteLine(@" | ___ \ / _` || __|| __|| | / _ \/ __|| '_ \ | || '_ \ ");
        _consoleWrapper.WriteLine(@" | |_/ /| (_| || |_ | |_ | ||  __/\__ \| | | || || |_) |");
        _consoleWrapper.WriteLine(@" \____/  \__,_| \__| \__||_| \___||___/|_| |_||_|| .__/ ");
        _consoleWrapper.WriteLine(@"                                                 | |    ");
        _consoleWrapper.WriteLine(@"                                                 |_|    ");
        _consoleWrapper.WriteLine("");
        _consoleWrapper.WriteLine("Welcome in Battleship game");
        _consoleWrapper.WriteLine("");

        SetTextColor(GameColor.Text);

        _consoleWrapper.WriteLine("This is simple version. Just discover all ships places by computer.");
        _consoleWrapper.WriteLine($"Board size: {_config.BoardCellsSize.Width}x{_config.BoardCellsSize.Height}");
        _consoleWrapper.WriteLine($"There are {_config.ShipLengthList.Count} ships on board.");
        _consoleWrapper.WriteLine($"Their lengths: {string.Join(", ", _config.ShipLengthList)}");
        _consoleWrapper.WriteLine("You need enter coordinates like A5, C6.");
        _consoleWrapper.WriteLine($"Type {ExitCommand.Name} to exit.");
        _consoleWrapper.WriteLine($"Type {PrintBoardCommand.Name} to print board. Just for debug purpose.");

        _board.ArrangeShipsOnComputerBoard(_config.ShipLengthList);
        _consoleWrapper.WriteLine("");
        _consoleWrapper.WriteLine("All ships has been placed, let's play!");
        _consoleWrapper.WriteLine("");

        while (true)
        {
            SwapTextAndBackgroundColors();

            _consoleWrapper.Write("Issue command:");

            SwapTextAndBackgroundColors();

            _consoleWrapper.WriteLine("");

            var strPosition = _consoleWrapper.ReadLine();

            if (strPosition.Trim() == "")
                continue;

            var validationResult = _commandsFactory.Validate(strPosition);
            string error = "";

            if (validationResult.Status == CommandValidationStatus.Unknown)
                error = "Please enter valid command.";
            if (validationResult.Status == CommandValidationStatus.KnownButError)
                error = validationResult.Error;

            if (error != "")
            {
                SetTextColor(GameColor.Error);
                _consoleWrapper.WriteLine(error);
                SetTextColor(GameColor.Text);
                continue;
            }

            var command = _commandsFactory.Create(strPosition);

            var result = command.Execute();

            if (result != null)
                _consoleWrapper.WriteLine(result);

            if (_board.IsAllSunk())
                break;
            if (_exitRequested.IsExitRequested)
                break;
        }
    }

    private void SetTextColor(GameColor gameColor) => _consoleWrapper.ForegroundColor = _consoleColorMapper.MapColor(gameColor);

    private void SetBackgroundColor(GameColor gameColor) => _consoleWrapper.BackgroundColor = _consoleColorMapper.MapColor(gameColor);

    private void SwapTextAndBackgroundColors()
    {
        (_consoleWrapper.ForegroundColor, _consoleWrapper.BackgroundColor) = 
            (_consoleWrapper.BackgroundColor, _consoleWrapper.ForegroundColor);
    }
}

