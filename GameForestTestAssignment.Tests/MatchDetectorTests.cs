using GameForestTestAssignment.Game;
using GameForestTestAssignment.Game.MatchDetection;

namespace GameForestTestAssignment.Tests;

public class MatchDetectorTests
{
    private readonly MatchDetector _detector = new();

    [Test]
    public void EmptyBoard_HasNoMatches()
    {
        var (matches, intersections) = _detector.FindMatches(new Board());

        Assert.That(matches, Is.Empty);
        Assert.That(intersections, Is.Empty);
    }

    [Test]
    public void ThreeInRow_IsHorizontalMatch()
    {
        var board = TestBoard.FromRows("GRRRG");

        var (matches, _) = _detector.FindMatches(board);

        Assert.That(matches, Has.Count.EqualTo(1));
        Assert.That(matches[0].Orientation, Is.EqualTo(Orientation.Horizontal));
        Assert.That(matches[0].Type, Is.EqualTo(CellType.Circle));
        Assert.That(matches[0].Cells, Is.EqualTo(new[] { (1, 0), (2, 0), (3, 0) }));
    }

    [Test]
    public void ThreeInColumn_IsVerticalMatch()
    {
        var board = TestBoard.FromRows("B", "B", "B");

        var (matches, _) = _detector.FindMatches(board);

        Assert.That(matches, Has.Count.EqualTo(1));
        Assert.That(matches[0].Orientation, Is.EqualTo(Orientation.Vertical));
        Assert.That(matches[0].Cells, Is.EqualTo(new[] { (0, 0), (0, 1), (0, 2) }));
    }

    [TestCase("RR")]
    [TestCase("RRGR")]
    [TestCase("RR.R")]
    public void BrokenRun_IsNotMatch(string row)
    {
        var (matches, _) = _detector.FindMatches(TestBoard.FromRows(row));

        Assert.That(matches, Is.Empty);
    }

    [Test]
    public void RunOfFive_IsSingleMatch()
    {
        var (matches, _) = _detector.FindMatches(TestBoard.FromRows("RRRRR"));

        Assert.That(matches, Has.Count.EqualTo(1));
        Assert.That(matches[0].Length, Is.EqualTo(5));
    }

    [Test]
    public void AdjacentRunsOfDifferentTypes_AreSeparateMatches()
    {
        var (matches, _) = _detector.FindMatches(TestBoard.FromRows("RRRGGG"));

        Assert.That(matches.Select(m => m.Type), Is.EquivalentTo(new[] { CellType.Circle, CellType.Square }));
    }

    [Test]
    public void LShape_ReportsCornerAsIntersection()
    {
        var board = TestBoard.FromRows(
            "RRR",
            "R..",
            "R..");

        var (matches, intersections) = _detector.FindMatches(board);

        Assert.That(matches, Has.Count.EqualTo(2));
        Assert.That(intersections, Is.EquivalentTo(new[] { (0, 0) }));
    }
}
