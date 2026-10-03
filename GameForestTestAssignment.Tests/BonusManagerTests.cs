using GameForestTestAssignment.Game;
using GameForestTestAssignment.Game.Bonuses;
using GameForestTestAssignment.Game.GameLogic;

namespace GameForestTestAssignment.Tests;

public class BonusManagerTests
{
    private BoardEffectQueue _effects = null!;
    private BonusManager _bonusManager = null!;

    [SetUp]
    public void SetUp()
    {
        _effects = new BoardEffectQueue();
        _bonusManager = new BonusManager(_effects);
    }

    [Test]
    public void HorizontalLine_RemovesItselfAndSpawnsDestroyersLeftAndRight()
    {
        _bonusManager.Activate(new LineBonus(CellType.Star, Orientation.Horizontal), 3, 4);

        Assert.That(Drain(), Is.EqualTo(new BoardEffect[]
        {
            new RemoveCellEffect(3, 4),
            new SpawnDestroyerEffect(3, 4, -1, 0, CellType.Star),
            new SpawnDestroyerEffect(3, 4, 1, 0, CellType.Star)
        }));
    }

    [Test]
    public void VerticalLine_SpawnsDestroyersUpAndDown()
    {
        _bonusManager.Activate(new LineBonus(CellType.Star, Orientation.Vertical), 3, 4);

        var destroyers = Drain().OfType<SpawnDestroyerEffect>().Select(e => (e.Dx, e.Dy));
        Assert.That(destroyers, Is.EquivalentTo(new[] { (0, -1), (0, 1) }));
    }

    [Test]
    public void Bomb_RemovesItselfAndSchedulesExplosion()
    {
        _bonusManager.Activate(new BombBonus(CellType.Circle), 2, 2);

        Assert.That(Drain(), Is.EqualTo(new BoardEffect[]
        {
            new RemoveCellEffect(2, 2),
            new ScheduleBombExplosionEffect(2, 2)
        }));
    }

    [Test]
    public void SameCell_IsActivatedOncePerResolution()
    {
        var bomb = new BombBonus(CellType.Circle);

        _bonusManager.Activate(bomb, 2, 2);
        Drain();
        _bonusManager.Activate(bomb, 2, 2);

        Assert.That(_effects.IsEmpty, Is.True);
    }

    [Test]
    public void Clear_AllowsActivatingCellAgain()
    {
        var bomb = new BombBonus(CellType.Circle);
        _bonusManager.Activate(bomb, 2, 2);
        Drain();

        _bonusManager.Clear();
        _bonusManager.Activate(bomb, 2, 2);

        Assert.That(Drain(), Is.Not.Empty);
    }

    private List<BoardEffect> Drain()
    {
        var result = new List<BoardEffect>();
        while (_effects.TryDequeue(out var effect))
            result.Add(effect);
        return result;
    }
}
