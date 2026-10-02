#region

using GameForestTestAssignment.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace GameForestTestAssignment.Core.ScreenSystem;

public class GameOverScreen(int score) : Screen
{
    private const int ButtonWidth = 200;
    private const int ButtonHeight = 60;

    private readonly Button _okButton = new() { Text = "OK" };

    private MouseState _previousMouse;

    public override void OnEnter()
    {
        _previousMouse = Mouse.GetState();
    }

    public override void LoadContent(SpriteBatch spriteBatch, Viewport viewport, IScreenNavigation screenNavigation)
    {
        base.LoadContent(spriteBatch, viewport, screenNavigation);
        RecalculateLayout();
    }

    public override void Update(GameTime gameTime)
    {
        RecalculateLayout();

        var mouse = Mouse.GetState();
        if (_okButton.Update(mouse, _previousMouse))
            ScreenNavigation.PopToFirstScreen();

        _previousMouse = mouse;
    }

    public override void Draw()
    {
        SpriteBatch.Begin();

        SpriteBatch.Draw(
            PersistentResources.WhitePixel,
            new Rectangle(0, 0, Viewport.Width, Viewport.Height),
            new Color(0, 0, 0, 150)
        );

        DrawCenteredText("GAME OVER", Viewport.Width / 2f, Viewport.Height * 0.30f, Color.Orange, 3f);
        DrawCenteredText($"Score: {score}", Viewport.Width / 2f, Viewport.Height * 0.45f, Color.White, 1.5f);

        _okButton.Draw(SpriteBatch, Font);

        SpriteBatch.End();
    }

    private void RecalculateLayout()
    {
        _okButton.SetPosition(
            Viewport.Width / 2, (int)(Viewport.Height * 0.65f),
            ButtonWidth, ButtonHeight
        );
    }

    private void DrawCenteredText(string text, float centerX, float topY, Color color, float scale)
    {
        var size = Font.MeasureString(text) * scale;
        SpriteBatch.DrawString(
            Font, text, new Vector2(centerX - size.X / 2f, topY),
            color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f
        );
    }
}