using Tmp.Asset;
using Tmp.Asset.BuiltIn.Texture;
using Tmp.Asset.Components;
using Tmp.Core;
using Tmp.Core.Comp;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Render;
using Tmp.Render.Components;
using Tmp.Time;
using Tmp.Window.Components;

namespace Tmp.Project;

public static class Palette
{
    public static Color Background { get; } = new("23222a");
    public static Color Shadow { get; } = Colors.Black;
}

public static class Project
{
    public static Component GetRoot()
    {
        return new СFunc(root =>
        {
            var gameSize = new Vector2I(320, 180);
            var settingDrawGizmo = root.CreateContext(new SettingDrawGizmo(false));
            
            var boundsSize = new Vector2(320, 180);
            var bounds = root.CreateContext(new Bounds(new Rect2(-boundsSize / 2, boundsSize)));
            root.CreateContext(new Snake());
            
            var viewportTexture = new Out<ITexture2D?>();

            return new CWindow(new WindowSettings
            {
                Title = "Hello world!",
                Size = new Vector2I(1280, 720),
                TargetFps = 60,
                ViewportSettings = new SubViewport.Settings
                {
                    Size = gameSize,
                    ClearColor = Palette.Background
                }
            })
            {
                new CTime
                {
                    new CAssets(new Assets())
                    {
                        new CNode2DTransformRoot()
                        {
                            new CSubViewport(new CSubViewport.Props
                            {
                                Settings = new SubViewport.Settings
                                {
                                    ClearColor = Colors.Transparent,
                                    Size = gameSize,
                                },
                                Texture = viewportTexture
                            })
                            {
                                new CBoundsGizmo(bounds).If(settingDrawGizmo.Value),
                                new CCamera2D()
                                {
                                    Offset = gameSize / 2
                                },
                                new CSnake(),
                                new CFood(),
                                new CHud(),   
                            },
                      
                            // shadow
                            new СFunc(self =>
                            {
                                var transform = self.UseTransform2D();
                                transform.Position += new Vector2(3, 3);
                                
                                var canvasItem = self.UseCanvasItem(transform);
                                canvasItem.Material = self.UseShaderMaterial(
                                    "assets://shaders/shadow_only/shader.jass",
                                    new IShaderMaterialParameters.Lambda(shader =>
                                    {
                                        shader.SetUniform("shadowColor", Palette.Shadow);
                                    })
                                );
                                
                                canvasItem.OnDraw(ctx =>
                                {
                                    viewportTexture.Value!.Draw(ctx, Vector2.Zero, Colors.White);
                                });

                                return [];
                            }),
                            new СFunc(self =>
                            {
                                var transform = self.UseTransform2D();
                                var canvasItem = self.UseCanvasItem(transform);
                                
                                canvasItem.OnDraw(ctx =>
                                {
                                    viewportTexture.Value!.Draw(ctx, Vector2.Zero, Colors.White);
                                });

                                return [];
                            }),
                        }
                    }
                }
            };
        });
    }
}

public readonly record struct SettingDrawGizmo(bool Value);
