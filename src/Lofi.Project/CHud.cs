using Lofi.Core.Comp;
using Lofi.Render.Components;

namespace Lofi.Project;

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
