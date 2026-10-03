using System.Collections.Generic;
using System.Linq;

namespace GameForestTestAssignment.Game.MatchDetection;

public enum MatchDirection
{
    Horizontal,
    Vertical
}

public class MatchResult(MatchDirection direction, CellType type, int length)
{
    public List<(int X, int Y)> Cells { get; } = [];
    public MatchDirection Direction { get; } = direction;
    public int Length { get; } = length;
    public CellType Type { get; } = type;

    public void AddCell(int x, int y)
    {
        Cells.Add((x, y));
    }
}

public class MatchDetector
{
    public (
        HashSet<(int, int)> intersectionCells,
        List<MatchResult> horizontalMatches,
        List<MatchResult>verticalMatches
        ) FindMatchesWithClassification(IBoard board)
    {
        var allMatches = FindMatches(board);
        var horizontalMatches = new List<MatchResult>();
        var verticalMatches = new List<MatchResult>();
        var horizontalCells = new HashSet<(int, int)>();
        var verticalCells = new HashSet<(int, int)>();

        foreach (var match in allMatches)
            if (match.Direction == MatchDirection.Horizontal)
            {
                horizontalMatches.Add(match);
                foreach (var cell in match.Cells)
                    horizontalCells.Add(cell);
            }
            else
            {
                verticalMatches.Add(match);
                foreach (var cell in match.Cells)
                    verticalCells.Add(cell);
            }

        var intersectionCells = horizontalCells.Where(verticalCells.Contains).ToHashSet();

        return (intersectionCells, horizontalMatches, verticalMatches);
    }

    private static List<MatchResult> FindMatches(IBoard board)
    {
        var matches = new List<MatchResult>();

        for (var y = 0; y < Board.Height; y++)
            ScanRow(board, y, matches);

        for (var x = 0; x < Board.Width; x++)
            ScanColumn(board, x, matches);

        return matches;
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
                var match = new MatchResult(MatchDirection.Horizontal, cell.CellType, runLength);
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
                var match = new MatchResult(MatchDirection.Vertical, cell.CellType, runLength);
                for (var i = 0; i < runLength; i++) match.AddCell(x, y + i);
                matches.Add(match);
            }

            y += runLength;
        }
    }
}