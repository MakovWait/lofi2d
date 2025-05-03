namespace Lofi.Render.Util;

public interface ICanvasItemContainer
{
    public void AddChild(CanvasItem child);

    public void RemoveChild(CanvasItem child);
}