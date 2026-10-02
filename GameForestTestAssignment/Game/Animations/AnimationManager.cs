#region

using System.Collections.Generic;

#endregion

namespace GameForestTestAssignment.Game.Animations;

public class AnimationManager
{
    private readonly List<IAnimation> _animations = [];
    private readonly List<IAnimation> _toRemove = [];

    public void AddAnimation(IAnimation animation)
    {
        _animations.Add(animation);
    }

    public void Update(float deltaTime)
    {
        _toRemove.Clear();

        foreach (var anim in _animations)
        {
            anim.Update(deltaTime);
            if (anim.IsComplete) _toRemove.Add(anim);
        }

        foreach (var anim in _toRemove)
            _animations.Remove(anim);
    }
}