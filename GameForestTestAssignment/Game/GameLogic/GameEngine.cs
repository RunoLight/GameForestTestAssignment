using System;
using GameForestTestAssignment.Game.Animations;
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

        switch (_stateMachine.State)
        {
            case GameState.Swapping:
            case GameState.SwapBack:
            {
                var (swapComplete, plan) = _stateMachine.UpdateSwap();
                if (swapComplete && plan != null) _resolution.Start(plan);
                break;
            }
            case GameState.Resolving:
                if (_resolution.IsFinished) _stateMachine.ApplyGravity();
                break;
            case GameState.Falling:
            {
                var (fallComplete, plan) = _stateMachine.UpdateFalling();
                if (fallComplete && plan != null) _resolution.Start(plan);
                break;
            }
            case GameState.Idle:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _resolution.Update(deltaTime);
        _animationManager.Update(deltaTime);
    }

    private void OnSwapRequested(SwapCommand command)
    {
        _inputHandler.ClearSelection();
        _stateMachine.RequestSwap(command);
    }
}
