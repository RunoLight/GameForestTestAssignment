using System;
using GameForestTestAssignment.Game.Bonuses;
using GameForestTestAssignment.Game.MatchDetection;

namespace GameForestTestAssignment.Game;

public interface IBoard
{
    Cell this[int x, int y] { get; }
    bool IsInBounds(int x, int y);
    void SwapCells(int x1, int y1, int x2, int y2);
    void MoveCell(int fromX, int fromY, int toX, int toY);
    void Shuffle();
}

public class Board : IBoard
{
    public const int Width = 8;
    public const int Height = 8;

    private const int MaxShuffleAttempts = 100;

    private readonly Cell[,] _cells = new Cell[Width, Height];

    public Board()
    {
        for (var x = 0; x < Width; x++)
        for (var y = 0; y < Height; y++)
            _cells[x, y] = new Cell();
    }

    public Cell this[int x, int y] => _cells[x, y];

    public void SwapCells(int x1, int y1, int x2, int y2)
    {
        var tempType = _cells[x1, y1].CellType;
        var tempBonus = _cells[x1, y1].Bonus;

        _cells[x1, y1].CellType = _cells[x2, y2].CellType;
        _cells[x1, y1].Bonus = _cells[x2, y2].Bonus;

        _cells[x2, y2].CellType = tempType;
        _cells[x2, y2].Bonus = tempBonus;
    }

    public void MoveCell(int fromX, int fromY, int toX, int toY)
    {
        if (fromX == toX && fromY == toY)
            return;

        _cells[toX, toY].CellType = _cells[fromX, fromY].CellType;
        _cells[toX, toY].Bonus = _cells[fromX, fromY].Bonus;
        _cells[fromX, fromY].CellType = CellType.None;
        _cells[fromX, fromY].Bonus = null;
    }

    public bool IsInBounds(int x, int y)
    {
        return x is >= 0 and < Width && y is >= 0 and < Height;
    }

    public void Shuffle()
    {
        for (var attempt = 0; attempt < MaxShuffleAttempts; attempt++)
        {
            PermuteCells();
            if (!MoveFinder.HasAnyMatch(this) && MoveFinder.HasPossibleMove(this))
                return;
        }

        GenerateRandomBoard();
    }

    public void GenerateRandomBoard()
    {
        do
        {
            FillWithoutMatches();
        } while (!MoveFinder.HasPossibleMove(this));
    }

    private void PermuteCells()
    {
        var contents = new (CellType Type, Bonus Bonus)[Width * Height];
        var i = 0;
        foreach (var cell in _cells)
            contents[i++] = (cell.CellType, cell.Bonus);

        Random.Shared.Shuffle(contents);

        i = 0;
        foreach (var cell in _cells)
            (cell.CellType, cell.Bonus) = contents[i++];
    }

    private void FillWithoutMatches()
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

            _cells[x, y].CellType = type;
        }
    }

    private void Clear()
    {
        foreach (var cell in _cells)
            cell.Reset();
    }

    private bool WouldCreateMatch(int x, int y, CellType type)
    {
        // Check horizontal
        var hCount = 1;
        for (var dx = -1; dx >= -2; dx--)
            if (x + dx >= 0 && _cells[x + dx, y].CellType == type)
                hCount++;
            else
                break;

        if (hCount >= 3)
            return true;

        // Check vertical
        var vCount = 1;
        for (var dy = -1; dy >= -2; dy--)
            if (y + dy >= 0 && _cells[x, y + dy].CellType == type)
                vCount++;
            else
                break;

        return vCount >= 3;
    }
}