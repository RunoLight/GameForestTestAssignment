#region

using System;
using GameForestTestAssignment.Game.Animations;
using GameForestTestAssignment.Game.Bonuses;
using GameForestTestAssignment.Game.Effects;
using GameForestTestAssignment.Game.MatchDetection;
using GameForestTestAssignment.Game.PlayerInput;
using Microsoft.Xna.Framework;

#endregion

namespace GameForestTestAssignment.Game.GameLogic;

public class GameEngine
{
    private readonly AnimationManager _animationManager;
    private readonly Board _board;
    private readonly BombService _bombService;
    private readonly BonusManager _bonusManager;
    private readonly DestroyerService _destroyerService;
    private readonly GravityService _gravityService;
    private readonly InputHandler _inputHandler;
    private readonly ParticlePool _particlePool;
    private readonly RemovalService _removalService;
    private readonly RenderStateApplier _renderStateApplier;

    private readonly GameStateMachine _stateMachine;
    private readonly TimerManager _timerManager;

    public GameEngine(Board board, BoardRenderer boardRenderer, AnimationManager animationManager,
        MatchDetector matchDetector, ScoreManager scoreManager, TimerManager timerManager,
        BonusManager bonusManager, ParticlePool particlePool, InputHandler inputHandler)
    {
        _board = board;
        _animationManager = animationManager;
        _timerManager = timerManager;
        _particlePool = particlePool;
        _inputHandler = inputHandler;
        _bonusManager = bonusManager;

        _stateMachine = new GameStateMachine(board, matchDetector, animationManager);

        var activationContext = new BonusActivationContext();

        _removalService = new RemovalService(board, animationManager, scoreManager, particlePool, activationContext);
        _bombService = new BombService(board, particlePool, activationContext);
        _destroyerService = new DestroyerService(board, activationContext);

        _bonusManager.SetActivator(activationContext);

        activationContext.Initialize(_removalService, _bombService, _destroyerService, _bonusManager);


        _renderStateApplier = new RenderStateApplier(board, boardRenderer);
        _gravityService = new GravityService(board, animationManager, bonusManager);

        _inputHandler.SwapRequested += OnSwapRequested;
        _inputHandler.CellSelected += OnCellSelected;
        _inputHandler.SelectionCleared += OnSelectionCleared;
    }

    public void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

        if (!_timerManager.IsExpired)
            _timerManager.Update(deltaTime / 1000f);

        _inputHandler.Enabled = _stateMachine.CanAcceptInput;
        _inputHandler.Update();

        switch (_stateMachine.State)
        {
            case GameState.Swapping:
            case GameState.SwapBack:
            {
                var (swapComplete, plan) = _stateMachine.UpdateSwap();
                if (swapComplete && plan != null) StartResolution(plan);
                break;
            }
            case GameState.Resolving:
                UpdateResolving();
                break;
            case GameState.Falling:
            {
                var (fallComplete, plan) = _stateMachine.UpdateFalling();
                if (fallComplete && plan != null) StartResolution(plan);
                break;
            }
            case GameState.Idle:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _renderStateApplier.UpdateHighlight(deltaTime);
        _destroyerService.Update(deltaTime);
        _bombService.ProcessPendingBombExplosions(deltaTime);
        _bonusManager.Flush();

        _particlePool.Update(deltaTime);
        _animationManager.Update(deltaTime);
    }

    public void ApplyRenderState()
    {
        _renderStateApplier.ApplyRenderState(
            _stateMachine.SwapAnimationsDict,
            _stateMachine.FallAnimationsDict,
            _removalService.RemovalAnimationsDict
        );
    }

    public void DrawEffects()
    {
        _renderStateApplier.DrawEffects(_destroyerService.Destroyers);
    }

    private void OnCellSelected(int x, int y)
    {
        _renderStateApplier.SelectCell(x, y);
    }

    private void OnSelectionCleared()
    {
        _renderStateApplier.ClearSelection();
    }

    private void OnSwapRequested(SwapCommand command)
    {
        _inputHandler.ClearSelection();
        _stateMachine.RequestSwap(command);
    }

    private void StartResolution(ResolutionPlan plan)
    {
        _removalService.Clear();
        _bonusManager.Clear();

        foreach (var (pos, bonus) in plan.BonusesToSpawn)
        {
            _board[pos.X, pos.Y].Bonus = bonus;
            bonus.Col = pos.X;
            bonus.Row = pos.Y;
        }

        foreach (var cell in plan.CellsToRemove)
            _removalService.MarkCellForRemoval(cell.X, cell.Y);

        foreach (var (x, y) in plan.BonusesToActivate)
        {
            var bonus = _board[x, y].Bonus;
            if (bonus != null)
                _bonusManager.QueueBonus(bonus, y, x);
        }

        _bonusManager.Flush();
    }

    private void UpdateResolving()
    {
        var animsDone = true;

        foreach (var anim in _removalService.RemovalAnimations)
            if (!anim.IsComplete)
                animsDone = false;

        if (animsDone && _destroyerService.DestroyerCount == 0 &&
            _bombService.PendingBombCount == 0 &&
            !_bonusManager.HasPending)
        {
            _removalService.ClearCells();
            _removalService.Clear();
            _stateMachine.ApplyGravity(_gravityService);
        }
    }
}