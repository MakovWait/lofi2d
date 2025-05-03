using Lofi2D.Core;
using Lofi2D.Core.Comp;
using Lofi2D.Math;
using Lofi2D.Math.Components;
using Lofi2D.Render.Texture;
using Lofi2D.Render.Util;

namespace Lofi2D.Render.Components;

public class CSubViewport(CSubViewport.Props props) : CFunc((self, children) =>
{
    var container = self.UseContext<ISubViewportContainer>();
    var viewport = new SubViewport(
        props.Settings
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
        public required SubViewport.Settings Settings { get; init; }
        
        public required IOut<ITexture2D?> Texture { get; init; }
    }
}