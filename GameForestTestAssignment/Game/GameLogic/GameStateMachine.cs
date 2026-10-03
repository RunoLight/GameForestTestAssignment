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
    IBoard board,
    MatchDetector matchDetector,
    AnimationManager animationManager,
    GravityService gravityService
)
{
    private SwapCommand _currentSwap;

    public GameState State { get; private set; } = GameState.Idle;
    public bool CanAcceptInput => State == GameState.Idle;

    public void RequestSwap(SwapCommand command)
    {
        if (State != GameState.Idle)
            return;

        _currentSwap = command;
        BeginSwap(command, false);
    }

    /// <returns> A plan once the swap animation ends with a match; null otherwise. </returns>
    public ResolutionPlan UpdateSwap()
    {
        if (State is not (GameState.Swapping or GameState.SwapBack) ||
            animationManager.IsPlaying<SwapAnimation>())
            return null;

        if (State == GameState.SwapBack)
        {
            State = GameState.Idle;
            return null;
        }

        var plan = MatchResolver.Build(board, matchDetector, (_currentSwap.EndX, _currentSwap.EndY));
        if (!plan.Involves(_currentSwap.StartX, _currentSwap.StartY) &&
            !plan.Involves(_currentSwap.EndX, _currentSwap.EndY))
        {
            BeginSwap(_currentSwap, true);
            return null;
        }

        State = GameState.Resolving;
        return plan;
    }

    public void ApplyGravity()
    {
        gravityService.Apply();
        State = GameState.Falling;
    }

    /// <returns> Returns a plan once the fall animation ends with a cascade match; null otherwise. </returns>
    public ResolutionPlan UpdateFalling()
    {
        if (State != GameState.Falling || animationManager.IsPlaying<FallAnimation>())
            return null;

        var plan = MatchResolver.Build(board, matchDetector, null);
        if (plan.IsEmpty)
        {
            State = GameState.Idle;
            return null;
        }

        State = GameState.Resolving;
        return plan;
    }

    private void BeginSwap(SwapCommand command, bool swapBack)
    {
        board.SwapCells(command.StartX, command.StartY, command.EndX, command.EndY);

        const float animDuration = 0.22f;
        var offset = new Vector2(command.EndX - command.StartX, command.EndY - command.StartY);
        animationManager.Play(new SwapAnimation(command.StartX, command.StartY, offset, animDuration));
        animationManager.Play(new SwapAnimation(command.EndX, command.EndY, -offset, animDuration));

        State = swapBack ? GameState.SwapBack : GameState.Swapping;
    }
}