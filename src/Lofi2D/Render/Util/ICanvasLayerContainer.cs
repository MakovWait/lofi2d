namespace Lofi2D.Render.Util;

public interface ICanvasLayerContainer
{
    public void Add(CanvasLayer layer);

    public void Remove(CanvasLayer layer);
}