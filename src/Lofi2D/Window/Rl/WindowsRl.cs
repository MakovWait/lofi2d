using Lofi2D.Core;
using Lofi2D.Core.Comp;
using Lofi2D.Math;
using Lofi2D.Render;
using Lofi2D.Window.Components;
using Raylib_cs;

namespace Lofi2D.Window.Rl;

public class WindowsRl : IWindows
{
    private WindowRl? _window;
    public IWindow Main => _window!;

    public void Start(WindowSettings settings, Input input)
    {
        var size = settings.Size ?? new Vector2I(800, 450);
        Raylib.SetConfigFlags(ConfigFlags.TopmostWindow | ConfigFlags.ResizableWindow);
        Raylib.InitWindow(
            size.X,
            size.Y,
            settings.Title ?? "Game"
        );
        Raylib.SetTargetFPS(settings.TargetFps ?? 60);
        _window = new WindowRl(
            new AppViewport(new SubViewport(settings.ViewportSettings), input)
        );
    }

    public void Close()
    {
        Raylib.CloseWindow();
    }
}

public class WindowRl(AppViewport viewport) : IWindow, IAppViewportTarget
{
    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Colors.White);

        var size = new Vector2I(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        viewport.Draw(size, this);

        Raylib.EndDrawing();
    }

    public void BindTo(INodeInit self)
    {
        viewport.BindTo(self);
    }

    void IAppViewportTarget.Draw(_Texture2D texture, Rect2 rect, Rect2 sourceRect)
    {
        Raylib.DrawTexturePro(
            texture,
            sourceRect,
            rect,
            Vector2.Zero,
            0,
            Colors.White
        );
    }
}
