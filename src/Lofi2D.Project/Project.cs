using Lofi2D.Asset;
using Lofi2D.Asset.Components;
using Lofi2D.Audio;
using Lofi2D.Audio.Components;
using Lofi2D.Core;
using Lofi2D.Core.Comp;
using Lofi2D.Math;
using Lofi2D.Math.Components;
using Lofi2D.Render;
using Lofi2D.Render.Components;
using Lofi2D.Render.Texture;
using Lofi2D.Time;
using Lofi2D.Window.Components;
using Raylib_cs;

namespace Lofi2D.Project;

public static class Palette
{
    public static Color Background { get; } = new("23222a");
    public static Color Shadow { get; } = Colors.Black;
}

public static class Project
{
    public static Component GetRoot()
    {
        return new CFunc(root =>
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
                new CAudioBusLayout(
                    new MasterBus
                    {
                        new AudioBus("Sfx"),
                        new AudioBus("Music"),
                    }
                )
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
                                    
                                    new CFunc(self =>
                                    {
                                        var time = self.UseTime();
                                        var transform = self.UseTransform2D();
                                        var canvasItem = self.UseCanvasItem(transform);
                                    
                                        canvasItem.OnDraw(ctx =>
                                        {
                                            Rlgl.PushMatrix();
                                            Rlgl.MultMatrixf(transform.Global);
                                            Raylib.DrawText("hell\no!", 0, 0, 10, Colors.White);
                                            Rlgl.PopMatrix();
                                            
                                            ctx.DrawCircle(new Vector2I(32, 0), 16, Colors.White, false, -1);
                                            // ctx.DrawRect(new Rect2(-8, -8, 16, 16), Colors.White, true, -3);
                                            ctx.DrawRectRounded(new Rect2(-8, -8, 16, 16), Colors.White, 0.4f, 32, false, -1);
                                            
                                            ctx.DrawLine(new Vector2(-32, -32), new Vector2(-32, 32), Colors.White, 1);
                                            
                                            // Raylib.DrawRectangleRounded(new Rect2(-16, -16, 32, 32), 0.4f, 32, Colors.White);
                                            // Raylib.DrawRectangleRoundedLines(new Rect2(-16, -16, 32, 32), 0.4f, 32, 2, Colors.White);
                                        });

                                        self.On<Update>(_ =>
                                        {
                                            // transform.Skew = 0;
                                            transform.Scale = new Vector2(2, 1f);
                                            transform.Rotation = 0;
                                            transform.Skew = Mathf.Lerp(-5.DegToRad(), 5.DegToRad(), Mathf.Sin(time.Elapsed));
                                            // transform.Rotation = Mathf.Lerp(-5.DegToRad(), 5.DegToRad(), Mathf.Sin(time.Elapsed));
                                            // transform.Position = new Vector2(100, 100);
                                        });
                                    
                                        return [];
                                    })
                                },

                                // shadow
                                new CFunc(self =>
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
                                new CFunc(self =>
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
                }
            };
        });
    }
}

public readonly record struct SettingDrawGizmo(bool Value);