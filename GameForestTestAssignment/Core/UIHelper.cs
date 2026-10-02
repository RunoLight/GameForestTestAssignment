#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace GameForestTestAssignment.Core;

public static class UiHelper
{
    public static void DrawBorder(SpriteBatch spriteBatch, Texture2D whitePixel, Rectangle rect, Color color)
    {
        if (whitePixel == null)
            return;

        spriteBatch.Draw(whitePixel, new Rectangle(rect.X, rect.Y, rect.Width, 2), color);
        spriteBatch.Draw(whitePixel, new Rectangle(rect.X, rect.Bottom - 2, rect.Width, 2), color);
        spriteBatch.Draw(whitePixel, new Rectangle(rect.X, rect.Y, 2, rect.Height), color);
        spriteBatch.Draw(whitePixel, new Rectangle(rect.Right - 2, rect.Y, 2, rect.Height), color);
    }
}