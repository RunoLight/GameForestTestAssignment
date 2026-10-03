namespace GameForestTestAssignment.Game.MatchDetection;

public static class MoveFinder
{
    public static bool HasPossibleMove(IBoard board)
    {
        for (var x = 0; x < Board.Width; x++)
        for (var y = 0; y < Board.Height; y++)
        {
            if (x + 1 < Board.Width && SwapCreatesMatch(board, x, y, x + 1, y))
                return true;
            if (y + 1 < Board.Height && SwapCreatesMatch(board, x, y, x, y + 1))
                return true;
        }

        return false;
    }

    public static bool HasAnyMatch(IBoard board)
    {
        for (var x = 0; x < Board.Width; x++)
        for (var y = 0; y < Board.Height; y++)
            if (HasMatchAt(board, x, y))
                return true;

        return false;
    }

    private static bool SwapCreatesMatch(IBoard board, int x1, int y1, int x2, int y2)
    {
        board.SwapCells(x1, y1, x2, y2);
        var createsMatch = HasMatchAt(board, x1, y1) || HasMatchAt(board, x2, y2);
        board.SwapCells(x1, y1, x2, y2);
        return createsMatch;
    }

    private static bool HasMatchAt(IBoard board, int x, int y)
    {
        var type = board[x, y].CellType;
        if (type == CellType.None)
            return false;

        var horizontal = 1 + CountRun(board, x, y, 1, 0, type) + CountRun(board, x, y, -1, 0, type);
        var vertical = 1 + CountRun(board, x, y, 0, 1, type) + CountRun(board, x, y, 0, -1, type);
        return horizontal >= 3 || vertical >= 3;
    }

    private static int CountRun(IBoard board, int x, int y, int dx, int dy, CellType type)
    {
        var count = 0;
        for (x += dx, y += dy; board.IsInBounds(x, y) && board[x, y].CellType == type; x += dx, y += dy)
            count++;

        return count;
    }
}
