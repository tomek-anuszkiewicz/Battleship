namespace BattleshipGame.Core.Tests.GameBoard;

public class BoardTests
{
    private Mock<IConfig> _configMock = new Mock<IConfig>(MockBehavior.Strict);

    private Mock<IRandomWrapper> _randomWrapperMock = new Mock<IRandomWrapper>(MockBehavior.Strict);

    private IBoard CreateSut() => new Board(_configMock.Object, _randomWrapperMock.Object);

    public BoardTests()
    {
        _configMock.Setup(el => el.BoardCellsSize).Returns(new BoardCellsSize(10, 10));
    }

    [Fact]
    public void Fire_HitWater_WaterHit()
    {
        var sut = CreateSut();

        sut.Fire(new(0, 0)).Should().Be(HitResult.Water);
    }

    [Fact]
    public void Fire_HitWaterAgain_WaterHitAgain()
    {
        var sut = CreateSut();

        sut.Fire(new(0, 0));
        sut.Fire(new(0, 0)).Should().Be(HitResult.WaterAgain);
    }

    [Fact]
    public void Fire_HitShip_ShipHit()
    {
        var sut = CreateSut();

        sut.AddShip(new Ship(new(1, 1), ShipOrientation.Horizontal, 2));

        sut.Fire(new(1, 1)).Should().Be(HitResult.Ship);
    }

    [Fact]
    public void Fire_HitShipAgain_ShipHitAgain()
    {
        var sut = CreateSut();

        sut.AddShip(new Ship(new(1, 1), ShipOrientation.Horizontal, 2));

        sut.Fire(new(1, 1)).Should();

        sut.Fire(new(1, 1)).Should().Be(HitResult.ShipAgain);
    }

    [Fact]
    public void Fire_HitWholeShip_ShipSunk()
    {
        var sut = CreateSut();

        sut.AddShip(new Ship(new(1, 1), ShipOrientation.Horizontal, 2));
        sut.AddShip(new Ship(new(5, 1), ShipOrientation.Horizontal, 2));

        sut.Fire(new(1, 1)).Should();

        sut.Fire(new(2, 1)).Should().Be(HitResult.ShipSunk);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    [InlineData(3, 0)]
    [InlineData(0, 1)]
    [InlineData(3, 1)]
    [InlineData(0, 2)]
    [InlineData(1, 2)]
    [InlineData(2, 2)]
    [InlineData(3, 2)]
    public void Fire_SunkShip_MarkWaterAroundAsWaterHit(int x, int y)
    {
        var sut = CreateSut();

        sut.AddShip(new Ship(new(1, 1), ShipOrientation.Horizontal, 2));

        sut.Fire(new(1, 1));
        sut.Fire(new(2, 1));

        sut.Fire(new(x, y)).Should().Be(HitResult.WaterAgain);
    }

    [Fact]
    public void Fire_HitAllShips_AllSunk()
    {
        var sut = CreateSut();

        sut.AddShip(new Ship(new(0, 0), ShipOrientation.Horizontal, 3));
        sut.AddShip(new Ship(new(7, 5), ShipOrientation.Vertical, 3));

        sut.Fire(new(0, 0));
        sut.Fire(new(1, 0));
        sut.Fire(new(2, 0));
                           
        sut.Fire(new(7, 5));
        sut.Fire(new(7, 6));

        sut.IsAllSunk().Should().BeFalse();
        sut.Fire(new(7, 7)).Should().Be(HitResult.AllSunk);
        sut.IsAllSunk().Should().BeTrue();
    }

    [Fact]
    public void ArrangeShipsOnComputerBoard_Call_Clear()
    {
        var sut = CreateSut();

        sut.AddShip(new Ship(new(0, 0), ShipOrientation.Horizontal, 3));
        sut.AddShip(new Ship(new(7, 5), ShipOrientation.Vertical, 3));

        _randomWrapperMock.Setup(el => el.NextPosition(new(10, 10))).Returns(new BoardPosition(2, 3));
        _randomWrapperMock.Setup(el => el.NextEnum<ShipOrientation>()).Returns(ShipOrientation.Vertical);

        sut.ArrangeShipsOnComputerBoard(new[] { 2 });

        sut.Fire(new(0, 0)).Should().Be(HitResult.Water);
        sut.Fire(new(2, 3)).Should().Be(HitResult.Ship);
        sut.Fire(new(2, 4)).Should().Be(HitResult.AllSunk);
    }

    [Fact]
    public void ArrangeShipsOnComputerBoard_CannotPutSecondShip_TryAgain()
    {
        var sut = CreateSut();

        _randomWrapperMock.SetupSequence(el => el.NextPosition(new(10, 10)))
            .Returns(new BoardPosition(1, 1))
            .Returns(new BoardPosition(1, 2))
            .Returns(new BoardPosition(1, 1))
            .Returns(new BoardPosition(1, 3));
        _randomWrapperMock.Setup(el => el.NextEnum<ShipOrientation>()).Returns(ShipOrientation.Vertical);

        sut.ArrangeShipsOnComputerBoard(new[] { 1, 1 });

        sut.Fire(new(1, 1)).Should().Be(HitResult.ShipSunk);
        sut.Fire(new(1, 3)).Should().Be(HitResult.AllSunk);

        _randomWrapperMock.Verify(el => el.NextPosition(It.Is<BoardCellsSize>(el => el == new BoardCellsSize(10, 10))), Times.Exactly(4));
        _randomWrapperMock.Verify(el => el.NextEnum<ShipOrientation>(), Times.Exactly(4));
    }

    [Fact]
    public void ArrangeShipsOnComputerBoard_CannotPutSecondShip_EnsureBoardIsClearedBeforeNextTry()
    {
        var sut = CreateSut();

        sut.AddShip(new Ship(new(4, 4), ShipOrientation.Horizontal, 5));

        _randomWrapperMock.SetupSequence(el => el.NextPosition(new(10, 10)))
            .Returns(new BoardPosition(1, 1))
            .Returns(new BoardPosition(1, 2))
            .Returns(new BoardPosition(1, 5))
            .Returns(new BoardPosition(1, 7));
        _randomWrapperMock.Setup(el => el.NextEnum<ShipOrientation>()).Returns(ShipOrientation.Vertical);

        sut.ArrangeShipsOnComputerBoard(new[] { 1, 1 });

        sut.Fire(new(1, 1)).Should().Be(HitResult.Water);
    }

    [Fact]
    public void AddShip_CanAddShipNotOk_Throw()
    {
        var sut = CreateSut();

        var ship = new Ship(new(8, 8), ShipOrientation.Horizontal, 5);
        sut.CanAddShip(ship).Should().Be(CanAddShipResult.Outside);
        sut.Invoking(el => el.AddShip(ship)).Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData(0, 0, ShipOrientation.Horizontal, 10)]
    [InlineData(0, 0, ShipOrientation.Vertical, 10)]
    [InlineData(9, 0, ShipOrientation.Vertical, 10)]
    [InlineData(0, 9, ShipOrientation.Horizontal, 10)]

    [InlineData(4, 3, ShipOrientation.Horizontal, 3)]
    [InlineData(6, 3, ShipOrientation.Horizontal, 1)]
    [InlineData(3, 4, ShipOrientation.Vertical, 3)]
    [InlineData(7, 4, ShipOrientation.Vertical, 3)]

    public void AddShip_Call_ShipAdded(int x, int y, ShipOrientation shipOrientation, int length)
    {
        var sut = CreateSut();

        var ship = new Ship(new(x, y), shipOrientation, length);

        sut.AddShip(ship);

        foreach (var cell in ship.GetShipCells().Take(length - 1))
            sut.Fire(cell).Should().Be(HitResult.Ship);

        sut.Fire(ship.GetShipCells().Last()).Should().Be(HitResult.AllSunk);
    }

    [Theory]
    [InlineData(0, 0, ShipOrientation.Horizontal, 10)]
    [InlineData(0, 0, ShipOrientation.Vertical, 10)]
    [InlineData(9, 0, ShipOrientation.Vertical, 10)]
    [InlineData(0, 9, ShipOrientation.Horizontal, 10)]

    [InlineData(4, 3, ShipOrientation.Horizontal, 3)]
    [InlineData(6, 3, ShipOrientation.Horizontal, 1)]
    [InlineData(3, 4, ShipOrientation.Vertical, 3)]
    [InlineData(7, 4, ShipOrientation.Vertical, 3)]

    public void CanAddShip_Ok_Ok(int x, int y, ShipOrientation shipOrientation, int length)
    {
        var sut = CreateSut();

        sut.AddShip(new Ship(new(5, 5), ShipOrientation.Horizontal, 1));

        sut.CanAddShip(new Ship(new(x, y), shipOrientation, length)).Should().Be(CanAddShipResult.Ok);
    }

    [Theory]
    [InlineData(5, 5, ShipOrientation.Horizontal, 1)]
    [InlineData(4, 5, ShipOrientation.Horizontal, 3)]
    [InlineData(5, 4, ShipOrientation.Vertical, 3)]
    public void CanAddShip_OnAnotherShip_OnAnotherShip(int x, int y, ShipOrientation shipOrientation, int length)
    {
        var sut = CreateSut();

        sut.AddShip(new Ship(new(5, 5), ShipOrientation.Horizontal, 1));

        sut.CanAddShip(new Ship(new(x, y), shipOrientation, length)).Should().Be(CanAddShipResult.OnAnotherShip);
    }

    [Theory]
    [InlineData(-1, -1, ShipOrientation.Horizontal, 1)]
    [InlineData(0, -1, ShipOrientation.Horizontal, 1)]
    [InlineData(5, -1, ShipOrientation.Horizontal, 1)]
    [InlineData(9, -1, ShipOrientation.Horizontal, 1)]
    [InlineData(10, -1, ShipOrientation.Horizontal, 1)]

    [InlineData(-1, 0, ShipOrientation.Horizontal, 1)]
    [InlineData(10, 0, ShipOrientation.Horizontal, 1)]

    [InlineData(-1, 5, ShipOrientation.Horizontal, 1)]
    [InlineData(10, 5, ShipOrientation.Horizontal, 1)]

    [InlineData(-1, 10, ShipOrientation.Horizontal, 1)]
    [InlineData(0, 10, ShipOrientation.Horizontal, 1)]
    [InlineData(5, 10, ShipOrientation.Horizontal, 1)]
    [InlineData(9, 10, ShipOrientation.Horizontal, 1)]
    [InlineData(10, 10, ShipOrientation.Horizontal, 1)]

    [InlineData(0, 5, ShipOrientation.Horizontal, 11)]
    [InlineData(5, 0, ShipOrientation.Vertical, 11)]
    [InlineData(-1, 5, ShipOrientation.Horizontal, 11)]
    [InlineData(5, -1, ShipOrientation.Vertical, 11)]
    public void CanAddShip_Outside_Outside(int x, int y, ShipOrientation shipOrientation, int length)
    {
        var sut = CreateSut();

        sut.AddShip(new Ship(new(5, 5), ShipOrientation.Horizontal, 1));

        sut.CanAddShip(new Ship(new(x, y), shipOrientation, length)).Should().Be(CanAddShipResult.Outside);
    }

    [Theory]
    [InlineData(4, 4, ShipOrientation.Horizontal, 1)]
    [InlineData(5, 4, ShipOrientation.Horizontal, 1)]
    [InlineData(6, 4, ShipOrientation.Horizontal, 1)]

    [InlineData(4, 5, ShipOrientation.Horizontal, 1)]
    [InlineData(6, 5, ShipOrientation.Horizontal, 1)]

    [InlineData(4, 6, ShipOrientation.Horizontal, 1)]
    [InlineData(5, 6, ShipOrientation.Horizontal, 1)]
    [InlineData(6, 6, ShipOrientation.Horizontal, 1)]

    [InlineData(3, 4, ShipOrientation.Horizontal, 3)]
    [InlineData(4, 4, ShipOrientation.Horizontal, 3)]
    [InlineData(5, 4, ShipOrientation.Horizontal, 3)]
    [InlineData(6, 4, ShipOrientation.Horizontal, 3)]

    [InlineData(3, 6, ShipOrientation.Horizontal, 3)]
    [InlineData(4, 6, ShipOrientation.Horizontal, 3)]
    [InlineData(5, 6, ShipOrientation.Horizontal, 3)]
    [InlineData(6, 6, ShipOrientation.Horizontal, 3)]

    [InlineData(4, 3, ShipOrientation.Vertical, 3)]
    [InlineData(4, 4, ShipOrientation.Vertical, 3)]
    [InlineData(4, 5, ShipOrientation.Vertical, 3)]
    [InlineData(4, 6, ShipOrientation.Vertical, 3)]

    [InlineData(6, 3, ShipOrientation.Vertical, 3)]
    [InlineData(6, 4, ShipOrientation.Vertical, 3)]
    [InlineData(6, 5, ShipOrientation.Vertical, 3)]
    [InlineData(6, 6, ShipOrientation.Vertical, 3)]
    public void CanAddShip_NextToAnother_NextToAnother(int x, int y, ShipOrientation shipOrientation, int length)
    {
        var sut = CreateSut();

        sut.AddShip(new Ship(new(5, 5), ShipOrientation.Horizontal, 1));

        sut.CanAddShip(new Ship(new(x, y), shipOrientation, length)).Should().Be(CanAddShipResult.NextToAnother);
    }
}
