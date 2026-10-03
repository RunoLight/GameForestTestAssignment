using GameForestTestAssignment.Game.Bonuses;

namespace GameForestTestAssignment.Game.GameLogic;

public interface IBonusActivator
{
    void MarkCellForRemoval(int x, int y);
    void SpawnDestroyer(int x, int y, int dx, int dy, CellType colorType);
    void ScheduleBombExplosion(int x, int y);
    void QueueBonus(Bonus bonus, int row, int col);
}