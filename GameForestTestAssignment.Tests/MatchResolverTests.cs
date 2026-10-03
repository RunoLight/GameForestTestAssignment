using GameForestTestAssignment.Game;
using GameForestTestAssignment.Game.Bonuses;
using GameForestTestAssignment.Game.MatchDetection;

namespace GameForestTestAssignment.Tests;

public class MatchResolverTests
{
    private static ResolutionPlan Build(Board board, (int X, int Y)? lastMoved = null)
    {
        return MatchResolver.Build(board, new MatchDetector(), lastMoved);
    }

    [Test]
    public void NoMatches_GivesEmptyPlan()
    {
        var plan = Build(TestBoard.Deadlocked());

        Assert.That(plan.IsEmpty, Is.True);
    }

    [Test]
    public void ThreeInRow_RemovesCellsWithoutBonus()
    {
        var plan = Build(TestBoard.FromRows("RRR"));

        Assert.That(plan.CellsToRemove, Is.EquivalentTo(new[] { (0, 0), (1, 0), (2, 0) }));
        Assert.That(plan.BonusesToSpawn, Is.Empty);
        Assert.That(plan.BonusesToActivate, Is.Empty);
    }

    [Test]
    public void FourInRow_SpawnsHorizontalLineAtLastMovedCell()
    {
        var plan = Build(TestBoard.FromRows("RRRR"), lastMoved: (1, 0));

        Assert.That(plan.BonusesToSpawn.Keys, Is.EquivalentTo(new[] { (1, 0) }));
        var bonus = plan.BonusesToSpawn[(1, 0)];
        Assert.That(bonus, Is.TypeOf<LineBonus>());
        Assert.That(((LineBonus)bonus).Orientation, Is.EqualTo(Orientation.Horizontal));
        Assert.That(bonus.ColorType, Is.EqualTo(CellType.Circle));
        Assert.That(plan.CellsToRemove, Is.EquivalentTo(new[] { (0, 0), (2, 0), (3, 0) }));
    }

    [Test]
    public void FourInColumn_SpawnsVerticalLineInMiddleWhenNothingMoved()
    {
        var plan = Build(TestBoard.FromRows("G", "G", "G", "G"));

        Assert.That(plan.BonusesToSpawn.Keys, Is.EquivalentTo(new[] { (0, 2) }));
        Assert.That(((LineBonus)plan.BonusesToSpawn[(0, 2)]).Orientation, Is.EqualTo(Orientation.Vertical));
    }

    [Test]
    public void LastMovedOutsideMatch_FallsBackToMiddle()
    {
        var plan = Build(TestBoard.FromRows("RRRR"), lastMoved: (5, 5));

        Assert.That(plan.BonusesToSpawn.Keys, Is.EquivalentTo(new[] { (2, 0) }));
    }

    [Test]
    public void FiveInRow_SpawnsBomb()
    {
        var plan = Build(TestBoard.FromRows("RRRRR"), lastMoved: (4, 0));

        Assert.That(plan.BonusesToSpawn[(4, 0)], Is.TypeOf<BombBonus>());
        Assert.That(plan.CellsToRemove, Has.Count.EqualTo(4));
    }

    [Test]
    public void LShape_SpawnsBombAtIntersection()
    {
        var board = TestBoard.FromRows(
            "RRR",
            "R..",
            "R..");

        var plan = Build(board);

        Assert.That(plan.BonusesToSpawn.Keys, Is.EquivalentTo(new[] { (0, 0) }));
        Assert.That(plan.BonusesToSpawn[(0, 0)], Is.TypeOf<BombBonus>());
        Assert.That(plan.CellsToRemove, Is.EquivalentTo(new[] { (1, 0), (2, 0), (0, 1), (0, 2) }));
    }

    [Test]
    public void ExistingBonusInMatch_IsActivatedAndRemoved()
    {
        var board = TestBoard.FromRows("RRR");
        board[1, 0].Bonus = new BombBonus(CellType.Circle);

        var plan = Build(board);

        Assert.That(plan.BonusesToActivate, Is.EquivalentTo(new[] { (1, 0) }));
        Assert.That(plan.CellsToRemove, Does.Contain((1, 0)));
    }

    [Test]
    public void NewBonus_IsNotPlacedOnCellThatAlreadyHasOne()
    {
        var board = TestBoard.FromRows("RRRR");
        board[1, 0].Bonus = new BombBonus(CellType.Circle);

        var plan = Build(board, lastMoved: (1, 0));

        Assert.That(plan.BonusesToSpawn.ContainsKey((1, 0)), Is.False);
    }
}
