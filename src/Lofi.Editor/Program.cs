﻿using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using Lofi;
using Lofi.Core;
using Lofi.Core.Comp;
using Lofi.Math;
using Lofi.Project;
using Lofi.Render;
using Lofi.Window;
using Lofi.Window.Components;

var app = new App(new CWindowsImGui
{
    Project.GetRoot()
});
await app.Run();

public class CWindowsImGui() : CFunc((self, children) =>
{
    var windows = new WindowsImGui();
    self.CreateContext<IWindows>(windows);
    
    self.OnLateCleanup(() => windows.Close());

    return children;
});

internal class WindowImGui(AppViewport viewport) : IWindow, IAppViewportTarget
{
    public void Draw()
    {
        Raylib.BeginDrawing();
        rlImGui.Begin();
        Raylib.ClearBackground(Color.White);

        if (ImGui.Begin("Game"))
        {
            viewport.Input.Enable(ImGui.IsWindowFocused());
            var region = ImGui.GetContentRegionAvail();
            viewport.Draw(
                new Vector2I(region.X.ToInt(), region.Y.ToInt()),
                this
            );
            ImGui.End();
        }

        if (ImGui.Begin("Game2"))
        {
            ImGui.End();
        }
        
        rlImGui.End();
        Raylib.EndDrawing();
    }

    public void BindTo(INodeInit self)
    {
        viewport.BindTo(self);
    }

    void IAppViewportTarget.Draw(Texture2D texture, Rect2 rect, Rect2 sourceRect)
    {
        var destWidth = rect.Size.X.ToInt();
        var destHeight = rect.Size.Y.ToInt();
        ImGui.SetCursorPosX(0.0f);
        ImGui.SetCursorPosX(rect.Position.X);
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + rect.Position.Y);
        rlImGui.ImageRect(texture, destWidth, destHeight, sourceRect);
    }
}

internal class WindowsImGui : IWindows
{
    private IWindow? _window;

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
        _window = new WindowImGui(new AppViewport(new SubViewport(size), input));
        rlImGui.Setup(false);
    }

    public void Close()
    {
        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }
}