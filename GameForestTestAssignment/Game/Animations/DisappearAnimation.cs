using System;
using GameForestTestAssignment.Utils;

namespace GameForestTestAssignment.Game.Animations;

public class DisappearAnimation(int x, int y, float durationMs) : AnimationWithDuration(durationMs)
{
    public override void Apply(BoardRenderState renderState)
    {
        renderState.SetScale(x, y, Math.Max(0.01f, 1f - Easing.EaseInCubic(Progress)));
        renderState.SetAlpha(x, y, 1f - Easing.Linear(Progress));
    }
}
