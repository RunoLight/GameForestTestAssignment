using GameForestTestAssignment.Utils;

namespace GameForestTestAssignment.Game.Animations;

public class DisappearAnimation(float durationMs) : AnimationWithDuration(durationMs)
{
    public float GetScale()
    {
        return 1f - Easing.EaseInCubic(Progress);
    }

    public float GetAlpha()
    {
        return 1f - Easing.Linear(Progress);
    }
}