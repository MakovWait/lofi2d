using Lofi2D.Core.Comp;
using Lofi2D.Render.Components;

namespace Lofi2D.Project;

public class CHud() : CFunc(self =>
{
    return new CCanvasLayer()
    {
        new CCanvasItem()
        {
            OnDraw = ctx => ctx.DrawFps(),
        },
    };
});
