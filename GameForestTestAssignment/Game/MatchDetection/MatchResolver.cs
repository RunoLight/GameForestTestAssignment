using System.Collections.Generic;
using GameForestTestAssignment.Game.Bonuses;

namespace GameForestTestAssignment.Game.MatchDetection;

public record ResolutionPlan(
    HashSet<(int X, int Y)> CellsToRemove,
    Dictionary<(int X, int Y), Bonus> BonusesToSpawn,
    List<(int X, int Y)> BonusesToActivate)
{
    public bool IsEmpty => CellsToRemove.Count == 0 && BonusesToSpawn.Count == 0 && BonusesToActivate.Count == 0;

    public bool Involves(int x, int y)
    {
        return CellsToRemove.Contains((x, y)) || BonusesToSpawn.ContainsKey((x, y)) ||
               BonusesToActivate.Exists(p => p.X == x && p.Y == y);
    }
}

public static class MatchResolver
{
    public static ResolutionPlan Build(IBoard board, MatchDetector detector, (int X, int Y)? lastMoved)
    {
        var cellsToRemove = new HashSet<(int X, int Y)>();
        var bonusesToSpawn = new Dictionary<(int X, int Y), Bonus>();
        var bonusesToActivate = new List<(int X, int Y)>();
        var plan = new ResolutionPlan(cellsToRemove, bonusesToSpawn, bonusesToActivate);
        var (matches, intersections) = detector.FindMatches(board);

        if (matches.Count == 0)
            return plan;

        var matchCells = new HashSet<(int X, int Y)>();
        foreach (var match in matches)
            matchCells.UnionWith(match.Cells);

        foreach (var cell in matchCells)
            if (board[cell.X, cell.Y].Bonus != null)
                plan.BonusesToActivate.Add(cell);

        foreach (var cell in intersections)
            TrySpawn(plan, board, cell, new BombBonus(board[cell.X, cell.Y].CellType));

        foreach (var match in matches)
        {
            var preferred = PreferredCell(match, lastMoved);
            if (match.Length >= 5)
                TrySpawn(plan, board, preferred, new BombBonus(match.Type));
            else if (match.Length == 4) TrySpawn(plan, board, preferred, new LineBonus(match.Type, match.Orientation));
        }

        foreach (var cell in matchCells)
            if (!plan.BonusesToSpawn.ContainsKey(cell))
                plan.CellsToRemove.Add(cell);

        return plan;
    }

    private static (int X, int Y) PreferredCell(MatchResult match, (int X, int Y)? lastMoved)
    {
        if (lastMoved.HasValue && match.Cells.Contains(lastMoved.Value))
            return lastMoved.Value;
        return match.Cells[match.Cells.Count / 2];
    }

    private static void TrySpawn(ResolutionPlan plan, IBoard board, (int X, int Y) cell, Bonus bonus)
    {
        if (plan.BonusesToSpawn.ContainsKey(cell))
            return;
        if (board[cell.X, cell.Y].Bonus != null)
            return;
        plan.BonusesToSpawn[cell] = bonus;
    }
}