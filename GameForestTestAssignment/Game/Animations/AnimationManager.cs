using System;
using System.Collections.Generic;

namespace GameForestTestAssignment.Game.Animations;

public class AnimationManager
{
    private readonly List<(IAnimation Animation, Action OnComplete)> _animations = [];
    private readonly List<Action> _completionCallbacks = [];

    public void Play(IAnimation animation, Action onComplete = null)
    {
        _animations.Add((animation, onComplete));
    }

    public void Stop(IAnimation animation)
    {
        _animations.RemoveAll(entry => entry.Animation == animation);
    }

    public bool IsPlaying<T>() where T : IAnimation
    {
        return _animations.Exists(entry => entry.Animation is T);
    }

    public void Update(float deltaTime)
    {
        _completionCallbacks.Clear();

        foreach (var (animation, onComplete) in _animations)
        {
            animation.Update(deltaTime);
            if (animation.IsComplete && onComplete != null)
                _completionCallbacks.Add(onComplete);
        }

        _animations.RemoveAll(entry => entry.Animation.IsComplete);

        foreach (var callback in _completionCallbacks)
            callback();
    }

    public void ApplyTo(BoardRenderState renderState)
    {
        foreach (var (animation, _) in _animations)
            animation.Apply(renderState);
    }
}
