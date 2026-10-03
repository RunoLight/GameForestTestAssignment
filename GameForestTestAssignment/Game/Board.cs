using Microsoft.Xna.Framework;

namespace GameForestTestAssignment.Game;

public interface IBoard
{
    Cell this[int x, int y] { get; }
    public int CellSize { get; }
    public Vector2 BoardPosition { get; }
    bool IsInBounds(int x, int y);
    void SwapCells(int x1, int y1, int x2, int y2);
    void MoveCell(int fromX, int fromY, int toX, int toY);
    void GenerateRandomBoard();
    public Vector2 GetCellCenter(int x, int y);
}

public class Board : IBoard
{
    public Board()
    {
        Grid = new Cell[Width, Height];
        for (var x = 0; x < Width; x++)
        for (var y = 0; y < Height; y++)
            Grid[x, y] = new Cell();

        RenderState = new BoardRenderState(Width, Height);
    }

    private Cell[,] Grid { get; }

    public static int Width => 8;
    public static int Height => 8;
    public BoardRenderState RenderState { get; }
    public int CellSize { get; init; } = 64;
    public Vector2 BoardPosition { get; init; } = Vector2.Zero;

    public Cell this[int x, int y] => Grid[x, y];

    public void GenerateRandomBoard()
    {
        Clear();
        for (var x = 0; x < Width; x++)
        for (var y = 0; y < Height; y++)
        {
            CellType type;
            do
            {
                type = CellTypeExtensions.RandomType();
            } while (WouldCreateMatch(x, y, type));

            Grid[x, y].CellType = type;
        }
    }

    public void SwapCells(int x1, int y1, int x2, int y2)
    {
        var tempType = Grid[x1, y1].CellType;
        var tempBonus = Grid[x1, y1].Bonus;

        Grid[x1, y1].CellType = Grid[x2, y2].CellType;
        Grid[x1, y1].Bonus = Grid[x2, y2].Bonus;

        Grid[x2, y2].CellType = tempType;
        Grid[x2, y2].Bonus = tempBonus;
    }

    public void MoveCell(int fromX, int fromY, int toX, int toY)
    {
        if (fromX == toX && fromY == toY)
            return;

        Grid[toX, toY].CellType = Grid[fromX, fromY].CellType;
        Grid[toX, toY].Bonus = Grid[fromX, fromY].Bonus;
        Grid[fromX, fromY].CellType = CellType.None;
        Grid[fromX, fromY].Bonus = null;
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public Vector2 GetCellCenter(int x, int y)
    {
        return BoardPosition + new Vector2(
            x * CellSize + CellSize * 0.5f,
            y * CellSize + CellSize * 0.5f);
    }

    public void ResetRenderStates()
    {
        RenderState.Reset();
    }

    private void Clear()
    {
        for (var x = 0; x < Width; x++)
        for (var y = 0; y < Height; y++)
        {
            Grid[x, y] ??= new Cell();
            Grid[x, y].Reset();
        }
    }

    private bool WouldCreateMatch(int x, int y, CellType type)
    {
        // Check horizontal
        var hCount = 1;
        for (var dx = -1; dx >= -2; dx--)
            if (x + dx >= 0 && Grid[x + dx, y].CellType == type)
                hCount++;
            else
                break;

        if (hCount >= 3)
            return true;

        // Check vertical
        var vCount = 1;
        for (var dy = -1; dy >= -2; dy--)
            if (y + dy >= 0 && Grid[x, y + dy].CellType == type)
                vCount++;
            else
                break;

        return vCount >= 3;
    }
}