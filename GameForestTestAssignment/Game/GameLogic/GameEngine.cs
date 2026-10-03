using System;
using GameForestTestAssignment.Game.Animations;
using GameForestTestAssignment.Game.Bonuses;
using GameForestTestAssignment.Game.MatchDetection;
using GameForestTestAssignment.Game.PlayerInput;

namespace GameForestTestAssignment.Game.GameLogic;

public class GameEngine
{
    private readonly AnimationManager _animationManager;
    private readonly Board _board;
    private readonly BombService _bombService;
    private readonly BonusManager _bonusManager;
    private readonly DestroyerService _destroyerService;
    private readonly BoardEffectQueue _effects;
    private readonly InputHandler _inputHandler;
    private readonly RemovalService _removalService;
    private readonly GameStateMachine _stateMachine;

    public GameEngine(Board board, GameStateMachine stateMachine, InputHandler inputHandler,
        AnimationManager animationManager, BoardEffectQueue effects, BonusManager bonusManager,
        RemovalService removalService, BombService bombService, DestroyerService destroyerService)
    {
        _board = board;
        _stateMachine = stateMachine;
        _inputHandler = inputHandler;
        _animationManager = animationManager;
        _effects = effects;
        _bonusManager = bonusManager;
        _removalService = removalService;
        _bombService = bombService;
        _destroyerService = destroyerService;

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

        _destroyerService.Update(deltaTime);
        _bombService.ProcessPendingBombExplosions(deltaTime);
        ProcessEffects();

        _animationManager.Update(deltaTime);
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
        if (_animationManager.IsPlaying<DisappearAnimation>() ||
            _destroyerService.DestroyerCount > 0 ||
            _bombService.PendingBombCount > 0 ||
            !_effects.IsEmpty)
            return;

        _removalService.Clear();
        _stateMachine.ApplyGravity();
    }
}
