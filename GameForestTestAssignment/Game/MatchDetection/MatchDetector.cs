using System.Collections.Generic;

namespace GameForestTestAssignment.Game.MatchDetection;

public class MatchResult(Orientation orientation, CellType type)
{
    public List<(int X, int Y)> Cells { get; } = [];
    public Orientation Orientation { get; } = orientation;
    public int Length => Cells.Count;
    public CellType Type { get; } = type;

    public void AddCell(int x, int y)
    {
        Cells.Add((x, y));
    }
}

public class MatchDetector
{
    public (List<MatchResult> Matches, HashSet<(int X, int Y)> Intersections) FindMatches(IBoard board)
    {
        var matches = new List<MatchResult>();

        for (var y = 0; y < Board.Height; y++)
            ScanRow(board, y, matches);

        for (var x = 0; x < Board.Width; x++)
            ScanColumn(board, x, matches);

        var horizontalCells = new HashSet<(int X, int Y)>();
        var verticalCells = new HashSet<(int X, int Y)>();
        foreach (var match in matches)
            (match.Orientation == Orientation.Horizontal ? horizontalCells : verticalCells).UnionWith(match.Cells);

        horizontalCells.IntersectWith(verticalCells);
        return (matches, horizontalCells);
    }

    private static void ScanRow(IBoard board, int y, List<MatchResult> matches)
    {
        var x = 0;
        while (x < Board.Width)
        {
            var cell = board[x, y];
            if (cell.IsEmpty)
            {
                x++;
                continue;
            }

            var runLength = 1;
            while (x + runLength < Board.Width &&
                   !board[x + runLength, y].IsEmpty &&
                   board[x + runLength, y].CellType == cell.CellType)
                runLength++;

            if (runLength >= 3)
            {
                var match = new MatchResult(Orientation.Horizontal, cell.CellType);
                for (var i = 0; i < runLength; i++) match.AddCell(x + i, y);
                matches.Add(match);
            }

            x += runLength;
        }
    }

    private static void ScanColumn(IBoard board, int x, List<MatchResult> matches)
    {
        var y = 0;
        while (y < Board.Height)
        {
            var cell = board[x, y];
            if (cell.IsEmpty)
            {
                y++;
                continue;
            }

            var runLength = 1;
            while (y + runLength < Board.Height &&
                   !board[x, y + runLength].IsEmpty &&
                   board[x, y + runLength].CellType == cell.CellType)
                runLength++;

            if (runLength >= 3)
            {
                var match = new MatchResult(Orientation.Vertical, cell.CellType);
                for (var i = 0; i < runLength; i++) match.AddCell(x, y + i);
                matches.Add(match);
            }

            y += runLength;
        }
    }
}