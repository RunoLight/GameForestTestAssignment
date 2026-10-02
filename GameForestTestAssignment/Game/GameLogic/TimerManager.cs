namespace GameForestTestAssignment.Game.GameLogic;

public class TimerManager(float totalTime)
{
    public float TimeRemaining { get; private set; } = totalTime;
    public bool IsExpired => TimeRemaining <= 0;

    public void Update(float deltaTime)
    {
        if (IsExpired)
            return;

        TimeRemaining -= deltaTime;
        if (TimeRemaining < 0)
            TimeRemaining = 0;
    }
}