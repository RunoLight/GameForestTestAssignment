#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace GameForestTestAssignment.Core.Components;

public sealed class Button
{
    private const float TextScale = 1.2f;
    private const int HoverGrow = 3;

    private static readonly Color FillIdle = new(0, 128, 0);
    private static readonly Color FillHover = new(144, 238, 144);
    private static readonly Color BorderIdle = new(144, 238, 144);
    private static readonly Color BorderHover = Color.White;

    private Rectangle _idleRect;
    private bool _isHovering;

    public string Text { get; init; } = "";

    public void SetPosition(int centerX, int topY, int width, int height)
    {
        _idleRect = new Rectangle(centerX - width / 2, topY, width, height);
    }

    /// <returns>True if clicked</returns>
    public bool Update(MouseState current, MouseState previous)
    {
        _isHovering = _idleRect.Contains(current.Position);

        var clicked = _isHovering &&
                      current.LeftButton == ButtonState.Pressed &&
                      previous.LeftButton == ButtonState.Released;

        return clicked;
    }

    public void Draw(SpriteBatch sb, SpriteFont font)
    {
        var grow = _isHovering ? HoverGrow : 0;
        var hoverRect = new Rectangle(
            _idleRect.X - grow,
            _idleRect.Y - grow,
            _idleRect.Width + grow * 2,
            _idleRect.Height + grow * 2
        );

        var fillColor = _isHovering ? FillHover : FillIdle;
        sb.Draw(PersistentResources.WhitePixel, hoverRect, fillColor * 0.8f);

        var borderColor = _isHovering ? BorderHover : BorderIdle;
        UiHelper.DrawBorder(sb, PersistentResources.WhitePixel, hoverRect, borderColor);

        DrawCenteredText(sb, font, hoverRect);
    }

    private void DrawCenteredText(SpriteBatch sb, SpriteFont font, Rectangle rect)
    {
        var size = font.MeasureString(Text) * TextScale;
        var pos = new Vector2(
            rect.X + (rect.Width - size.X) / 2f,
            rect.Y + (rect.Height - size.Y) / 2f
        );

        sb.DrawString(
            font, Text, pos, Color.White, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f
        );
    }
}