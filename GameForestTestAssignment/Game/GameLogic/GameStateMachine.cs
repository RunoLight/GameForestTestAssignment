#region

using System.Collections.Generic;
using GameForestTestAssignment.Game.Animations;
using GameForestTestAssignment.Game.MatchDetection;
using GameForestTestAssignment.Game.PlayerInput;
using Microsoft.Xna.Framework;

#endregion

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
    AnimationManager animationManager
)
{
    private SwapCommand? _currentSwap;

    public GameState State { get; private set; } = GameState.Idle;
    public bool CanAcceptInput => State == GameState.Idle;

    public Dictionary<(int X, int Y), SwapAnimation> SwapAnimationsDict { get; } = new();
    public Dictionary<(int X, int Y), FallAnimation> FallAnimationsDict { get; } = new();

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

        var allDone = true;
        foreach (var anim in SwapAnimationsDict.Values)
            if (!anim.IsComplete)
                allDone = false;

        if (!allDone)
            return (false, null);

        SwapAnimationsDict.Clear();

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

    public void ApplyGravity(GravityService gravityService)
    {
        gravityService.Apply(FallAnimationsDict);
        State = GameState.Falling;
        _currentSwap = null;
    }

    // Returns (fallComplete, plan) - plan is null if no new matches, non-null if cascade
    public (bool fallComplete, ResolutionPlan plan) UpdateFalling()
    {
        if (State != GameState.Falling)
            return (false, null);

        var allDone = true;
        foreach (var anim in FallAnimationsDict.Values)
            if (!anim.IsComplete)
                allDone = false;

        if (!allDone) return (false, null);

        FallAnimationsDict.Clear();
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

        var startScreen = GetCellScreenPos(command.StartX, command.StartY);
        var endScreen = GetCellScreenPos(command.EndX, command.EndY);

        const float animDuration = 220f;
        var animAtStart = new SwapAnimation(endScreen, startScreen, animDuration);
        var animAtEnd = new SwapAnimation(startScreen, endScreen, animDuration);

        animationManager.AddAnimation(animAtStart);
        animationManager.AddAnimation(animAtEnd);

        SwapAnimationsDict.Clear();
        SwapAnimationsDict.Add((command.StartX, command.StartY), animAtStart);
        SwapAnimationsDict.Add((command.EndX, command.EndY), animAtEnd);

        State = swapBack ? GameState.SwapBack : GameState.Swapping;
    }

    private Vector2 GetCellScreenPos(int x, int y)
    {
        return board.BoardPosition + new Vector2(x * board.CellSize, y * board.CellSize);
    }
}