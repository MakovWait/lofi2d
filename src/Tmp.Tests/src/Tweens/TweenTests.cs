using Tmp.Animation;
using Tmp.Core.Comp;
using Tmp.Time;

namespace Tmp.Tests.Tweens;

public class TweenTests
{
    [Test]
    public void CompleteAllStepsPerTick()
    {
        var time = new FrameTime(null);
        var tween = new Tween();

        var stepsFinished = new List<int>();
        tween.StepFinished.Connect(stepIdx => stepsFinished.Add(stepIdx));

        tween = tween.Chain();
        tween.TweenInterval(0.1f);
        tween.TweenInterval(0.2f);
        tween.TweenCallback(() => Console.WriteLine("callback!"));
        
        time.Tick(1f);
        var finished = tween.Step(time);
        
        Assert.That(finished, Is.True);
        Assert.That(stepsFinished, Is.EquivalentTo(new List<int> {0, 1, 2}));
    }
    
    [Test]
    public void TestLoops()
    {
        var time = new FrameTime(null);
        var tween = new Tween();

        var stepsFinished = new List<int>();
        tween.StepFinished.Connect(stepIdx => stepsFinished.Add(stepIdx));
        
        tween.SetLoops(2);
        Assert.That(tween.LoopsLeft, Is.EqualTo(2));

        tween = tween.Chain();
        tween.TweenInterval(0.1f);

        time.Tick(0.1f);
        var finished = tween.Step(time);
        Assert.That(finished, Is.False);

        time.Tick(0.1f);
        finished = tween.Step(time);
        Assert.That(finished, Is.True);

        Assert.That(tween.LoopsLeft, Is.EqualTo(0));
        Assert.That(stepsFinished, Is.EquivalentTo(new List<int> {0, 0}));
    }
    
    [Test]
    public void TestLoopsUnlimited()
    {
        var time = new FrameTime(null);
        var tween = new Tween();

        var stepsFinished = new List<int>();
        tween.StepFinished.Connect(stepIdx => stepsFinished.Add(stepIdx));
        
        tween.SetLoops(0);
        tween = tween.Chain();
        tween.TweenInterval(0.1f);
        
        time.Tick(0.1f);
        var finished = tween.Step(time);
        Assert.That(finished, Is.False);

        time.Tick(0.1f);
        finished = tween.Step(time);
        Assert.That(finished, Is.False);
        
        Assert.That(stepsFinished, Is.EquivalentTo(new List<int> {0, 0}));
    }
    
    [Test]
    public void TestInfiniteLoopsCheck()
    {
        var time = new FrameTime(null);
        var tween = new Tween();

        tween.SetLoops(0);
        Assert.That(tween.LoopsLeft, Is.EqualTo(-1));
        tween = tween.Chain();
        tween.TweenCallback(() => {});
        
        time.Tick(0.1f);
        Assert.Throws<Exception>(
            () => tween.Step(time),
            "Infinite loop detected. Check SetLoops() description for more info."
        );
    }
    
    [Test]
    public void TestSpeedScale()
    {
        var time = new FrameTime(null);
        var tween = new Tween();

        tween.SetSpeedScale(2);
        tween = tween.Chain();
        tween.TweenInterval(0.1f);
        
        time.Tick(0.05f);
        var finished = tween.Step(time);
        Assert.That(finished, Is.True);
    }
    
    [Test]
    public void TestTweenRange()
    {
        var time = new FrameTime(null);
        var tween = new Tween();

        float? lastRange = null;
        
        tween = tween.Chain();
        tween.TweenRange(1f, x =>
        {
            lastRange = x;
        });
        
        time.Tick(0.5f);
        tween.Step(time);
        Assert.That(lastRange, Is.EqualTo(0.5));

        time.Tick(0.25f);
        tween.Step(time);
        Assert.That(lastRange, Is.EqualTo(0.75));

        time.Tick(0.25f);
        tween.Step(time);
        Assert.That(lastRange, Is.EqualTo(1f));
    }
    
    [Test]
    public void TestTweenRangeWithTimeOverflow()
    {
        var time = new FrameTime(null);
        var tween = new Tween();

        float? lastRange = null;
        
        tween = tween.Chain();
        tween.TweenRange(1f, x =>
        {
            lastRange = x;
        });
        
        time.Tick(1.5f);
        tween.Step(time);
        Assert.That(lastRange, Is.EqualTo(1));
    }
}