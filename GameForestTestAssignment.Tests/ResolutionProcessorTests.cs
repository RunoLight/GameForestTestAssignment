using GameForestTestAssignment.Game;
using GameForestTestAssignment.Game.Animations;
using GameForestTestAssignment.Game.Bonuses;
using GameForestTestAssignment.Game.Effects;
using GameForestTestAssignment.Game.GameLogic;
using GameForestTestAssignment.Game.MatchDetection;
using Microsoft.Xna.Framework;

namespace GameForestTestAssignment.Tests;

public class ResolutionProcessorTests
{
    private const float FrameTime = 1f / 60f;
    private const int CellReward = 10;

    private Board _board = null!;
    private AnimationManager _animations = null!;
    private ScoreManager _score = null!;
    private ResolutionProcessor _processor = null!;

    [SetUp]
    public void SetUp()
    {
        _board = TestBoard.Deadlocked();
        _animations = new AnimationManager();
        _score = new ScoreManager();

        var layout = new BoardLayout(64, Vector2.Zero);
        var effects = new BoardEffectQueue();
        var particles = new ParticlePool();
        var removal = new RemovalService(_board, layout, _animations, _score, particles, new ScorePopups(), effects);
        var bombs = new BombService(layout, particles, new ScreenShake(), effects);
        var destroyers = new DestroyerService(_board, effects);
        _processor = new ResolutionProcessor(_board, _animations, effects, new BonusManager(effects), removal, bombs,
            destroyers);
    }

    [Test]
    public void RemovedCells_AreClearedAfterAnimationAndScored()
    {
        _processor.Start(Plan(remove: [(0, 0), (1, 0), (2, 0)]));

        Assert.That(_board[0, 0].IsEmpty, Is.False, "Cell must stay visible while it disappears.");

        RunUntilFinished();

        Assert.That(EmptyCells(), Is.EquivalentTo(new[] { (0, 0), (1, 0), (2, 0) }));
        Assert.That(_score.Score, Is.EqualTo(3 * CellReward));
    }

    [Test]
    public void SpawnedBonus_StaysOnBoard()
    {
        var bomb = new BombBonus(_board[1, 1].CellType);

        _processor.Start(Plan(remove: [(0, 1), (2, 1)], spawn: new() { [(1, 1)] = bomb }));
        RunUntilFinished();

        Assert.That(_board[1, 1].Bonus, Is.SameAs(bomb));
        Assert.That(_board[1, 1].IsEmpty, Is.False);
        Assert.That(EmptyCells(), Is.EquivalentTo(new[] { (0, 1), (2, 1) }));
    }

    [Test]
    public void HorizontalLine_ClearsWholeRow()
    {
        PlaceBonus(2, 4, Orientation.Horizontal);

        _processor.Start(Plan(activate: [(2, 4)]));
        RunUntilFinished();

        Assert.That(EmptyCells(), Is.EquivalentTo(Enumerable.Range(0, Board.Width).Select(x => (x, 4))));
        Assert.That(_score.Score, Is.EqualTo(Board.Width * CellReward));
    }

    [Test]
    public void VerticalLine_ClearsWholeColumn()
    {
        PlaceBonus(5, 0, Orientation.Vertical);

        _processor.Start(Plan(activate: [(5, 0)]));
        RunUntilFinished();

        Assert.That(EmptyCells(), Is.EquivalentTo(Enumerable.Range(0, Board.Height).Select(y => (5, y))));
    }

    [Test]
    public void Bomb_ExplodesAfterDelayAndClearsThreeByThree()
    {
        _board[3, 3].Bonus = new BombBonus(_board[3, 3].CellType);

        _processor.Start(Plan(activate: [(3, 3)]));
        Advance(0.2f);
        Assert.That(_score.Score, Is.EqualTo(CellReward), "Only the bomb itself before the delay.");

        Advance(0.1f);
        Assert.That(_score.Score, Is.EqualTo(9 * CellReward));

        RunUntilFinished();
        var expected = from x in Enumerable.Range(2, 3) from y in Enumerable.Range(2, 3) select (x, y);
        Assert.That(EmptyCells(), Is.EquivalentTo(expected));
    }

    [Test]
    public void BombInCorner_IgnoresCellsOutsideBoard()
    {
        _board[0, 0].Bonus = new BombBonus(_board[0, 0].CellType);

        _processor.Start(Plan(activate: [(0, 0)]));
        RunUntilFinished();

        Assert.That(EmptyCells(), Is.EquivalentTo(new[] { (0, 0), (1, 0), (0, 1), (1, 1) }));
    }

    [Test]
    public void DestroyerHittingBomb_TriggersChainReaction()
    {
        PlaceBonus(0, 4, Orientation.Horizontal);
        _board[6, 4].Bonus = new BombBonus(_board[6, 4].CellType);

        _processor.Start(Plan(activate: [(0, 4)]));
        RunUntilFinished();

        var row = Enumerable.Range(0, Board.Width).Select(x => (x, 4));
        var blast = from x in Enumerable.Range(5, 3) from y in new[] { 3, 5 } select (x, y);
        Assert.That(EmptyCells(), Is.EquivalentTo(row.Concat(blast)));
        Assert.That(_score.Score, Is.EqualTo(14 * CellReward));
    }

    [Test]
    public void DestroyerHittingAnotherLine_ActivatesItOnce()
    {
        PlaceBonus(0, 2, Orientation.Horizontal);
        PlaceBonus(4, 2, Orientation.Vertical);

        _processor.Start(Plan(activate: [(0, 2)]));
        RunUntilFinished();

        var row = Enumerable.Range(0, Board.Width).Select(x => (x, 2));
        var column = Enumerable.Range(0, Board.Height).Where(y => y != 2).Select(y => (4, y));
        Assert.That(EmptyCells(), Is.EquivalentTo(row.Concat(column)));
        Assert.That(_score.Score, Is.EqualTo(15 * CellReward));
    }

    private void PlaceBonus(int x, int y, Orientation orientation)
    {
        _board[x, y].Bonus = new LineBonus(_board[x, y].CellType, orientation);
    }

    private static ResolutionPlan Plan(
        List<(int X, int Y)>? remove = null,
        Dictionary<(int X, int Y), Bonus>? spawn = null,
        List<(int X, int Y)>? activate = null)
    {
        return new ResolutionPlan([..remove ?? []], spawn ?? [], activate ?? []);
    }

    private void Advance(float seconds)
    {
        _processor.Update(seconds);
        _animations.Update(seconds);
    }

    private void RunUntilFinished()
    {
        for (var frame = 0; frame < 600 && !_processor.IsFinished; frame++)
            Advance(FrameTime);

        Assert.That(_processor.IsFinished, Is.True, "Resolution did not finish in 10 seconds.");
    }

    private List<(int X, int Y)> EmptyCells()
    {
        return TestBoard.AllPositions().Where(p => _board[p.X, p.Y].IsEmpty).ToList();
    }
}
