using Raylib_cs;
using Tmp.Asset;
using Tmp.Asset.Components;
using Tmp.Core;
using Tmp.Core.Comp;
using Tmp.Core.Comp.Flow;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Render.Components;
using Tmp.Window.Components;
using Timer = Tmp.Time.Timer;

namespace Tmp.Project;

public static class Project
{
    public static Component GetRoot()
    {
        return new CWindow(new WindowSettings
        {
            Title = "Hello world!",
            Size = new Vector2I(640, 360),
            TargetFps = 60
        })
        {
            new CAssets(new Assets())
            {
                new CNode2DTransformRoot()
                {
                    new CCanvasLayer()
                    {
                        new CCanvasItem()
                        {
                            OnDraw = ctx => ctx.DrawFps(),
                        },
                    },
                    new CCamera2D()
                    {
                        Width = 640/2f,
                        Height = 360f/2
                    },
                    new Component()
                    {
                        Name = "BulletsContainer"
                    },
                    new CSnake(),
                }
            }
        };
    }
}

public class CSnake : Component
{
    protected override Components Init(INodeInit self)
    {
        const int spacing = 8;
        var bodyParts = new ReactiveList<BodyPart>();
        var headPositions = new Out<List<Vector2>>();
        var headTransform = new Out<CNode2DTransform>();
        
        self.OnMount(() =>
        {
            for (var i = 0; i < 8; i++)
            {
                bodyParts.Add(new BodyPart(Guid.NewGuid().ToString()));
            }
        });
        
        self.OnLate<Update>(dt =>
        {
            for (var i = 0; i < bodyParts.Count; i++)
            {
                var part = bodyParts.Get(i);
                var targetDistance = (i + 1) * 16f;
                var distanceSum = 0.0f;

                var prevPoint = headTransform.Value.GlobalPosition;
                Vector2? targetPoint = null;
                for (var j = 0; j < headPositions.Value.Count; j++)
                {
                    var currentPoint = headPositions.Value[j];
                    var d = prevPoint.DistanceTo(currentPoint);
                    distanceSum += d;
                    if (distanceSum >= targetDistance)
                    {
                        if (j > 0)
                        {
                            targetPoint = headPositions.Value[j - 1];
                        }
                        else
                        {
                            targetPoint = currentPoint;
                        }
                        break;
                    }
                    prevPoint = currentPoint;
                }
                if (targetPoint.HasValue)
                {
                    part.Position = targetPoint.Value;
                }
            }

            // var prevPos = headTransform.Value.Position;
            // foreach (var bodyPart in bodyParts)
            // {
            //     var dir = (prevPos - bodyPart.Position).Normalized();
            //     var dist = prevPos.DistanceTo(bodyPart.Position);
            //     if (dist > spacing)
            //     {
            //         bodyPart.Position += dir * (dist - spacing);
            //     }
            //     prevPos = bodyPart.Position;
            // }
        });
        
        return [
            new CHead()
            {
                OutPositions = headPositions,
                OutTransform = headTransform,
            },
            new For<BodyPart>
            {
                In = bodyParts,
                Render = (part, _) => new CBodyPart(part)
            },
        ];
    }
}

public class BodyPart(string key) : For<BodyPart>.IItem
{
    public string Key { get; } = key;
    
    public Vector2 Position { get; set; }
}

public class CBodyPart(BodyPart data) : Component
{
    protected override Components Init(INodeInit self)
    {
        var transform = self.UseTransform2D();
        var canvasItem = self.UseCanvasItem(transform);
        
        canvasItem.OnDraw(
            ctx =>
            {
                ctx.DrawRect(new Rect2I(-8, -8, 16, 16), Color.Blue);
            }
        );
        
        self.On<Update>(dt =>
        {
            transform.Position = data.Position;
        });
        
        return base.Init(self);
    }
}

public class CHead : Component
{
    public required IOut<List<Vector2>> OutPositions { get; init; }
    public required IOut<CNode2DTransform> OutTransform { get; init; }
    
    protected override Components Init(INodeInit self)
    {
        var dir = Vector2.Up;
        Radians rotationSpeed = (1.66f * Mathf.Pi);
        const int speed = 60;
        
        var transform = self.UseTransform2D();
        OutTransform.Init(transform);
        var canvasItem = self.UseCanvasItem(transform);
        var input = self.UseContext<Input>();

        var positions = new List<Vector2>();
        OutPositions.Init(positions);
        var savePositionsTimer = new Timer(0.1f);

        canvasItem.OnDraw(
            ctx =>
            {
                ctx.DrawRect(new Rect2I(-8, -8, 16, 16), Color.White);
                ctx.DrawLine(Vector2.Zero, dir * 20, Color.Blue);
                
                foreach (var position in positions)
                {
                    var drawPos = position - transform.GlobalPosition;
                    ctx.DrawRect(new Rect2I((int)drawPos.X, (int)drawPos.Y, 2, 2), Color.Red);
                }
            }
        );
                        
        self.On<Update>(dt =>
        {
            if (input.IsKeyPressed(KeyboardKey.A))
            {
                dir = dir.Rotated(-rotationSpeed * dt);
            }
            if (input.IsKeyPressed(KeyboardKey.D))
            {
                dir = dir.Rotated(rotationSpeed * dt);
            }
            transform.GlobalPosition += dir.Normalized() * dt * speed;
            
            if (savePositionsTimer.Tick(dt))
            {
                positions.Insert(0, transform.GlobalPosition);
                if (positions.Count > 256)
                {
                    positions.RemoveAt(positions.Count - 1);
                }
                
                // if (positions.Count > 256)
                // {
                //     positions.RemoveAt(0);
                // }
                // positions.Add(transform.GlobalPosition);
            }
        });
                        
        return [];
    }
}
