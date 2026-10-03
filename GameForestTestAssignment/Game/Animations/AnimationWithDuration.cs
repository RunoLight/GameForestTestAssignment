using System;

namespace GameForestTestAssignment.Game.Animations;

public abstract class AnimationWithDuration(float durationMs) : IAnimation
{
    private float _elapsed;

    protected float Progress { get; private set; }
    public bool IsComplete => _elapsed >= durationMs;

    public void Update(float deltaTime)
    {
        _elapsed = Math.Min(_elapsed + deltaTime, durationMs);
        Progress = _elapsed / durationMs;
    }

    public abstract void Apply(BoardRenderState renderState);
}
