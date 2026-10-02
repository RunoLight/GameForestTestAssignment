#region

using System;
using GameForestTestAssignment.Utils;

#endregion

namespace GameForestTestAssignment.Game.Animations;

public class FallAnimation(float startY, float endY, float durationMs, Action<float> onComplete = null)
    : AnimationWithDuration(durationMs)
{
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if (IsComplete) onComplete?.Invoke(Progress);
    }

    public float GetCurrentY()
    {
        var t = Easing.EaseOutCubic(Progress);
        return startY + (endY - startY) * t;
    }
}