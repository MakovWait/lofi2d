using Lofi2D.Math.Components;
using Lofi2D.Core.Comp;
using Lofi2D.Math;
using Lofi2D.Render.Util;

namespace Lofi2D.Render.Components;

public class CCamera2D : Component
{
    public Vector2 Offset { get; init; }
    
    public Transform2D? InitialTransform { get; init; }

    protected override Core.Comp.Components Init(INodeInit self)
    {
         var transform = self.UseTransform2D(InitialTransform);
         var camera = self.UseContext<ICamera2D>();
         
         self.On<PreDraw>(_ =>
         {
             camera.Target = transform.GlobalPosition;
             camera.Offset = Offset;
         });
         
         return Children;
    }
}