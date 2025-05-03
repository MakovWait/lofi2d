using Lofi.Core.Comp;
using Lofi.Math;
using Lofi.Math.Components;
using Lofi.Render.Util;

namespace Lofi.Render.Components;

public class CCanvasLayer : Component
{
    protected override Core.Comp.Components Init(INodeInit self)
    {
        var layer = new CanvasLayer();

        self.CreateContext(new CNode2DTransform
        {
            Local = Transform2D.Identity
        });
        self.CreateContext<ICanvasItemContainer>(layer);

        var container = self.UseContext<ICanvasLayerContainer>();
        container.Add(layer);
        self.OnLateCleanup(() => container.Remove(layer));

        return Children;
    }
};