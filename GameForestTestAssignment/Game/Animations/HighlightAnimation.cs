using System;

namespace GameForestTestAssignment.Game.Animations;

public class HighlightAnimation : IAnimation
{
    private float _elapsed;

    public bool IsComplete => false;

    public void Update(float deltaTime)
    {
        _elapsed += deltaTime;
    }

    public float GetRotation()
    {
        return (float)Math.Sin(_elapsed * 0.003f) * 0.3f;
    }

    public float GetScalePulse()
    {
        return 1f + (float)Math.Sin(_elapsed * 0.005f) * 0.08f;
    }
}