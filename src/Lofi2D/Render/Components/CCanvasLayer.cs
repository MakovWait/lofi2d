using Lofi2D.Core.Comp;
using Lofi2D.Math;
using Lofi2D.Math.Components;
using Lofi2D.Render.Util;

namespace Lofi2D.Render.Components;

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