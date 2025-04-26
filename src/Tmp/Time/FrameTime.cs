using Raylib_cs;
using Tmp.Core.Comp;

namespace Tmp.Time;

public class FrameTime(FrameTime? parent)
{
    public FrameTime Root => GetRoot();
    /// local scale
    public float Scale { get; set; } = 1f;
    /// seconds
    public float Elapsed { get; private set; }
    /// seconds
    public float Delta { get; private set; }
    /// seconds
    public float DeltaUnscaled { get; private set; }

    public void Tick(float delta)
    {
        Delta = delta;
        Delta = delta * FinalScale();
        Elapsed += Delta;
    }

    private float FinalScale()
    {
        return Scale * (parent?.Scale ?? 1f);
    }

    private FrameTime GetRoot()
    {
        return parent == null ? this : parent.Root;
    }
}

public class CTime() : ComponentFunc((self, children) =>
{
    var time = self.CreateContext(new FrameTime(null));
    
    self.On<First>(_ =>
    {
        time.Tick(Raylib.GetFrameTime());
    });
    
    return children;
});

public static class TimeEx
{
    public static FrameTime UseTime(this INodeInit self)
    {
        return self.UseContext<FrameTime>();
    }
    
    public static FrameTime CreateFrameTimeOverride(this INodeInit self)
    {
        var parentTime = self.UseTime();
        var timeOverride = self.CreateContext(new FrameTime(parentTime));
        
        self.On<First>(_ =>
        {
            timeOverride.Tick(Raylib.GetFrameTime());
        });
        
        return timeOverride;
    }
}