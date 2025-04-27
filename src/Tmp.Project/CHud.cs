using Tmp.Core.Comp;
using Tmp.Render.Components;

namespace Tmp.Project;

public class CHud() : СFunc(self =>
{
    return new CCanvasLayer()
    {
        new CCanvasItem()
        {
            OnDraw = ctx => ctx.DrawFps(),
        },
    };
});
