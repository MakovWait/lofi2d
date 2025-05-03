using Lofi.Core.Comp;

namespace Lofi.Time;

public class Stopwatch
{
    public float Elapsed => _elapsed;

    /// seconds 
    private float _elapsed;

    public void Tick(float delta)
    {
        _elapsed += delta;
    }

    public void Reset()
    {
        _elapsed = 0;
    }
}

public static class StopwatchEx
{
    public static Stopwatch UseStopwatch(this INodeInit self, FrameTime? time = null)
    {
        time ??= self.UseTime();
        var stopwatch = new Stopwatch();

        self.On<First>(_ =>
        {
            stopwatch.Tick(time.Delta);
        });

        return stopwatch;
    }
}