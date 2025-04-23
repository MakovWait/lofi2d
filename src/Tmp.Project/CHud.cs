using Tmp.Core.Comp;
using Tmp.Render.Components;

namespace Tmp.Project;

public class CHud() : ComponentFunc(self =>
{
    return new CCanvasLayer()
    {
        new CCanvasItem()
        {
            OnDraw = ctx => ctx.DrawFps(),
        },
    };
});
