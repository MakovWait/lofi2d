using Lofi2D.Core;
using Lofi2D.Core.Comp;
using Lofi2D.Math;
using Lofi2D.Render;
using Lofi2D.Window.Rl;

namespace Lofi2D.Window.Components;

public readonly struct WindowSettings
{
    public string? Title { get; init; }
    public Vector2I? Size { get; init; }
    public int? TargetFps { get; init; }
    public required SubViewport.Settings ViewportSettings { get; init; }
}

public class CWindow(WindowSettings settings) : CFunc((self, children) =>
{
    var windows = self.UseContext<IWindows>();

    var input = new Input(true);
    windows.Start(settings, input);
    var window = windows.Main;
    
    window.BindTo(self);

    self.CreateContext(input);
    
    self.On<Draw>(_ => window.Draw());

    return children;
});

public class CWindowsRl() : CFunc((self, children) =>
{
    var windows = new WindowsRl();
    self.CreateContext<IWindows>(windows);
    
    self.OnLateCleanup(() => windows.Close());

    return children;
});
