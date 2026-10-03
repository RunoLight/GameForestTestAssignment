using GameForestTestAssignment.Game;
using GameForestTestAssignment.Game.Animations;
using GameForestTestAssignment.Game.Bonuses;
using GameForestTestAssignment.Game.GameLogic;

namespace GameForestTestAssignment.Tests;

public class GravityServiceTests
{
    [Test]
    public void Apply_DropsPiecesKeepingOrderAndFillsBoard()
    {
        var board = TestBoard.FromRows("R", ".", "G");
        var bonus = new BombBonus(CellType.Circle);
        board[0, 0].Bonus = bonus;
        var animations = new AnimationManager();

        new GravityService(board, animations).Apply();

        Assert.That(board[0, 7].CellType, Is.EqualTo(CellType.Square));
        Assert.That(board[0, 6].CellType, Is.EqualTo(CellType.Circle));
        Assert.That(board[0, 6].Bonus, Is.SameAs(bonus));
        Assert.That(TestBoard.AllPositions().Any(p => board[p.X, p.Y].IsEmpty), Is.False);
        Assert.That(animations.IsPlaying<FallAnimation>(), Is.True);
    }

    [Test]
    public void Apply_OnFullBoard_ChangesNothing()
    {
        var board = TestBoard.Deadlocked();
        var before = TestBoard.AllPositions().Select(p => board[p.X, p.Y].CellType).ToArray();
        var animations = new AnimationManager();

        new GravityService(board, animations).Apply();

        var after = TestBoard.AllPositions().Select(p => board[p.X, p.Y].CellType).ToArray();
        Assert.That(after, Is.EqualTo(before));
        Assert.That(animations.IsPlaying<FallAnimation>(), Is.False);
    }
}
