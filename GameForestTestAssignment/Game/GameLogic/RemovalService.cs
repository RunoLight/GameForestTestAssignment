using System.Collections.Generic;
using GameForestTestAssignment.Game.Animations;
using GameForestTestAssignment.Game.Effects;

namespace GameForestTestAssignment.Game.GameLogic;

public class RemovalService(
    IBoard board,
    AnimationManager animationManager,
    ScoreManager scoreManager,
    ParticlePool particlePool,
    IBonusActivator bonusActivator
)
{
    private readonly HashSet<(int X, int Y)> _cellsToRemove = [];

    public IEnumerable<DisappearAnimation> RemovalAnimations => RemovalAnimationsDict.Values;
    public Dictionary<(int X, int Y), DisappearAnimation> RemovalAnimationsDict { get; } = new();

    public void MarkCellForRemoval(int x, int y)
    {
        TryMarkForRemoval(x, y, true);
    }

    public void Clear()
    {
        _cellsToRemove.Clear();
        RemovalAnimationsDict.Clear();
    }

    public void ClearCells()
    {
        foreach (var cell in _cellsToRemove)
        {
            if (!board.IsInBounds(cell.X, cell.Y))
                continue;

            board[cell.X, cell.Y].CellType = CellType.None;
            board[cell.X, cell.Y].Bonus = null;
        }

        _cellsToRemove.Clear();
        RemovalAnimationsDict.Clear();
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

        var disappearAnim = new DisappearAnimation(280f);
        RemovalAnimationsDict[(x, y)] = disappearAnim;
        animationManager.AddAnimation(disappearAnim);
        particlePool.CreateExplosion(board.GetCellCenter(x, y), board[x, y].CellType.GetColor());

        var bonus = board[x, y].Bonus;
        if (bonus != null)
            bonusActivator.QueueBonus(bonus, y, x);
    }
}