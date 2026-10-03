using GameForestTestAssignment.Game;
using GameForestTestAssignment.Game.Bonuses;
using GameForestTestAssignment.Game.MatchDetection;

namespace GameForestTestAssignment.Tests;

public class BoardTests
{
    [Test]
    public void SwapCells_SwapsTypesAndBonuses()
    {
        var board = TestBoard.FromRows("RG");
        var bonus = new BombBonus(CellType.Circle);
        board[0, 0].Bonus = bonus;

        board.SwapCells(0, 0, 1, 0);

        Assert.That(board[0, 0].CellType, Is.EqualTo(CellType.Square));
        Assert.That(board[0, 0].Bonus, Is.Null);
        Assert.That(board[1, 0].CellType, Is.EqualTo(CellType.Circle));
        Assert.That(board[1, 0].Bonus, Is.SameAs(bonus));
    }

    [Test]
    public void MoveCell_MovesContentAndEmptiesSource()
    {
        var board = TestBoard.FromRows("R");
        var bonus = new BombBonus(CellType.Circle);
        board[0, 0].Bonus = bonus;

        board.MoveCell(0, 0, 0, 5);

        Assert.That(board[0, 0].IsEmpty, Is.True);
        Assert.That(board[0, 0].Bonus, Is.Null);
        Assert.That(board[0, 5].CellType, Is.EqualTo(CellType.Circle));
        Assert.That(board[0, 5].Bonus, Is.SameAs(bonus));
    }

    [TestCase(0, 0, true)]
    [TestCase(7, 7, true)]
    [TestCase(-1, 0, false)]
    [TestCase(0, 8, false)]
    public void IsInBounds_ChecksBoardEdges(int x, int y, bool expected)
    {
        Assert.That(new Board().IsInBounds(x, y), Is.EqualTo(expected));
    }

    [Test, Repeat(20)]
    public void GenerateRandomBoard_IsFullPlayableAndHasNoMatches()
    {
        var board = new Board();

        board.GenerateRandomBoard();

        Assert.That(TestBoard.AllPositions().Any(p => board[p.X, p.Y].IsEmpty), Is.False);
        Assert.That(MoveFinder.HasAnyMatch(board), Is.False);
        Assert.That(MoveFinder.HasPossibleMove(board), Is.True);
    }

    [Test, Repeat(200)]
    public void Shuffle_KeepsPiecesAndBonuses()
    {
        var board = TestBoard.Deadlocked();
        var bonus = new LineBonus(board[0, 0].CellType, Orientation.Horizontal);
        board[0, 0].Bonus = bonus;
        var piecesBefore = Pieces(board);

        board.Shuffle();

        Assert.That(Pieces(board), Is.EquivalentTo(piecesBefore));
        var bonusCell = TestBoard.AllPositions().Single(p => board[p.X, p.Y].Bonus != null);
        Assert.That(board[bonusCell.X, bonusCell.Y].Bonus, Is.SameAs(bonus));
        Assert.That(board[bonusCell.X, bonusCell.Y].CellType, Is.EqualTo(bonus.ColorType));
    }

    [Test, Repeat(20)]
    public void Shuffle_ProducesPlayableBoardWithoutMatches()
    {
        var board = TestBoard.Deadlocked();

        board.Shuffle();

        Assert.That(MoveFinder.HasAnyMatch(board), Is.False);
        Assert.That(MoveFinder.HasPossibleMove(board), Is.True);
    }

    private static List<CellType> Pieces(Board board)
    {
        return TestBoard.AllPositions().Select(p => board[p.X, p.Y].CellType).ToList();
    }
}
