using System;
using Microsoft.Xna.Framework;

namespace GameForestTestAssignment.Game.Effects;

public class ScreenShake
{
    private const float Duration = 0.25f;
    private const float Amplitude = 6f;

    private float _remaining;

    public Vector2 Offset { get; private set; }

    public void Start()
    {
        _remaining = Duration;
    }

    public void Update(float deltaTime)
    {
        if (_remaining <= 0f)
        {
            Offset = Vector2.Zero;
            return;
        }

        _remaining = Math.Max(0f, _remaining - deltaTime);
        var strength = Amplitude * (_remaining / Duration);
        Offset = new Vector2(
            MathF.Round((Random.Shared.NextSingle() * 2f - 1f) * strength),
            MathF.Round((Random.Shared.NextSingle() * 2f - 1f) * strength));
    }
}