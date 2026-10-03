using Microsoft.Xna.Framework;

namespace GameForestTestAssignment.Game;

public sealed class BoardLayout(int cellSize, Vector2 position)
{
    public int CellSize { get; } = cellSize;
    public Vector2 Position { get; } = position;

    public Vector2 GetCellTopLeft(int x, int y)
    {
        return Position + new Vector2(x, y) * CellSize;
    }

    public Vector2 GetCellCenter(float x, float y)
    {
        return Position + new Vector2(x + 0.5f, y + 0.5f) * CellSize;
    }
}
