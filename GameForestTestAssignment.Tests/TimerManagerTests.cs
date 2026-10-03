using GameForestTestAssignment.Game.GameLogic;

namespace GameForestTestAssignment.Tests;

public class TimerManagerTests
{
    [Test]
    public void CountsDownInSeconds()
    {
        var timer = new TimerManager(60f);

        timer.Update(1.5f);

        Assert.That(timer.TimeRemaining, Is.EqualTo(58.5f));
        Assert.That(timer.IsExpired, Is.False);
    }

    [Test]
    public void StopsAtZeroAndExpires()
    {
        var timer = new TimerManager(1f);

        timer.Update(5f);

        Assert.That(timer.TimeRemaining, Is.Zero);
        Assert.That(timer.IsExpired, Is.True);
    }
}
