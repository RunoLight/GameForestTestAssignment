#region

using System.Collections.Generic;
using GameForestTestAssignment.Game.GameLogic;

#endregion

namespace GameForestTestAssignment.Game.Bonuses;

public class BonusManager
{
    private readonly Queue<Bonus> _activationQueue = new();
    private readonly HashSet<(int Row, int Col)> _queuedOrActivated = [];

    private IBonusActivator _activator;

    public bool HasPending => _activationQueue.Count > 0;

    public void SetActivator(IBonusActivator activationContext)
    {
        _activator = activationContext;
    }

    public void QueueBonus(Bonus bonus, int row, int col)
    {
        bonus.Row = row;
        bonus.Col = col;
        if (!_queuedOrActivated.Add((row, col)))
            return;

        _activationQueue.Enqueue(bonus);
    }

    public void Flush()
    {
        while (_activationQueue.Count > 0)
        {
            var activation = _activationQueue.Dequeue();
            ProcessBonus(activation, _activator);
        }
    }

    public void Clear()
    {
        _activationQueue.Clear();
        _queuedOrActivated.Clear();
    }

    private static void ProcessBonus(Bonus bonus, IBonusActivator activator)
    {
        switch (bonus)
        {
            case LineBonus lineBonus:
                ProcessLineBonus(lineBonus, activator);
                break;
            case BombBonus bombBonus:
                ProcessBombBonus(bombBonus, activator);
                break;
        }
    }

    private static void ProcessLineBonus(LineBonus lineBonus, IBonusActivator activator)
    {
        activator.MarkCellForRemoval(lineBonus.Col, lineBonus.Row);

        if (lineBonus.Orientation == BonusOrientation.Horizontal)
        {
            activator.SpawnDestroyer(lineBonus.Col, lineBonus.Row, -1, 0, lineBonus.ColorType);
            activator.SpawnDestroyer(lineBonus.Col, lineBonus.Row, 1, 0, lineBonus.ColorType);
        }
        else
        {
            activator.SpawnDestroyer(lineBonus.Col, lineBonus.Row, 0, -1, lineBonus.ColorType);
            activator.SpawnDestroyer(lineBonus.Col, lineBonus.Row, 0, 1, lineBonus.ColorType);
        }
    }

    private static void ProcessBombBonus(BombBonus bombBonus, IBonusActivator activator)
    {
        activator.ScheduleBombExplosion(bombBonus.Col, bombBonus.Row);
    }
}