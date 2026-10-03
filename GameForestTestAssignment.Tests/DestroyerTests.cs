using GameForestTestAssignment.Game;
using GameForestTestAssignment.Game.Effects;

namespace GameForestTestAssignment.Tests;

public class DestroyerTests
{
    [Test]
    public void FliesToEdge_HittingEveryCellExceptStart()
    {
        var hits = new List<(int, int)>();
        var destroyer = new Destroyer(3, 3, 1, 0, CellType.Circle);

        for (var i = 0; i < 60 && destroyer.IsAlive; i++)
            destroyer.Update(1f / 60f, new Board(), (x, y) => hits.Add((x, y)));

        Assert.That(hits, Is.EqualTo(new[] { (4, 3), (5, 3), (6, 3), (7, 3) }));
        Assert.That(destroyer.IsAlive, Is.False);
    }

    [Test]
    public void LongFrame_DoesNotSkipCells()
    {
        var hits = new List<(int, int)>();
        var destroyer = new Destroyer(2, 7, 0, -1, CellType.Circle);

        destroyer.Update(1f, new Board(), (x, y) => hits.Add((x, y)));

        Assert.That(hits, Is.EqualTo(Enumerable.Range(0, 7).Reverse().Select(y => (2, y))));
        Assert.That(destroyer.IsAlive, Is.False);
    }
}
