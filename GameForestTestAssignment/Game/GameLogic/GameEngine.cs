using System;
using GameForestTestAssignment.Game.Animations;
using GameForestTestAssignment.Game.Bonuses;
using GameForestTestAssignment.Game.Effects;
using GameForestTestAssignment.Game.MatchDetection;
using GameForestTestAssignment.Game.PlayerInput;
using Microsoft.Xna.Framework;

namespace GameForestTestAssignment.Game.GameLogic;

public class GameEngine
{
    private readonly AnimationManager _animationManager;
    private readonly Board _board;
    private readonly BombService _bombService;
    private readonly BonusManager _bonusManager;
    private readonly DestroyerService _destroyerService;
    private readonly BoardEffectQueue _effects = new();
    private readonly GravityService _gravityService;
    private readonly InputHandler _inputHandler;
    private readonly ParticlePool _particlePool;
    private readonly RemovalService _removalService;
    private readonly RenderStateApplier _renderStateApplier;

    private readonly GameStateMachine _stateMachine;
    private readonly TimerManager _timerManager;

    public GameEngine(Board board, BoardRenderer boardRenderer, AnimationManager animationManager,
        MatchDetector matchDetector, ScoreManager scoreManager, TimerManager timerManager,
        ParticlePool particlePool, InputHandler inputHandler)
    {
        _board = board;
        _animationManager = animationManager;
        _timerManager = timerManager;
        _particlePool = particlePool;
        _inputHandler = inputHandler;

        _stateMachine = new GameStateMachine(board, matchDetector, animationManager);

        _bonusManager = new BonusManager(_effects);
        _removalService = new RemovalService(board, animationManager, scoreManager, particlePool, _effects);
        _bombService = new BombService(board, particlePool, _effects);
        _destroyerService = new DestroyerService(board, _effects);

        _renderStateApplier = new RenderStateApplier(board, boardRenderer);
        _gravityService = new GravityService(board, animationManager, _bonusManager);

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
        ProcessEffects();

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
            _board[pos.X, pos.Y].Bonus = bonus;

        foreach (var cell in plan.CellsToRemove)
            _effects.Enqueue(new RemoveCellEffect(cell.X, cell.Y));

        foreach (var (x, y) in plan.BonusesToActivate)
        {
            var bonus = _board[x, y].Bonus;
            if (bonus != null)
                _effects.Enqueue(new ActivateBonusEffect(x, y, bonus));
        }

        ProcessEffects();
    }

    private void ProcessEffects()
    {
        while (_effects.TryDequeue(out var effect))
        {
            switch (effect)
            {
                case RemoveCellEffect e:
                    _removalService.MarkCellForRemoval(e.X, e.Y);
                    break;
                case ActivateBonusEffect e:
                    _bonusManager.Activate(e.Bonus, e.X, e.Y);
                    break;
                case SpawnDestroyerEffect e:
                    _destroyerService.SpawnDestroyer(e.X, e.Y, e.Dx, e.Dy, e.ColorType);
                    break;
                case ScheduleBombExplosionEffect e:
                    _bombService.ScheduleBombExplosion(e.X, e.Y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(effect), effect, null);
            }
        }
    }

    private void UpdateResolving()
    {
        var animsDone = true;

        foreach (var anim in _removalService.RemovalAnimations)
            if (!anim.IsComplete)
                animsDone = false;

        if (animsDone && _destroyerService.DestroyerCount == 0 &&
            _bombService.PendingBombCount == 0 &&
            _effects.IsEmpty)
        {
            _removalService.ClearCells();
            _removalService.Clear();
            _stateMachine.ApplyGravity(_gravityService);
        }
    }
}