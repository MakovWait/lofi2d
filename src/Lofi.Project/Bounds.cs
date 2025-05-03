using Lofi.Core.Comp;
using Lofi.Math;
using Lofi.Math.Components;
using Lofi.Render;
using Lofi.Render.Components;

namespace Lofi.Project;

public class Bounds(Rect2 rect, float spawnFoodOffset = 16)
{
    public Vector2 CheckBounce(Vector2 position, Vector2 dir, float radius)
    {
        var topNormal = Vector2.Down;
        if (position.Y - radius < rect.Position.Y && dir.Dot(topNormal) < 0)
        {
            dir = dir.Bounce(topNormal);
        }
        
        var bottomNormal = Vector2.Up;
        if (position.Y + radius > rect.Position.Y + rect.Size.Y && dir.Dot(bottomNormal) < 0)
        {
            dir = dir.Bounce(bottomNormal);
        }
        
        var leftNormal = Vector2.Right;
        if (position.X - radius < rect.Position.X && dir.Dot(leftNormal) < 0)
        {
            dir = dir.Bounce(leftNormal);
        }
        
        var rightNormal = Vector2.Left;
        if (position.X + radius > rect.Position.X + rect.Size.X && dir.Dot(rightNormal) < 0)
        {
            dir = dir.Bounce(rightNormal);
        }

        return dir;
    }

    public void DrawGizmo(IDrawContext ctx)
    {
        var topLeft = rect.Position;
        var topRight = rect.Position + new Vector2(rect.Size.X, 0);
        var bottomLeft = rect.Position + new Vector2(0, rect.Size.Y);
        var bottomRight = rect.Position + rect.Size;

        ctx.DrawLine(topLeft, topRight, Colors.Blue);
        ctx.DrawLine(topRight, bottomRight, Colors.Blue);
        ctx.DrawLine(bottomRight, bottomLeft, Colors.Blue);
        ctx.DrawLine(bottomLeft, topLeft, Colors.Blue);
    }

    public Vector2 GetRandomFoodPosition()
    {
        var center = rect.GetCenter();
        var radius = Mathf.Min(rect.Size.X, rect.Size.Y) / 2f - spawnFoodOffset;
        
        var angle = Random.Shared.NextSingle() * Mathf.Tau;
        var r = Mathf.Sqrt(Random.Shared.NextSingle()) * radius;

        var offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * r;
        return center + offset;
    }
}

public class CBoundsGizmo(Bounds bounds) : CFunc(self =>
{
    var canvasItem = self.UseCanvasItem(self.UseTransform2D());

    canvasItem.OnDraw(bounds.DrawGizmo);

    return [];
});