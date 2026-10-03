#region

using GameForestTestAssignment.Core;
using GameForestTestAssignment.Game.Bonuses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace GameForestTestAssignment.Game;

public class BoardRenderer(SpriteBatch spriteBatch, int textureSize = 64)
{
    private static readonly Color BackgroundColor1 = new(60, 60, 80);
    private static readonly Color BackgroundColor2 = new(50, 50, 70);

    public void DrawBoard(Board board)
    {
        DrawBoardBackground(board);
        DrawCells(board);
    }

    public void DrawDestroyer(Vector2 center, Color color, int cellSize)
    {
        var radius = cellSize * 0.28f;
        spriteBatch.Draw(PersistentResources.ShapeTextures[CellType.Circle], center, null, color, 0f,
            new Vector2(textureSize / 2f), radius * 2f / textureSize, SpriteEffects.None, 0f);
        spriteBatch.Draw(PersistentResources.ShapeTextures[CellType.Circle], center, null, Color.White * 0.7f, 0f,
            new Vector2(textureSize / 2f), radius * 0.8f / textureSize, SpriteEffects.None, 0f);
    }

    private void DrawBoardBackground(IBoard board)
    {
        for (var x = 0; x < Board.Width; x++)
        for (var y = 0; y < Board.Height; y++)
        {
            var pos = GetCellPosition(board, x, y);
            var bgColor = (x + y) % 2 == 0 ? BackgroundColor1 : BackgroundColor2;
            spriteBatch.Draw(PersistentResources.WhitePixel,
                new Rectangle((int)pos.X, (int)pos.Y, board.CellSize, board.CellSize), bgColor);
        }
    }

    private void DrawCells(Board board)
    {
        var texScaleBase = board.CellSize / (float)textureSize;

        for (var x = 0; x < Board.Width; x++)
        for (var y = 0; y < Board.Height; y++)
        {
            var cell = board[x, y];
            if (cell.IsEmpty)
                continue;

            var render = board.RenderState[x, y];
            var tex = PersistentResources.ShapeTextures[cell.CellType];
            var origin = new Vector2(textureSize / 2f);
            var center = GetCellPosition(board, x, y) + new Vector2(board.CellSize / 2f) + render.Offset;
            var scale = texScaleBase * render.Scale;
            var tint = cell.CellType.GetColor() * render.Alpha;

            spriteBatch.Draw(tex, center, null, tint, render.Rotation, origin, scale, SpriteEffects.None, 0f);

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
                spriteBatch.Draw(PersistentResources.WhitePixel,
                    new Rectangle((int)(center.X - size * 0.38f), (int)(center.Y - size * 0.08f),
                        (int)(size * 0.76f), (int)(size * 0.16f)), accent);
                break;
            case LineBonus { Orientation: BonusOrientation.Vertical }:
                spriteBatch.Draw(PersistentResources.WhitePixel,
                    new Rectangle((int)(center.X - size * 0.08f), (int)(center.Y - size * 0.38f),
                        (int)(size * 0.16f), (int)(size * 0.76f)), accent);
                break;
            case BombBonus:
                var bombRadius = size * 0.18f;
                spriteBatch.Draw(PersistentResources.ShapeTextures[CellType.Circle], center, null, accent, 0f,
                    new Vector2(textureSize / 2f), bombRadius * 2f / textureSize, SpriteEffects.None, 0f);
                break;
        }
    }

    private static Vector2 GetCellPosition(IBoard board, int x, int y)
    {
        return board.BoardPosition + new Vector2(x * board.CellSize, y * board.CellSize);
    }
}