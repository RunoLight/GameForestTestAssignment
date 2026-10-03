using System.Collections.Generic;
using GameForestTestAssignment.Game.Bonuses;

namespace GameForestTestAssignment.Game.GameLogic;

public abstract record BoardEffect;

public sealed record RemoveCellEffect(int X, int Y) : BoardEffect;

public sealed record ActivateBonusEffect(int X, int Y, Bonus Bonus) : BoardEffect;

public sealed record SpawnDestroyerEffect(int X, int Y, int Dx, int Dy, CellType ColorType) : BoardEffect;

public sealed record ScheduleBombExplosionEffect(int X, int Y) : BoardEffect;

public sealed class BoardEffectQueue
{
    private readonly Queue<BoardEffect> _queue = new();

    public bool IsEmpty => _queue.Count == 0;

    public void Enqueue(BoardEffect effect)
    {
        _queue.Enqueue(effect);
    }

    public bool TryDequeue(out BoardEffect effect)
    {
        return _queue.TryDequeue(out effect);
    }
}
