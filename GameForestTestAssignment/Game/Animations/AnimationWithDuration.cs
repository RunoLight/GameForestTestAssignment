using System;

namespace GameForestTestAssignment.Game.Animations;

public abstract class AnimationWithDuration(float duration) : IAnimation
{
    private float _elapsed;

    protected float Progress { get; private set; }
    public bool IsComplete => _elapsed >= duration;

    public void Update(float deltaTime)
    {
        _elapsed = Math.Min(_elapsed + deltaTime, duration);
        Progress = _elapsed / duration;
    }

    public abstract void Apply(BoardRenderState renderState);
}
