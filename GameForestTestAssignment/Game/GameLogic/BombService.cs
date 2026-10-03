using System.Collections.Generic;
using GameForestTestAssignment.Game.Effects;
using Microsoft.Xna.Framework;

namespace GameForestTestAssignment.Game.GameLogic;

public class BombService(
    BoardLayout layout,
    ParticlePool particlePool,
    ScreenShake screenShake,
    BoardEffectQueue effects
)
{
    private const float ExplosionDelay = 0.25f;

    private readonly List<(int X, int Y, float Delay)> _pendingBombExplosions = [];

    public int PendingBombCount => _pendingBombExplosions.Count;

    public void ScheduleBombExplosion(int x, int y)
    {
        _pendingBombExplosions.Add((x, y, ExplosionDelay));
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
                effects.Enqueue(new RemoveCellEffect(x + dx, y + dy));

            particlePool.CreateBigExplosion(layout.GetCellCenter(x, y), Color.Orange);
            screenShake.Start();
            _pendingBombExplosions.RemoveAt(i);
        }
    }
}