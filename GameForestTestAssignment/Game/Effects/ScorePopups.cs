using System.Collections.Generic;
using GameForestTestAssignment.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameForestTestAssignment.Game.Effects;

public class ScorePopups
{
    private const float Lifetime = 0.7f;
    private const float RiseSpeed = 50f;
    private const float TextScale = 0.9f;

    private static readonly Color TextColor = Color.Gold;
    private static readonly Vector2 ShadowOffset = new(1f, 1f);

    private readonly List<Popup> _popups = [];

    public void Spawn(Vector2 position, int points)
    {
        _popups.Add(new Popup { Position = position, Text = $"+{points}", Age = 0f });
    }

    public void Update(float deltaTime)
    {
        for (var i = _popups.Count - 1; i >= 0; i--)
        {
            var popup = _popups[i];
            popup.Age += deltaTime;
            popup.Position.Y -= RiseSpeed * deltaTime;

            if (popup.Age >= Lifetime)
                _popups.RemoveAt(i);
            else
                _popups[i] = popup;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var font = PersistentResources.Font;

        foreach (var popup in _popups)
        {
            var alpha = 1f - popup.Age / Lifetime;
            var origin = font.MeasureString(popup.Text) / 2f;

            spriteBatch.DrawString(font, popup.Text, popup.Position + ShadowOffset, Color.Black * alpha,
                0f, origin, TextScale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, popup.Text, popup.Position, TextColor * alpha,
                0f, origin, TextScale, SpriteEffects.None, 0f);
        }
    }

    private struct Popup
    {
        public Vector2 Position;
        public string Text;
        public float Age;
    }
}