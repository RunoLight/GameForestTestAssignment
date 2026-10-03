using System;
using Microsoft.Xna.Framework;

namespace GameForestTestAssignment.Game;

public enum CellType
{
    None = 0,
    Circle,
    Square,
    Triangle,
    Diamond,
    Star
}

public static class CellTypeExtensions
{
    private static readonly CellType[] AllTypes =
    [
        CellType.Circle, CellType.Square,
        CellType.Triangle, CellType.Diamond,
        CellType.Star
    ];

    private static readonly Random Random = new();

    public static Color GetColor(this CellType type)
    {
        return type switch
        {
            CellType.Circle => Color.Red,
            CellType.Square => Color.Green,
            CellType.Triangle => Color.Blue,
            CellType.Diamond => Color.Yellow,
            CellType.Star => Color.Purple,
            _ => Color.Transparent
        };
    }

    public static CellType RandomType()
    {
        return AllTypes[Random.Next(AllTypes.Length)];
    }
}