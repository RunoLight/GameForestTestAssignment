#region

using System.Collections.Generic;
using GameForestTestAssignment.Game.Effects;
using Microsoft.Xna.Framework;

#endregion

namespace GameForestTestAssignment.Game.GameLogic;

public class BombService(Board board, ParticlePool particlePool, IBonusActivator bonusActivator)
{
    private readonly List<(int X, int Y, float Delay)> _pendingBombExplosions = [];

    public int PendingBombCount => _pendingBombExplosions.Count;

    public void ScheduleBombExplosion(int x, int y)
    {
        bonusActivator.MarkCellForRemoval(x, y);
        _pendingBombExplosions.Add((x, y, 250f));
    }

    public void ProcessPendingBombExplosions(float deltaTime)
    {
        for (var i = _pendingBombExplosions.Count - 1; i >= 0; i--)
        {
            var (x, y, delay) = _pendingBombExplosions[i];
            delay -= deltaTime;
            if (delay > 0)
            {
                _pendingBombExplosions[i] = (x, y, delay);
                continue;
            }

            for (var dx = -1; dx <= 1; dx++)
            for (var dy = -1; dy <= 1; dy++)
            {
                var nx = x + dx;
                var ny = y + dy;
                bonusActivator.MarkCellForRemoval(nx, ny);
            }

            particlePool.CreateBigExplosion(GetCellCenter(x, y), Color.Orange);
            _pendingBombExplosions.RemoveAt(i);
        }
    }

    private Vector2 GetCellCenter(int x, int y)
    {
        return board.BoardPosition + new Vector2(
            x * board.CellSize + board.CellSize * 0.5f,
            y * board.CellSize + board.CellSize * 0.5f);
    }
}