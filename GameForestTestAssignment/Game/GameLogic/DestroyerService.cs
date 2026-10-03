#region

using System.Collections.Generic;
using GameForestTestAssignment.Game.Effects;

#endregion

namespace GameForestTestAssignment.Game.GameLogic;

public class DestroyerService(IBoard board, IBonusActivator bonusActivator)
{
    private readonly List<Destroyer> _destroyers = [];

    public IEnumerable<Destroyer> Destroyers => _destroyers;
    public int DestroyerCount => _destroyers.Count;

    public void SpawnDestroyer(int x, int y, int dx, int dy, CellType colorType)
    {
        _destroyers.Add(new Destroyer(x, y, dx, dy, colorType, board.GetCellCenter(x, y)));
    }

    public void Update(float deltaTime)
    {
        for (var i = _destroyers.Count - 1; i >= 0; i--)
        {
            var destroyer = _destroyers[i];
            destroyer.Update(deltaTime, board, bonusActivator.MarkCellForRemoval);

            if (!destroyer.IsAlive)
                _destroyers.RemoveAt(i);
        }
    }
}