using System;
using GameForestTestAssignment.Game.Animations;
using GameForestTestAssignment.Game.Bonuses;
using GameForestTestAssignment.Game.MatchDetection;

namespace GameForestTestAssignment.Game.GameLogic;

public class ResolutionProcessor(
    IBoard board,
    AnimationManager animationManager,
    BoardEffectQueue effects,
    BonusManager bonusManager,
    RemovalService removalService,
    BombService bombService,
    DestroyerService destroyerService
)
{
    public bool IsFinished =>
        !animationManager.IsPlaying<DisappearAnimation>() &&
        destroyerService.DestroyerCount == 0 &&
        bombService.PendingBombCount == 0 &&
        effects.IsEmpty;

    public void Start(ResolutionPlan plan)
    {
        removalService.Clear();
        bonusManager.Clear();

        foreach (var (pos, bonus) in plan.BonusesToSpawn)
            board[pos.X, pos.Y].Bonus = bonus;

        foreach (var cell in plan.CellsToRemove)
            effects.Enqueue(new RemoveCellEffect(cell.X, cell.Y));

        foreach (var (x, y) in plan.BonusesToActivate)
        {
            var bonus = board[x, y].Bonus;
            if (bonus != null)
                effects.Enqueue(new ActivateBonusEffect(x, y, bonus));
        }

        ProcessEffects();
    }

    public void Update(float deltaTime)
    {
        destroyerService.Update(deltaTime);
        bombService.ProcessPendingBombExplosions(deltaTime);
        ProcessEffects();
    }

    private void ProcessEffects()
    {
        while (effects.TryDequeue(out var effect))
        {
            switch (effect)
            {
                case RemoveCellEffect e:
                    removalService.MarkCellForRemoval(e.X, e.Y);
                    break;
                case ActivateBonusEffect e:
                    bonusManager.Activate(e.Bonus, e.X, e.Y);
                    break;
                case SpawnDestroyerEffect e:
                    destroyerService.SpawnDestroyer(e.X, e.Y, e.Dx, e.Dy, e.ColorType);
                    break;
                case ScheduleBombExplosionEffect e:
                    bombService.ScheduleBombExplosion(e.X, e.Y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(effect), effect, null);
            }
        }
    }
}
