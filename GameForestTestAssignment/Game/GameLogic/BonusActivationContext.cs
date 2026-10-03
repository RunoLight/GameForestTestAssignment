using GameForestTestAssignment.Game.Bonuses;

namespace GameForestTestAssignment.Game.GameLogic;

public sealed class BonusActivationContext : IBonusActivator
{
    private BombService _bomb;
    private BonusManager _bonusManager;
    private DestroyerService _destroyer;
    private RemovalService _removal;

    public void MarkCellForRemoval(int x, int y)
    {
        _removal.MarkCellForRemoval(x, y);
    }

    public void SpawnDestroyer(int x, int y, int dx, int dy, CellType c)
    {
        _destroyer.SpawnDestroyer(x, y, dx, dy, c);
    }

    public void ScheduleBombExplosion(int x, int y)
    {
        _bomb.ScheduleBombExplosion(x, y);
    }

    public void QueueBonus(Bonus bonus, int row, int col)
    {
        _bonusManager.QueueBonus(bonus, row, col);
    }

    internal void Initialize(
        RemovalService removal,
        BombService bomb,
        DestroyerService destroyer,
        BonusManager bonusManager)
    {
        _removal = removal;
        _bomb = bomb;
        _destroyer = destroyer;
        _bonusManager = bonusManager;
    }
}