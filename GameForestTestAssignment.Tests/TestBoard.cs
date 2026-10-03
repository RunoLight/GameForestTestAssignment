using GameForestTestAssignment.Game;

namespace GameForestTestAssignment.Tests;

/// <summary>
/// Builds boards from text rows: '.' is empty, R/G/B/Y/P are the five piece types.
/// Rows and columns that are not specified stay empty.
/// </summary>
internal static class TestBoard
{
    private static readonly CellType[] Palette =
        [CellType.Circle, CellType.Square, CellType.Triangle, CellType.Diamond, CellType.Star];

    public static Board FromRows(params string[] rows)
    {
        var board = new Board();
        for (var y = 0; y < rows.Length; y++)
        for (var x = 0; x < rows[y].Length; x++)
            board[x, y].CellType = Parse(rows[y][x]);

        return board;
    }

    /// <summary>
    /// A full board with no matches and no possible moves:
    /// neighbors differ by 1 along a row and by 2 along a column (mod 5), so no swap lines up three.
    /// </summary>
    public static Board Deadlocked()
    {
        var board = new Board();
        for (var x = 0; x < Board.Width; x++)
        for (var y = 0; y < Board.Height; y++)
            board[x, y].CellType = Palette[(x + 2 * y) % Palette.Length];

        return board;
    }

    public static CellType Parse(char symbol)
    {
        return symbol switch
        {
            '.' => CellType.None,
            'R' => CellType.Circle,
            'G' => CellType.Square,
            'B' => CellType.Triangle,
            'Y' => CellType.Diamond,
            'P' => CellType.Star,
            _ => throw new ArgumentOutOfRangeException(nameof(symbol), symbol, null)
        };
    }

    public static IEnumerable<(int X, int Y)> AllPositions()
    {
        for (var x = 0; x < Board.Width; x++)
        for (var y = 0; y < Board.Height; y++)
            yield return (x, y);
    }
}
