using Raylib_cs;
using Tmp.Core.Comp;

namespace Tmp.Time;

public class FrameTime(FrameTime? parent)
{
    public FrameTime Root => GetRoot();
    
    /// local scale
    public float Scale { get; set; } = 1f;
    
    /// seconds
    public float Elapsed => _elapsed;

    /// seconds
    public float Delta => _delta;
    
    private float _delta;
    private float _elapsed;
    
    public void Tick(float delta)
    {
        _delta = delta * FinalScale();
        _elapsed += _delta;
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