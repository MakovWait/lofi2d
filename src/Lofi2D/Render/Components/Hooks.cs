using Lofi2D.Core.Comp;
using Lofi2D.Math.Components;
using Lofi2D.Render.Util;

namespace Lofi2D.Render.Components;

public static class Hooks
{
    public static CanvasItem UseCanvasItem(this INodeInit self, CNode2DTransform transform)
    {
        var parent = self.UseContext<ICanvasItemContainer>();
        var item = new CanvasItem();

        self.CreateContext<ICanvasItemContainer>(item);

        parent.AddChild(item);
        self.OnLateCleanup(() =>
        {
            parent.RemoveChild(item);
        });

        self.On<PreDraw>(_ =>
        {
            item.SetFinalTransform(transform.Global);
        });

        return item;
    }
}