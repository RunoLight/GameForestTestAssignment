using GameForestTestAssignment.Game.MatchDetection;

namespace GameForestTestAssignment.Tests;

public class MoveFinderTests
{
    [Test]
    public void DeadlockedBoard_HasNoMatchesAndNoMoves()
    {
        var board = TestBoard.Deadlocked();

        Assert.That(MoveFinder.HasAnyMatch(board), Is.False);
        Assert.That(MoveFinder.HasPossibleMove(board), Is.False);
    }

    [TestCase("RRGR")]
    [TestCase("RGRR")]
    public void HorizontalSwapCompletingRow_IsPossibleMove(string row)
    {
        Assert.That(MoveFinder.HasPossibleMove(TestBoard.FromRows(row)), Is.True);
    }

    [Test]
    public void VerticalSwapCompletingRow_IsPossibleMove()
    {
        var board = TestBoard.FromRows(
            "RR.",
            "..R");

        Assert.That(MoveFinder.HasPossibleMove(board), Is.True);
    }

    [Test]
    public void HasPossibleMove_LeavesBoardUnchanged()
    {
        var board = TestBoard.Deadlocked();
        var before = TestBoard.AllPositions().Select(p => board[p.X, p.Y].CellType).ToArray();

        MoveFinder.HasPossibleMove(board);

        var after = TestBoard.AllPositions().Select(p => board[p.X, p.Y].CellType).ToArray();
        Assert.That(after, Is.EqualTo(before));
    }

    [Test]
    public void ExistingRun_IsDetectedAsMatch()
    {
        Assert.That(MoveFinder.HasAnyMatch(TestBoard.FromRows("B", "B", "B")), Is.True);
    }
}
