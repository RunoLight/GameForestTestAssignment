using GameForestTestAssignment.Game.Animations;

namespace GameForestTestAssignment.Tests;

public class AnimationManagerTests
{
    [Test]
    public void Callback_FiresOnceWhenAnimationCompletes()
    {
        var manager = new AnimationManager();
        var calls = 0;
        manager.Play(new DisappearAnimation(0, 0, 0.5f), () => calls++);

        manager.Update(0.3f);
        Assert.That(calls, Is.Zero);
        Assert.That(manager.IsPlaying<DisappearAnimation>(), Is.True);

        manager.Update(0.3f);
        manager.Update(0.3f);
        Assert.That(calls, Is.EqualTo(1));
        Assert.That(manager.IsPlaying<DisappearAnimation>(), Is.False);
    }

    [Test]
    public void IsPlaying_ChecksAnimationType()
    {
        var manager = new AnimationManager();

        manager.Play(new FallAnimation(0, 0, -1f, 0.5f));

        Assert.That(manager.IsPlaying<FallAnimation>(), Is.True);
        Assert.That(manager.IsPlaying<SwapAnimation>(), Is.False);
    }

    [Test]
    public void Stop_RemovesAnimationWithoutCallback()
    {
        var manager = new AnimationManager();
        var highlight = new HighlightAnimation(0, 0);
        var called = false;
        manager.Play(highlight, () => called = true);

        manager.Stop(highlight);
        manager.Update(1f);

        Assert.That(manager.IsPlaying<HighlightAnimation>(), Is.False);
        Assert.That(called, Is.False);
    }
}
