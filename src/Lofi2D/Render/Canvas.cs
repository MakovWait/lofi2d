using Lofi2D.Render.Util;

namespace Lofi2D.Render;

public class Canvas : ICanvasItemContainer
{
    private readonly CanvasItem _root = new();
    
    public void AddChild(CanvasItem child)
    {
        _root.AddChild(child);
    }

    public void RemoveChild(CanvasItem child)
    {
        _root.RemoveChild(child);
    }
    
    public void Draw()
    {
        _root.Draw();
    }
}