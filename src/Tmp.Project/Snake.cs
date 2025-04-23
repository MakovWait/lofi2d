using Raylib_cs;
using Tmp.Asset.BuiltIn.Texture;
using Tmp.Asset.Components;
using Tmp.Core;
using Tmp.Core.Comp;
using Tmp.Core.Comp.Flow;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Render;
using Tmp.Render.Components;

namespace Tmp.Project;

public class Snake
{
    public SnakeHead Head { get; } = new();
    
    public readonly ReactiveList<BodyPart> BodyParts = [];

    public void AddBodyPart()
    {
        BodyParts.Add(new BodyPart(Guid.NewGuid().ToString())
        {
            Rotation = 0,
            Position = Head.Transform.Origin
        });
    }
}

public class SnakeHead
{
    public Transform2D Transform { get; set; }

    public LastPosition CurrentPosition => new()
    {
        Position = Transform.Origin,
        Rotation = Transform.Rotation
    };
    
    public LastPositions LastPositions { get; } = new();
}

public class CSnake : Component
{
    protected override Components Init(INodeInit self)
    {
        var snake = self.UseContext<Snake>();
        var head = snake.Head;
        var bodyParts = snake.BodyParts;
        
        self.OnLate<Update>(dt =>
        {
            SyncBodyPartsMovement();
            CheckHeadAndBodyCollisions();
        });
        
        return [
            new CHead(),
            new For<BodyPart>
            {
                In = bodyParts,
                Render = (part, _) => new CBodyPart(part)
            },
        ];

        void CheckHeadAndBodyCollisions()
        {
            var shouldRemove = false;
            foreach (var bodyPart in bodyParts)
            {
                if (bodyPart.Collides(head))
                {
                    shouldRemove = true;
                }

                if (shouldRemove)
                {
                    bodyParts.QueueRemove(bodyPart);
                }
            }
            bodyParts.FlushRemoveQueue();
        }
        
        void SyncBodyPartsMovement()
        {
            for (var i = 0; i < bodyParts.Count; i++)
            {
                var part = bodyParts.Get(i);
                var targetDistance = (i + 1) * 10.4f;
                var distanceSum = 0.0f;

                var prevPoint = head.CurrentPosition;
                LastPosition? targetPoint = null;
                for (var j = 0; j < head.LastPositions.Count; j++)
                {
                    var currentPoint = head.LastPositions[j];
                    var d = prevPoint.Position.DistanceTo(currentPoint.Position);
                    distanceSum += d;
                    if (distanceSum >= targetDistance)
                    {
                        targetPoint = head.LastPositions.GetPrevSafe(j) ?? currentPoint;
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

    public bool Collides(SnakeHead head)
    {
        return Position.DistanceSquaredTo(head.Transform.Origin) <= 4 * 4;
    }
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
    protected override Components Init(INodeInit self)
    {
        var dir = Vector2.Up;
        Radians rotationSpeed = (1.66f * Mathf.Pi);
        const int speed = 60;
        const int radius = 6;
        
        var savePositionsTimer = new Timer(0.1f);

        var head = self.UseContext<Snake>().Head;
        var transform = self.UseTransform2D();
        var canvasItem = self.UseCanvasItem(transform);
        var input = self.UseContext<Input>();
        var texture = self.UseAsset<ITexture2D>("assets://body_part_texture.jass");
        var bounds = self.UseContext<Bounds>();

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

            dir = bounds.CheckBounce(transform.GlobalPosition, dir, radius);

            transform.GlobalPosition += dir.Normalized() * speed * dt;
            transform.GlobalRotation = dir.Angle();

            head.Transform = transform.Global;
            if (savePositionsTimer.Tick(dt))
            {
                head.LastPositions.Add(new LastPosition()
                {
                    Position = transform.GlobalPosition,
                    Rotation = transform.GlobalRotation,
                });
            }
        });
        
        return [
            // gizmo
            new ComponentFunc(gizmo =>
            {
                var gizmoTransform = gizmo.UseTransform2D();
                var gizmoCanvasItem = gizmo.UseCanvasItem(gizmoTransform);
            
                gizmoCanvasItem.OnDraw(
                    ctx =>
                    {
                        ctx.DrawLine(Vector2.Zero, dir * 20, Color.Blue);
                        head.LastPositions.Draw(gizmoTransform.GlobalPosition, ctx);
                    }
                );
            
                gizmo.On<Update>(_ =>
                {
                    gizmoTransform.GlobalRotation = 0;
                });
            
                return [];
            }).If(self.UseContext<SettingDrawGizmo>().Value)
        ];
    }
}


public class LastPositions(int maxCount = 2048)
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