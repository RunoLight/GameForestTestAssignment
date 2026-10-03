using GameForestTestAssignment.Game.Animations;
using GameForestTestAssignment.Game.MatchDetection;
using GameForestTestAssignment.Game.PlayerInput;
using Microsoft.Xna.Framework;

namespace GameForestTestAssignment.Game.GameLogic;

public enum GameState
{
    Idle,
    Swapping,
    SwapBack,
    Resolving,
    Falling
}

public class GameStateMachine(
    Board board,
    MatchDetector matchDetector,
    AnimationManager animationManager,
    GravityService gravityService
)
{
    private SwapCommand? _currentSwap;

    public GameState State { get; private set; } = GameState.Idle;
    public bool CanAcceptInput => State == GameState.Idle;

    public void RequestSwap(SwapCommand command)
    {
        if (State != GameState.Idle)
            return;

        _currentSwap = command;
        BeginSwap(command, false);
    }

    // Returns (swapComplete, plan) - plan is null for swap-back, non-null for valid match
    public (bool swapComplete, ResolutionPlan plan ) UpdateSwap()
    {
        if (State is not (GameState.Swapping or GameState.SwapBack))
            return (false, null);

        if (animationManager.IsPlaying<SwapAnimation>())
            return (false, null);

        if (State == GameState.SwapBack)
        {
            _currentSwap = null;
            State = GameState.Idle;
            return (true, null);
        }

        if (_currentSwap == null)
        {
            State = GameState.Idle;
            return (true, null);
        }

        var plan = MatchResolver.Build(
            board, matchDetector, (_currentSwap.Value.EndX, _currentSwap.Value.EndY)
        );
        if (!plan.Involves(_currentSwap.Value.StartX, _currentSwap.Value.StartY) &&
            !plan.Involves(_currentSwap.Value.EndX, _currentSwap.Value.EndY))
        {
            BeginSwap(_currentSwap.Value, true);
            return (false, null);
        }

        State = GameState.Resolving;
        return (true, plan);
    }

    public void ApplyGravity()
    {
        gravityService.Apply();
        State = GameState.Falling;
        _currentSwap = null;
    }

    // Returns (fallComplete, plan) - plan is null if no new matches, non-null if cascade
    public (bool fallComplete, ResolutionPlan plan) UpdateFalling()
    {
        if (State != GameState.Falling)
            return (false, null);

        if (animationManager.IsPlaying<FallAnimation>())
            return (false, null);

        var plan = MatchResolver.Build(board, matchDetector, null);
        if (plan.IsEmpty)
        {
            State = GameState.Idle;
            return (true, null);
        }

        State = GameState.Resolving;
        return (true, plan);
    }

    private void BeginSwap(SwapCommand command, bool swapBack)
    {
        board.SwapCells(command.StartX, command.StartY, command.EndX, command.EndY);

        const float animDuration = 220f;
        var offset = new Vector2(command.EndX - command.StartX, command.EndY - command.StartY) * board.CellSize;
        animationManager.Play(new SwapAnimation(command.StartX, command.StartY, offset, animDuration));
        animationManager.Play(new SwapAnimation(command.EndX, command.EndY, -offset, animDuration));

        State = swapBack ? GameState.SwapBack : GameState.Swapping;
    }
}
