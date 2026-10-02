#region

using System;
using GameForestTestAssignment.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace GameForestTestAssignment.Core.ScreenSystem;

public class MainMenuScreen : Screen
{
    private const int ButtonWidth = 200;
    private const int ButtonHeight = 60;

    private readonly Button _playButton = new() { Text = "PLAY" };

    private MouseState _previousMouse;
    private float _titlePulse;

    public override void OnEnter()
    {
        _previousMouse = Mouse.GetState();
        RecalculateLayout();
    }

    public override void Update(GameTime gameTime)
    {
        _titlePulse += (float)gameTime.ElapsedGameTime.TotalSeconds * 2f;

        var mouse = Mouse.GetState();
        if (_playButton.Update(mouse, _previousMouse))
            ScreenNavigation.PushScreen(new GameScreen());

        _previousMouse = mouse;
    }

    public override void Draw()
    {
        SpriteBatch.Begin();

        DrawBackgroundGradient(Viewport);
        DrawTitle(Viewport);
        DrawSubtitle(Viewport);

        _playButton.Draw(SpriteBatch, Font);

        SpriteBatch.End();
    }

    private void RecalculateLayout()
    {
        _playButton.SetPosition(
            Viewport.Width / 2, (int)(Viewport.Height / 2f + 50),
            ButtonWidth, ButtonHeight
        );
    }

    private void DrawBackgroundGradient(Viewport vp)
    {
        var top = new Color(15, 15, 35);
        var bottom = new Color(25, 25, 55);

        for (var y = 0; y < vp.Height; y += 4)
        {
            var t = (float)y / vp.Height;
            var color = Color.Lerp(top, bottom, t);
            SpriteBatch.Draw(PersistentResources.WhitePixel,
                new Rectangle(0, y, vp.Width, 4), color);
        }
    }

    private void DrawTitle(Viewport vp)
    {
        const float baseScale = 2.5f;
        var scale = baseScale + (float)Math.Sin(_titlePulse) * 0.1f;
        var size = Font.MeasureString("MATCH-3") * scale;

        SpriteBatch.DrawString(Font, "MATCH-3",
            new Vector2(vp.Width / 2f - size.X / 2f, vp.Height * 0.3f),
            Color.White * 0.9f, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f
        );
    }

    private void DrawSubtitle(Viewport vp)
    {
        const string text = "Match Three Game";
        var size = Font.MeasureString(text);

        SpriteBatch.DrawString(Font, text,
            new Vector2(vp.Width / 2f - size.X / 2f, vp.Height * 0.3f + 60),
            Color.White * 0.5f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f
        );
    }
}