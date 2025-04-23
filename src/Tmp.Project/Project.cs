using Raylib_cs;
using Tmp.Asset;
using Tmp.Asset.BuiltIn.Texture;
using Tmp.Asset.Components;
using Tmp.Core;
using Tmp.Core.Comp;
using Tmp.Core.Comp.Flow;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Render;
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
        var headRef = new Out<CHead.Ref>();
        var bodyParts = new ReactiveList<BodyPart>();
        
        self.OnMount(() =>
        {
            for (var i = 0; i < 8; i++)
            {
                bodyParts.Add(new BodyPart(Guid.NewGuid().ToString()));
            }
        });
        
        self.OnLate<Update>(dt =>
        {
            SyncBodyPartsMovement();
        });
        
        return [
            new CHead()
            {
                OutRef = headRef
            },
            new For<BodyPart>
            {
                In = bodyParts,
                Render = (part, _) => new CBodyPart(part)
            },
        ];

        void SyncBodyPartsMovement()
        {
            for (var i = 0; i < bodyParts.Count; i++)
            {
                var part = bodyParts.Get(i);
                var targetDistance = (i + 1) * 10.4f;
                var distanceSum = 0.0f;

                var prevPoint = headRef.Value!.CurrentPosition;
                LastPosition? targetPoint = null;
                for (var j = 0; j < headRef.Value.LastPositions.Count; j++)
                {
                    var currentPoint = headRef.Value.LastPositions[j];
                    var d = prevPoint.Position.DistanceTo(currentPoint.Position);
                    distanceSum += d;
                    if (distanceSum >= targetDistance)
                    {
                        targetPoint = headRef.Value.LastPositions.GetPrevSafe(j) ?? currentPoint;
                        break;
                    }
                    prevPoint = currentPoint;
                }
                if (targetPoint.HasValue)
                {
                    part.Position = targetPoint.Value.Position;
                    part.Rotation = targetPoint.Value.Rotation;
                }
            }
        }
    }
}

public class BodyPart(string key) : For<BodyPart>.IItem
{
    public string Key { get; } = key;
    
    public Vector2 Position { get; set; }
    
    public Radians Rotation { get; set; }
}

public class CBodyPart(BodyPart data) : Component
{
    protected override Components Init(INodeInit self)
    {
        var transform = self.UseTransform2D();
        var canvasItem = self.UseCanvasItem(transform);
        var texture = self.UseAsset<ITexture2D>("assets://body_part_texture.jass");
        
        canvasItem.OnDraw(
            ctx =>
            {
                texture.Value.Draw(ctx, new Vector2(-16, -16), Color.White);
            }
        );
        
        self.On<Update>(dt =>
        {
            transform.GlobalPosition = data.Position;
            transform.GlobalRotation = data.Rotation;
        });
        
        return base.Init(self);
    }
}

public class CHead : Component
{
    public required IOut<Ref> OutRef { get; init; }
    
    protected override Components Init(INodeInit self)
    {
        var dir = Vector2.Up;
        Radians rotationSpeed = (1.66f * Mathf.Pi);
        const int speed = 60;
        
        var transform = self.UseTransform2D();
        var canvasItem = self.UseCanvasItem(transform);
        var input = self.UseContext<Input>();
        var texture = self.UseAsset<ITexture2D>("assets://body_part_texture.jass");

        var positions = new LastPositions();
        var savePositionsTimer = new Timer(0.1f);

        self.Init(OutRef, new Ref(transform, positions));

        canvasItem.OnDraw(
            ctx =>
            {
                texture.Value.Draw(ctx, new Vector2(-16, -16), Color.White);
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
            transform.GlobalRotation = dir.Angle();
            
            if (savePositionsTimer.Tick(dt))
            {
                positions.Add(new LastPosition()
                {
                    Position = transform.GlobalPosition,
                    Rotation = transform.GlobalRotation,
                });
            }
        });
        
        return [
            new ComponentFunc(gizmo =>
            {
                var gizmoTransform = gizmo.UseTransform2D();
                var gizmoCanvasItem = gizmo.UseCanvasItem(gizmoTransform);
            
                gizmoCanvasItem.OnDraw(
                    ctx =>
                    {
                        ctx.DrawLine(Vector2.Zero, dir * 20, Color.Blue);
                        positions.Draw(gizmoTransform.GlobalPosition, ctx);
                    }
                );
            
                gizmo.On<Update>(_ =>
                {
                    gizmoTransform.GlobalRotation = 0;
                });
            
                return [];
            }).If(true)
        ];
    }

    public class Ref(CNode2DTransform transform, LastPositions positions)
    {
        public LastPosition CurrentPosition => new()
        {
            Position = transform.GlobalPosition,
            Rotation = transform.GlobalRotation,
        };  
        
        public LastPositions LastPositions => positions;
    }
}


public class LastPositions(int maxCount = 256)
{
    private readonly List<LastPosition> _positions = [];
    
    public LastPosition this[int index] => _positions[index];
    
    public int Count => _positions.Count;
    
    public LastPosition? GetPrevSafe(int idx)
    {
        if (idx > 0)
        {
            return this[idx - 1];
        }
        else
        {
            return null;
        }
    }
    
    public void Add(LastPosition position)
    {
        _positions.Insert(0, position);
        if (_positions.Count > maxCount)
        {
            _positions.RemoveAt(_positions.Count - 1);
        }
                
        // if (_positions.Count > maxCount)
        // {
        //     _positions.RemoveAt(0);
        // }
        // _positions.Add(position);
    }

    public void Draw(Vector2 origin, IDrawContext? ctx)
    {
        foreach (var position in _positions)
        {
            var drawPos = (position.Position - origin) - Vector2.One;
            ctx.DrawRect(new Rect2I((int)drawPos.X, (int)drawPos.Y, 2, 2), Color.Red);
        }
    }
}

public struct LastPosition
{
    public required Vector2 Position { get; init; }
    
    public required Radians Rotation { get; init; }
}