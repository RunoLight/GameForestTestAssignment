using System;
using GameForestTestAssignment.Utils;
using Microsoft.Xna.Framework;

namespace GameForestTestAssignment.Game.Animations;

public class SwapAnimation(Vector2 startPos, Vector2 endPos, float durationMs, Action<float> onComplete = null)
    : AnimationWithDuration(durationMs)
{
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if (IsComplete)
            onComplete?.Invoke(Progress);
    }

    public Vector2 GetCurrentPosition()
    {
        var t = Easing.EaseOutQuad(Progress);
        return Vector2.Lerp(startPos, endPos, t);
    }
}