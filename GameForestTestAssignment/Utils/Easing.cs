namespace GameForestTestAssignment.Utils;

public static class Easing
{
    public static float Linear(float t)
    {
        return t;
    }

    public static float EaseOutQuad(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }

    public static float EaseOutCubic(float t)
    {
        return 1f - (1f - t) * (1f - t) * (1f - t);
    }

    public static float EaseInCubic(float t)
    {
        return t * t * t;
    }
}