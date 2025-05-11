using Lofi2D.Core.Comp;
using Lofi2D.Math.Components;
using Lofi2D.Render.Components;

namespace Lofi2D.GUI;

public static class Hooks
{
    public static Control UseControl(this INodeInit self)
    {
        var parent = self.UseNullableContext<Control>();
        
        var transform = self.UseTransform2D();
        var canvasItem = self.UseCanvasItem();
        var control = self.CreateContext<Control>(new Control(canvasItem, transform));
        
        if (parent != null)
        {
            parent.AddChild(control);
            self.OnLateCleanup(() => parent.RemoveChild(control));
        }
        
        self.On<PreDraw>(_ => control.PreDraw());
        
        return control;
    }
}