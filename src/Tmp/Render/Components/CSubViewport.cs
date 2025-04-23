using Tmp.Asset.BuiltIn.Texture;
using Tmp.Core;
using Tmp.Core.Comp;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Render.Util;

namespace Tmp.Render.Components;

public class CSubViewport(CSubViewport.Props props) : ComponentFunc((self, children) =>
{
    var container = self.UseContext<ISubViewportContainer>();
    var viewport = new SubViewport(
        props.Size
    );
    viewport.BindTo(self);
    
    self.CreateContext(new CNode2DTransform
    {
        Local = Transform2D.Identity
    });

    props.Texture.Init(viewport.Texture);
    
    viewport.AddTo(container);
    self.OnLateCleanup(() => viewport.RemoveFrom(container));

    return children;
})
{
    public readonly struct Props
    {
        public Vector2I Size { get; init; }
        
        public IOut<ITexture2D?> Texture { get; init; }
    }
}