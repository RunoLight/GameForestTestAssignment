using System.Collections.Generic;
using GameForestTestAssignment.Game.Bonuses;

namespace GameForestTestAssignment.Game.MatchDetection;

public record ResolutionPlan(
    HashSet<(int X, int Y)> CellsToRemove,
    Dictionary<(int X, int Y), Bonus> BonusesToSpawn,
    List<(int X, int Y)> BonusesToActivate) {
    public bool IsEmpty => CellsToRemove.Count == 0 && BonusesToSpawn.Count == 0 && BonusesToActivate.Count == 0;
    public bool Involves(int x, int y) =>
        CellsToRemove.Contains((x, y)) || BonusesToSpawn.ContainsKey((x, y)) ||
        BonusesToActivate.Exists(p => p.X == x && p.Y == y);
}

public static class MatchResolver
{
    public static ResolutionPlan Build(IBoard board, MatchDetector detector, (int X, int Y)? lastMoved)
    {
        var cellsToRemove = new HashSet<(int X, int Y)>();
        var bonusesToSpawn = new Dictionary<(int X, int Y), Bonus>();
        var bonusesToActivate = new List<(int X, int Y)>();
        var plan = new ResolutionPlan(cellsToRemove, bonusesToSpawn, bonusesToActivate);
        var (intersections, hMatches, vMatches) = detector.FindMatchesWithClassification(board);
        var allMatches = new List<MatchResult>(hMatches);
        allMatches.AddRange(vMatches);

        if (allMatches.Count == 0)
            return plan;

        var matchCells = new HashSet<(int X, int Y)>();
        foreach (var match in allMatches)
        {
            foreach (var cell in match.Cells)
                matchCells.Add(cell);
        }

        foreach (var cell in matchCells)
        {
            if (board[cell.X, cell.Y].Bonus != null)
                plan.BonusesToActivate.Add(cell);
        }

        foreach (var intersection in intersections)
        {
            TrySpawn(plan, board, intersection,
                BonusFactory.Create(BonusType.Bomb, board[intersection.Item1, intersection.Item2].CellType));
        }

        foreach (var match in allMatches)
        {
            var preferred = PreferredCell(match, lastMoved);
            if (match.Length >= 5)
            {
                TrySpawn(plan, board, preferred, BonusFactory.Create(BonusType.Bomb, match.Type));
            }
            else if (match.Length == 4)
            {
                var orientation = match.Direction == MatchDirection.Horizontal
                    ? BonusOrientation.Horizontal
                    : BonusOrientation.Vertical;
                TrySpawn(plan, board, preferred, BonusFactory.Create(BonusType.Line, match.Type, orientation));
            }
        }

        foreach (var cell in matchCells)
        {
            if (!plan.BonusesToSpawn.ContainsKey(cell))
                plan.CellsToRemove.Add(cell);
        }

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