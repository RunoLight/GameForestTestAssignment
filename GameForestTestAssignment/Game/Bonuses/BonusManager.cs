using System.Collections.Generic;
using GameForestTestAssignment.Game.GameLogic;

namespace GameForestTestAssignment.Game.Bonuses;

public class BonusManager(BoardEffectQueue effects)
{
    private readonly HashSet<(int X, int Y)> _activated = [];

    public void Activate(Bonus bonus, int x, int y)
    {
        if (!_activated.Add((x, y)))
            return;

        switch (bonus)
        {
            case LineBonus lineBonus:
                ActivateLineBonus(lineBonus, x, y);
                break;
            case BombBonus:
                ActivateBombBonus(x, y);
                break;
        }
    }

    public void Clear()
    {
        _activated.Clear();
    }

    private void ActivateLineBonus(LineBonus lineBonus, int x, int y)
    {
        effects.Enqueue(new RemoveCellEffect(x, y));

        if (lineBonus.Orientation == BonusOrientation.Horizontal)
        {
            effects.Enqueue(new SpawnDestroyerEffect(x, y, -1, 0, lineBonus.ColorType));
            effects.Enqueue(new SpawnDestroyerEffect(x, y, 1, 0, lineBonus.ColorType));
        }
        else
        {
            effects.Enqueue(new SpawnDestroyerEffect(x, y, 0, -1, lineBonus.ColorType));
            effects.Enqueue(new SpawnDestroyerEffect(x, y, 0, 1, lineBonus.ColorType));
        }
    }

    private void ActivateBombBonus(int x, int y)
    {
        effects.Enqueue(new RemoveCellEffect(x, y));
        effects.Enqueue(new ScheduleBombExplosionEffect(x, y));
    }
}
