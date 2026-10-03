using GameForestTestAssignment.Game;
using GameForestTestAssignment.Game.Animations;
using GameForestTestAssignment.Game.GameLogic;
using GameForestTestAssignment.Game.MatchDetection;
using GameForestTestAssignment.Game.PlayerInput;

namespace GameForestTestAssignment.Tests;

public class GameStateMachineTests
{
    private Board _board = null!;
    private AnimationManager _animations = null!;
    private GameStateMachine _stateMachine = null!;

    private void Create(Board board)
    {
        _board = board;
        _animations = new AnimationManager();
        _stateMachine = new GameStateMachine(board, new MatchDetector(), _animations,
            new GravityService(board, _animations));
    }

    private void FinishAnimations()
    {
        _animations.Update(10f);
    }

    [Test]
    public void SwapCreatingMatch_ReturnsPlanAfterAnimation()
    {
        Create(TestBoard.FromRows("RRGR"));

        _stateMachine.RequestSwap(new SwapCommand(2, 0, 3, 0));

        Assert.That(_stateMachine.State, Is.EqualTo(GameState.Swapping));
        Assert.That(_stateMachine.UpdateSwap(), Is.Null, "No plan while the swap is animating.");

        FinishAnimations();
        var plan = _stateMachine.UpdateSwap();

        Assert.That(plan, Is.Not.Null);
        Assert.That(plan!.CellsToRemove, Is.EquivalentTo(new[] { (0, 0), (1, 0), (2, 0) }));
        Assert.That(_stateMachine.State, Is.EqualTo(GameState.Resolving));
    }

    [Test]
    public void SwapWithoutMatch_SwapsBackAndReturnsToIdle()
    {
        Create(TestBoard.FromRows("RG"));

        _stateMachine.RequestSwap(new SwapCommand(0, 0, 1, 0));
        FinishAnimations();

        Assert.That(_stateMachine.UpdateSwap(), Is.Null);
        Assert.That(_stateMachine.State, Is.EqualTo(GameState.SwapBack));
        Assert.That(_board[0, 0].CellType, Is.EqualTo(CellType.Circle));

        FinishAnimations();
        _stateMachine.UpdateSwap();

        Assert.That(_stateMachine.State, Is.EqualTo(GameState.Idle));
        Assert.That(_stateMachine.CanAcceptInput, Is.True);
    }

    [Test]
    public void SwapRequest_IsIgnoredWhileBusy()
    {
        Create(TestBoard.FromRows("RRGR", "BY"));
        _stateMachine.RequestSwap(new SwapCommand(2, 0, 3, 0));

        _stateMachine.RequestSwap(new SwapCommand(0, 1, 1, 1));

        Assert.That(_stateMachine.CanAcceptInput, Is.False);
        Assert.That(_board[0, 1].CellType, Is.EqualTo(CellType.Triangle));
    }

    [Test]
    public void FallingIntoMatch_ReturnsCascadePlan()
    {
        var board = TestBoard.Deadlocked();
        board[1, 0].CellType = CellType.Circle;
        board[2, 0].CellType = CellType.Circle;
        Create(board);

        _stateMachine.ApplyGravity();
        var plan = _stateMachine.UpdateFalling();

        Assert.That(plan, Is.Not.Null);
        Assert.That(plan!.CellsToRemove, Is.EquivalentTo(new[] { (0, 0), (1, 0), (2, 0) }));
        Assert.That(_stateMachine.State, Is.EqualTo(GameState.Resolving));
    }

    [Test]
    public void BoardWithoutMoves_IsShuffledBeforeReturningToIdle()
    {
        Create(TestBoard.Deadlocked());

        _stateMachine.ApplyGravity();
        Assert.That(_stateMachine.UpdateFalling(), Is.Null);

        Assert.That(_stateMachine.State, Is.EqualTo(GameState.Falling));
        Assert.That(_animations.IsPlaying<FallAnimation>(), Is.True, "Shuffled pieces drop in.");

        FinishAnimations();
        _stateMachine.UpdateFalling();

        Assert.That(_stateMachine.State, Is.EqualTo(GameState.Idle));
        Assert.That(MoveFinder.HasPossibleMove(_board), Is.True);
    }
}
