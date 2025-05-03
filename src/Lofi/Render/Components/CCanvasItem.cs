using Lofi.Core.Comp;
using Lofi.Math;
using Lofi.Math.Components;

namespace Lofi.Render.Components;

public class CCanvasItem : Component
{
    public Transform2D? Transform2D { get; init; } = null;

    public Action<IDrawContext>? OnDraw { get; init; } = null;
    
    protected override Core.Comp.Components Init(INodeInit self)
    {
        var transform = self.UseTransform2D(Transform2D);
        var canvasItem = self.UseCanvasItem(transform);
        canvasItem.OnDraw(ctx =>
        {
            OnDraw?.Invoke(ctx);
        });
        return Children;
    }
}