#region

using System;

#endregion

namespace GameForestTestAssignment.Game.Animations;

public class AnimationWithDuration(float durationMs) : IAnimation
{
    private float _elapsed;

    public float Progress { get; private set; }
    public bool IsComplete => _elapsed >= durationMs;

    public virtual void Update(float deltaTime)
    {
        _elapsed = Math.Min(_elapsed + deltaTime, durationMs);
        Progress = _elapsed / durationMs;
    }
}