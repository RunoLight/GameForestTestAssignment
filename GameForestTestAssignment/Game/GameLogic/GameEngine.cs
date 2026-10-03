using System;
using GameForestTestAssignment.Game.Animations;
using GameForestTestAssignment.Game.MatchDetection;
using GameForestTestAssignment.Game.PlayerInput;

namespace GameForestTestAssignment.Game.GameLogic;

public class GameEngine
{
    private readonly AnimationManager _animationManager;
    private readonly InputHandler _inputHandler;
    private readonly ResolutionProcessor _resolution;
    private readonly GameStateMachine _stateMachine;

    public GameEngine(GameStateMachine stateMachine, InputHandler inputHandler,
        AnimationManager animationManager, ResolutionProcessor resolution)
    {
        _stateMachine = stateMachine;
        _inputHandler = inputHandler;
        _animationManager = animationManager;
        _resolution = resolution;

        _inputHandler.SwapRequested += OnSwapRequested;
    }

    public void Update(float deltaTime)
    {
        _inputHandler.Enabled = _stateMachine.CanAcceptInput;
        _inputHandler.Update();

        ResolutionPlan plan = null;
        switch (_stateMachine.State)
        {
            case GameState.Swapping:
            case GameState.SwapBack:
                plan = _stateMachine.UpdateSwap();
                break;
            case GameState.Resolving:
                if (_resolution.IsFinished) _stateMachine.ApplyGravity();
                break;
            case GameState.Falling:
                plan = _stateMachine.UpdateFalling();
                break;
            case GameState.Idle:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (plan != null)
            _resolution.Start(plan);

        _resolution.Update(deltaTime);
        _animationManager.Update(deltaTime);
    }

    private void OnSwapRequested(SwapCommand command)
    {
        _inputHandler.ClearSelection();
        _stateMachine.RequestSwap(command);
    }
}