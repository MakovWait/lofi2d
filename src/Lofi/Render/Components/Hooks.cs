using Lofi.Core.Comp;
using Lofi.Math.Components;
using Lofi.Render.Util;

namespace Lofi.Render.Components;

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