using System;

namespace GameForestTestAssignment.Game.Animations;

public class HighlightAnimation(int x, int y) : IAnimation
{
    private float _elapsed;

    public bool IsComplete => false;

    public void Update(float deltaTime)
    {
        _elapsed += deltaTime;
    }

    public void Apply(BoardRenderState renderState)
    {
        renderState.SetRotation(x, y, (float)Math.Sin(_elapsed * 3f) * 0.3f);
        renderState.SetScale(x, y, 1f + (float)Math.Sin(_elapsed * 5f) * 0.08f);
    }
}
