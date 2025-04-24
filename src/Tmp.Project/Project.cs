using Tmp.Asset;
using Tmp.Asset.Components;
using Tmp.Core.Comp;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Render.Components;
using Tmp.Time;
using Tmp.Window.Components;

namespace Tmp.Project;

public static class Project
{
    public static Component GetRoot()
    {
        return new ComponentFunc(root =>
        {
            var gameSize = new Vector2I(320, 180);
            var settingDrawGizmo = root.CreateContext(new SettingDrawGizmo(false));
            
            var boundsSize = new Vector2(320, 180);
            var bounds = root.CreateContext(new Bounds(new Rect2(-boundsSize / 2, boundsSize)));
            root.CreateContext(new Snake());

            return new CWindow(new WindowSettings
            {
                Title = "Hello world!",
                Size = gameSize,
                TargetFps = 60
            })
            {
                new CTime
                {
                    new CAssets(new Assets())
                    {
                        new CNode2DTransformRoot()
                        {
                            new CBoundsGizmo(bounds).If(settingDrawGizmo.Value),
                            new CCamera2D()
                            {
                                Offset = gameSize / 2
                            },
                            new CSnake(),
                            new CFood(),
                            new CHud(),
                        }
                    }
                }
            };
        });
    }
}

public readonly record struct SettingDrawGizmo(bool Value);
