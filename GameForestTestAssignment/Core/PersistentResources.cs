using System;
using System.Collections.Generic;
using GameForestTestAssignment.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameForestTestAssignment.Core;

public static class PersistentResources
{
    private const int ShapeTextureSize = 64;

    private static bool _isInitialized;

    public static Texture2D WhitePixel { get; private set; } = null!;
    public static SpriteFont Font { get; private set; } = null!;
    public static Dictionary<CellType, Texture2D> ShapeTextures { get; private set; } = null!;

    public static void Initialize(GraphicsDevice device, ContentManager content)
    {
        if (_isInitialized)
            return;

        _isInitialized = true;

        WhitePixel = new Texture2D(device, 1, 1);
        WhitePixel.SetData([Color.White]);

        Font = content.Load<SpriteFont>("Fonts");

        ShapeTextures = new Dictionary<CellType, Texture2D>();

        foreach (var type in Enum.GetValues<CellType>())
        {
            if (type == CellType.None)
                continue;
            ShapeTextures[type] = CreateShapeTexture(ShapeTextureSize, type, device);
        }
    }

    public static void Dispose()
    {
        WhitePixel?.Dispose();
        WhitePixel = null!;

        if (ShapeTextures != null)
        {
            foreach (var tex in ShapeTextures.Values)
                tex.Dispose();
            ShapeTextures.Clear();
            ShapeTextures = null!;
        }

        _isInitialized = false;

        // Font not disposed here - managed by ContentManager
    }

    private static Texture2D CreateShapeTexture(int size, CellType cellType, GraphicsDevice device)
    {
        var texture = new Texture2D(device, size, size);
        var colors = new Color[size * size];

        for (var i = 0; i < colors.Length; i++)
            colors[i] = Color.Transparent;

        var cx = size / 2f;
        var cy = size / 2f;
        var r = size / 2f - 2;

        for (var y = 0; y < size; y++)
        for (var x = 0; x < size; x++)
        {
            var dx = x - cx;
            var dy = y - cy;
            var dist = (float)Math.Sqrt(dx * dx + dy * dy);

            var inside = cellType switch
            {
                CellType.Circle => dist <= r,
                CellType.Square => Math.Abs(dx) <= r * 0.8f && Math.Abs(dy) <= r * 0.8f,
                CellType.Triangle => IsInsideTriangle(dx, dy, r),
                CellType.Diamond => Math.Abs(dx) + Math.Abs(dy) <= r * 1.1f,
                CellType.Star => IsInsideStar(dx, dy, r),
                _ => false
            };

            if (inside)
            {
                var edge = 1f - Math.Max(0, (dist - (r - 3)) / 3f);
                colors[y * size + x] = Color.White * (0.75f + 0.25f * edge);
            }
        }

        texture.SetData(colors);
        return texture;
    }

    private static bool IsInsideTriangle(float dx, float dy, float r)
    {
        var h = r * (float)Math.Sqrt(3) / 2;
        return dy <= h * 0.5f && dy >= -h &&
               (Math.Abs(dx) <= (h + dy) / (float)Math.Sqrt(3) ||
                Math.Abs(dx) <= (h - dy * 2) / (2f * (float)Math.Sqrt(3)));
    }

    private static bool IsInsideStar(float dx, float dy, float r)
    {
        var angle = (float)Math.Atan2(dy, dx);
        var dist = (float)Math.Sqrt(dx * dx + dy * dy);
        var starR = r * (0.5f + 0.5f * (float)Math.Cos(5 * angle));
        return dist <= starR;
    }
}