using System.Collections.Generic;
using GameForestTestAssignment.Game.Animations;
using GameForestTestAssignment.Game.Effects;

namespace GameForestTestAssignment.Game.GameLogic;

public class RemovalService(
    IBoard board,
    BoardLayout layout,
    AnimationManager animationManager,
    ScoreManager scoreManager,
    ParticlePool particlePool,
    BoardEffectQueue effects
)
{
    private readonly HashSet<(int X, int Y)> _cellsToRemove = [];

    public void MarkCellForRemoval(int x, int y)
    {
        TryMarkForRemoval(x, y, true);
    }

    public void Clear()
    {
        _cellsToRemove.Clear();
    }

    private void TryMarkForRemoval(int x, int y, bool awardScore)
    {
        if (!board.IsInBounds(x, y))
            return;

        if (board[x, y].CellType == CellType.None)
            return;

        if (!_cellsToRemove.Add((x, y)))
            return;

        if (awardScore)
            scoreManager.AddScore(1);

        animationManager.Play(new DisappearAnimation(x, y, 0.28f), () => ClearCell(x, y));
        particlePool.CreateExplosion(layout.GetCellCenter(x, y), board[x, y].CellType.GetColor());

        var bonus = board[x, y].Bonus;
        if (bonus != null)
            effects.Enqueue(new ActivateBonusEffect(x, y, bonus));
    }

    private void ClearCell(int x, int y)
    {
        board[x, y].CellType = CellType.None;
        board[x, y].Bonus = null;
    }
}