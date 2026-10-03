#region

using System;
using System.Collections.Generic;
using GameForestTestAssignment.Core;
using GameForestTestAssignment.Game.Bonuses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace GameForestTestAssignment.Game;

public class BoardRenderer
{
    private static readonly Color BackgroundColor1 = new(60, 60, 80);
    private static readonly Color BackgroundColor2 = new(50, 50, 70);

    private readonly Dictionary<CellType, Texture2D> _shapeTextures;
    private readonly SpriteBatch _spriteBatch;
    private readonly int _textureSize;

    public BoardRenderer(SpriteBatch spriteBatch, int textureSize = 64)
    {
        _spriteBatch = spriteBatch;
        _textureSize = textureSize;
        _shapeTextures = new Dictionary<CellType, Texture2D>();

        foreach (var type in Enum.GetValues<CellType>())
        {
            if (type == CellType.None)
                continue;
            _shapeTextures[type] = CreateShapeTexture(textureSize, type);
        }
    }

    private GraphicsDevice GraphicsDevice => _spriteBatch.GraphicsDevice;

    public void DrawBoard(Board board)
    {
        DrawBoardBackground(board);
        DrawCells(board);
    }

    public void DrawDestroyer(Vector2 center, Color color, int cellSize)
    {
        var radius = cellSize * 0.28f;
        _spriteBatch.Draw(_shapeTextures[CellType.Circle], center, null, color, 0f,
            new Vector2(_textureSize / 2f), radius * 2f / _textureSize, SpriteEffects.None, 0f);
        _spriteBatch.Draw(_shapeTextures[CellType.Circle], center, null, Color.White * 0.7f, 0f,
            new Vector2(_textureSize / 2f), radius * 0.8f / _textureSize, SpriteEffects.None, 0f);
    }

    private Texture2D CreateShapeTexture(int size, CellType cellType)
    {
        var texture = new Texture2D(GraphicsDevice, size, size);
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

    private void DrawBoardBackground(IBoard board)
    {
        for (var x = 0; x < Board.Width; x++)
        for (var y = 0; y < Board.Height; y++)
        {
            var pos = GetCellPosition(board, x, y);
            var bgColor = (x + y) % 2 == 0 ? BackgroundColor1 : BackgroundColor2;
            _spriteBatch.Draw(PersistentResources.WhitePixel,
                new Rectangle((int)pos.X, (int)pos.Y, board.CellSize, board.CellSize), bgColor);
        }
    }

    private void DrawCells(Board board)
    {
        var texScaleBase = board.CellSize / (float)_textureSize;

        for (var x = 0; x < Board.Width; x++)
        for (var y = 0; y < Board.Height; y++)
        {
            var cell = board[x, y];
            if (cell.IsEmpty)
                continue;

            var render = board.RenderState[x, y];
            var tex = _shapeTextures[cell.CellType];
            var origin = new Vector2(_textureSize / 2f);
            var center = GetCellPosition(board, x, y) + new Vector2(board.CellSize / 2f) + render.Offset;
            var scale = texScaleBase * render.Scale;
            var tint = cell.CellType.GetColor() * render.Alpha;

            _spriteBatch.Draw(tex, center, null, tint, render.Rotation, origin, scale, SpriteEffects.None, 0f);

            if (cell.Bonus != null)
                DrawBonusIndicator(board, center, cell, x, y);
        }
    }

    private void DrawBonusIndicator(Board board, Vector2 center, Cell cell, int x, int y)
    {
        var render = board.RenderState[x, y];
        var accent = Color.White * (0.85f * render.Alpha);
        var size = board.CellSize * render.Scale;

        switch (cell.Bonus)
        {
            case LineBonus { Orientation: BonusOrientation.Horizontal }:
                _spriteBatch.Draw(PersistentResources.WhitePixel,
                    new Rectangle((int)(center.X - size * 0.38f), (int)(center.Y - size * 0.08f),
                        (int)(size * 0.76f), (int)(size * 0.16f)), accent);
                break;
            case LineBonus { Orientation: BonusOrientation.Vertical }:
                _spriteBatch.Draw(PersistentResources.WhitePixel,
                    new Rectangle((int)(center.X - size * 0.08f), (int)(center.Y - size * 0.38f),
                        (int)(size * 0.16f), (int)(size * 0.76f)), accent);
                break;
            case BombBonus:
                var bombRadius = size * 0.18f;
                _spriteBatch.Draw(_shapeTextures[CellType.Circle], center, null, accent, 0f,
                    new Vector2(_textureSize / 2f), bombRadius * 2f / _textureSize, SpriteEffects.None, 0f);
                break;
        }
    }

    private static Vector2 GetCellPosition(IBoard board, int x, int y)
    {
        return board.BoardPosition + new Vector2(x * board.CellSize, y * board.CellSize);
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