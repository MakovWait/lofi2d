using System.Diagnostics;
using Lofi2D.Math;
using Lofi2D.Render.Util;
using Raylib_cs;

namespace Lofi2D.Render;

public class CanvasItem : ICanvasItemContainer, IDrawContext, IMaterialTarget
{
    public IMaterial? Material { get; set; }
    public bool UseParentMaterial { get; set; } = false;
    public Color Modulate { get; set; } = Colors.White;
    public Color SelfModulate { get; set; } = Colors.White;
    
    private Color FinalModulate => SelfModulate * InheritedModulate;
    private Color InheritedModulate => (_parent?.Modulate ?? Colors.White) * Modulate;
    
    private CanvasItem? _parent;
    private Transform2D _transform = Transform2D.Identity;

    private readonly List<CanvasItem> _children = [];
    private Action<IDrawContext>? _onDraw;

    public void AddChild(CanvasItem child)
    {
        Debug.Assert(child._parent == null);
        _children.Add(child);
        child._parent = this;
    }

    public void RemoveChild(CanvasItem child)
    {
        Debug.Assert(child._parent == this);
        _children.Remove(child);
        child._parent = null;
    }

    Color IMaterialTarget.Modulate => FinalModulate;
    
    void IMaterialTarget.Draw()
    {
        InvokeDrawSelfCallback();
    }
    
    public void Draw()
    {
        DrawSelf();
        DrawChildren();
    }

    private void DrawSelf()
    {
        var material = UseParentMaterial ? _parent?.Material : Material;
        if (material != null)
        {
            material.Draw(this);
        }
        else
        {
            InvokeDrawSelfCallback();
        }
    }

    private void InvokeDrawSelfCallback()
    {
        _onDraw?.Invoke(this);
    }
    
    private void DrawChildren()
    {
        foreach (var canvasItem in _children)
        {
            canvasItem.Draw();
        }
    }

    public void DrawLine(Vector2 from, Vector2 to, Color color, float width=-1)
    {
        var transform = GetFinalTransform();

        if (width < 0)
        {
            from = transform.BasisXform(from);
            to = transform.BasisXform(to);
            Raylib.DrawLineEx(transform.Origin + from, transform.Origin + to, width.Abs(), color * FinalModulate);
        }
        else
        {
            Rlgl.PushMatrix();
            Rlgl.MultMatrixf(transform);
            Raylib.DrawLineEx(transform.Origin + from, transform.Origin + to, width.Abs(), color * FinalModulate);
            Rlgl.PopMatrix();   
        }
    }

    public void DrawRect(Rect2 rect, Color color, bool filled = true, float width = -1)
    {
        if (!filled && width == 0)
        {
            return;
        }
        
        var transform = GetFinalTransform();
        
        Rlgl.PushMatrix();
        Rlgl.MultMatrixf(transform);
        if (filled)
        {
            Raylib.DrawRectangleV(rect.Position, rect.Size, color * FinalModulate);
        }
        else
        {
            var effectiveWidth = width.Abs();
            float finalWidth;
            if (width > 0)
            {
                var averageScale = (transform.Scale.X + transform.Scale.Y) / 2.0f;
                var unscaledWidth = effectiveWidth * averageScale;
                finalWidth = unscaledWidth;
            }
            else
            {
                finalWidth = effectiveWidth;
            }
            
            if ((finalWidth > rect.Size.X) || (finalWidth > rect.Size.Y))
            {
                if (rect.Size.X >= rect.Size.Y) finalWidth = rect.Size.Y/2;
                else if (rect.Size.X <= rect.Size.Y) finalWidth = rect.Size.X/2;
            }
            
            Raylib.DrawRectangleLinesEx(new Rect2(rect.Position + new Vector2(-1, -1), rect.Size + new Vector2(1, 1)), finalWidth, Colors.WebGray);
        }
        Rlgl.PopMatrix();
    }

    public void DrawRectRounded(Rect2 rect, Color color, float roundness, int segments, bool filled = true, float width = -1)
    {
        if (roundness <= 0)
        {
            DrawRect(rect, color, filled, width);
            return;
        }
        
        if (!filled && width == 0)
        {
            return;
        }
        
        var transform = GetFinalTransform();
        Rlgl.PushMatrix();
        Rlgl.MultMatrixf(transform);
        if (filled)
        {
            Raylib.DrawRectangleRounded(rect, roundness, segments, color * FinalModulate);
        }
        else
        {
            var effectiveWidth = width.Abs();
            float finalWidth;
            if (width > 0)
            {
                var averageScale = (transform.Scale.X + transform.Scale.Y) / 2.0f;
                var unscaledWidth = effectiveWidth * averageScale;
                finalWidth = unscaledWidth;
            }
            else
            {
                finalWidth = effectiveWidth;
            }
            Raylib.DrawRectangleRoundedLinesEx(rect, roundness, segments, finalWidth, color * FinalModulate);
            // Raylib.DrawRectangleRounded(rect, 0.1f, 0, color * FinalModulate);
            // Raylib.DrawRectangleRoundedLines(new Rect2(rect.Position + new Vector2(1, 1), rect.Size + new Vector2(-2, -2)), 0.1f, 0, Colors.Red);
            
            // var effectiveWidth = width.Abs();
            // float finalWidth;
            // if (width > 0)
            // {
            //     var averageScale = (transform.Scale.X + transform.Scale.Y) / 2.0f;
            //     var unscaledWidth = effectiveWidth * averageScale;
            //     finalWidth = unscaledWidth;
            // }
            // else
            // {
            //     finalWidth = effectiveWidth;
            // }
            //
            // if ((finalWidth > rect.Size.X) || (finalWidth > rect.Size.Y))
            // {
            //     if (rect.Size.X >= rect.Size.Y) finalWidth = rect.Size.Y/2;
            //     else if (rect.Size.X <= rect.Size.Y) finalWidth = rect.Size.X/2;
            // }
            //
            // var topLeft = rect.Position;
            // var topRight = new Vector2(rect.Position.X + rect.Size.X, rect.Position.Y);
            // var bottomRight = new Vector2(rect.Position.X + rect.Size.X, rect.Position.Y + rect.Size.Y);
            // var bottomLeft = new Vector2(rect.Position.X, rect.Position.Y + rect.Size.Y);
            //
            // Raylib.DrawLineEx(topLeft, topRight, finalWidth, color * FinalModulate);
            // Raylib.DrawLineEx(topRight, bottomRight, finalWidth, color * FinalModulate);
            // Raylib.DrawLineEx(bottomRight, bottomLeft, finalWidth, color * FinalModulate);
            // Raylib.DrawLineEx(bottomLeft, topLeft, finalWidth, color * FinalModulate);
        }
        Rlgl.PopMatrix();
    }

    public void DrawCircle(Vector2 position, float radius, Color color, bool filled = true, float width = -1)
    {
        if (!filled && width == 0)
        {
            return;
        }
        
        var transform = GetFinalTransform();
        
        Rlgl.PushMatrix();
        Rlgl.MultMatrixf(transform);
        if (filled)
        {
            Raylib.DrawCircleV(position, radius, color * FinalModulate);
        }
        else
        {
            var effectiveWidth = width.Abs();
            float finalWidth;
            if (width > 0)
            {
                var averageScale = (transform.Scale.X + transform.Scale.Y) / 2.0f;
                var unscaledWidth = effectiveWidth * averageScale;
                finalWidth = unscaledWidth;
            }
            else
            {
                finalWidth = effectiveWidth;
            }
            
            if (finalWidth.IsEqualApproxTo(1))
            {
                Raylib.DrawCircleLinesV(position, radius, color * FinalModulate);
            }
            else
            {
                Raylib.DrawRing(position, radius - finalWidth, radius, 0, 360, 36, color * FinalModulate);
            }
        }
        Rlgl.PopMatrix();
    }

    public void DrawText(
        _Font font, Vector2 position, string text, float spacing, float size = 16, Color? modulate = null,
        TextOutline? outline = null
    )
    {
        modulate ??= Colors.White;
        
        var transform = GetFinalTransform();
        Raylib.SetTextLineSpacing((int)size);
        
        Rlgl.PushMatrix();
        Rlgl.MultMatrixf(transform);
        if (outline.HasValue)
        {
            var thickness = outline.Value.Size;
            var outlineColor = outline.Value.Color;
            for (var dx = -thickness; dx <= thickness; dx++)
            {
                for (var dy = -thickness; dy <= thickness; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    Raylib.DrawTextEx(
                        font, text, position + new Vector2(dx, dy), size, spacing, 
                        outlineColor * FinalModulate
                    );
                }
            }
        }
        Raylib.DrawTextEx(font, text, position, size, spacing, modulate.Value * FinalModulate);
        Rlgl.PopMatrix();
    }
    
    public void DrawTextureRect(_Texture2D texture, Rect2 rect, Color color)
    {
        DrawTextureRectRegion(texture, rect, new Rect2(0, 0, texture.Width, texture.Height), color);
    }

    public void DrawTextureRectRegion(_Texture2D texture, Rect2 rect, Rect2 sourceRect, Color modulate)
    {
        var transform = GetFinalTransform();
        
        var topLeft = rect.Position;
        var topRight = new Vector2(rect.Position.X + rect.Size.X, rect.Position.Y);
        var bottomRight = new Vector2(rect.Position.X + rect.Size.X, rect.Position.Y + rect.Size.Y);
        var bottomLeft = new Vector2(rect.Position.X, rect.Position.Y + rect.Size.Y);   
        
        topLeft = transform.Origin + transform.BasisXform(topLeft);
        topRight = transform.Origin + transform.BasisXform(topRight);
        bottomRight = transform.Origin + transform.BasisXform(bottomRight);
        bottomLeft = transform.Origin + transform.BasisXform(bottomLeft);

        var flipX = false;
        if (sourceRect.Size.X < 0)
        {
            flipX = true;
            sourceRect.Size = new Vector2(-sourceRect.Size.X, sourceRect.Size.Y);
        }

        if (sourceRect.Size.Y < 0)
        {
            sourceRect.Position = new Vector2(sourceRect.Position.X, sourceRect.Position.Y - sourceRect.Size.Y);
        }

        rect.Size = new Vector2(rect.Size.X.Abs(), rect.Size.Y.Abs());
        
        var width = texture.Width;
        var height = texture.Height;
        
        var sourceX = sourceRect.Position.X;
        var sourceY = sourceRect.Position.Y;
        var sourceWidth = sourceRect.Size.X;
        var sourceHeight = sourceRect.Size.Y;
        
        Rlgl.SetTexture(texture.Id);
        Rlgl.Begin(DrawMode.Quads);
        
        var finalModulate = modulate * FinalModulate;
        Rlgl.Color4ub(finalModulate.R8B, finalModulate.G8B, finalModulate.B8B, finalModulate.A8B);
        Rlgl.Normal3f(0.0f, 0.0f, 1.0f);
        
        if (flipX) Rlgl.TexCoord2f((sourceX + sourceWidth)/width, sourceY/height);
        else Rlgl.TexCoord2f(sourceX/width, sourceY/height);
        Rlgl.Vertex2f(topLeft.X, topLeft.Y);

        // Bottom-left corner for texture and quad
        if (flipX) Rlgl.TexCoord2f((sourceX + sourceWidth)/width, (sourceY + sourceHeight)/height);
        else Rlgl.TexCoord2f(sourceX/width, (sourceY + sourceHeight)/height);
        Rlgl.Vertex2f(bottomLeft.X, bottomLeft.Y);

        // Bottom-right corner for texture and quad
        if (flipX) Rlgl.TexCoord2f(sourceX/width, (sourceY + sourceHeight)/height);
        else Rlgl.TexCoord2f((sourceX + sourceWidth)/width, (sourceY + sourceHeight)/height);
        Rlgl.Vertex2f(bottomRight.X, bottomRight.Y);
        
        // Top-right corner for texture and quad
        if (flipX) Rlgl.TexCoord2f(sourceX/width, sourceY/height);
        else Rlgl.TexCoord2f((sourceX + sourceWidth)/width, sourceY/height);
        Rlgl.Vertex2f(topRight.X, topRight.Y);
        
        Rlgl.End();
        Rlgl.SetTexture(0);
    }

    public void DrawFps()
    {
        var transform = GetFinalTransform();
        Raylib.DrawFPS(transform.Origin.X.ToInt(), transform.Origin.Y.ToInt());
    }

    public void SetFinalTransform(Transform2D transform)
    {
        _transform = transform;
    }

    public void OnDraw(Action<IDrawContext> onDraw)
    {
        _onDraw = onDraw;
    }
    
    private Transform2D GetFinalTransform()
    {
        return _transform;
    }
}

public readonly record struct TextOutline(float Size, Color Color);

public interface IDrawContext
{
    /// <summary>
    /// <para>Draws a line from a 2D point to another, with a given color and width.</para>
    /// <para>If <paramref name="width"/> is negative, this means that when the CanvasItem is scaled, the line will remain thin. If this behavior is not desired, then pass a positive <paramref name="width"/> like <c>1.0</c>.</para>
    /// </summary>
    void DrawLine(Vector2 from, Vector2 to, Color color, float width=-1);
    
    /// <summary>
    /// <para>Draws a rect. </para>
    /// <para>If <paramref name="filled"/> is <see langword="true"/>, the rect will be filled with the <paramref name="color"/> specified. If <paramref name="filled"/> is <see langword="false"/>, the rect will be drawn as a stroke with the <paramref name="color"/> and <paramref name="width"/> specified.</para>
    /// <para>If <paramref name="width"/> is negative, this means that when the CanvasItem is scaled, the lines will remain thin. If this behavior is not desired, then pass a positive <paramref name="width"/> like <c>1.0</c>.</para>
    /// <para><b>Note:</b> <paramref name="width"/> is only effective if <paramref name="filled"/> is <see langword="false"/>.</para>
    /// </summary>
    void DrawRect(Rect2 rect, Color color, bool filled = true, float width = -1f);
    
    /// <summary>
    /// <para>Draws a rounded rect. </para>
    /// <para>If <paramref name="filled"/> is <see langword="true"/>, the rect will be filled with the <paramref name="color"/> specified. If <paramref name="filled"/> is <see langword="false"/>, the rect will be drawn as a stroke with the <paramref name="color"/> and <paramref name="width"/> specified.</para>
    /// <para>If <paramref name="width"/> is negative, this means that when the CanvasItem is scaled, the lines will remain thin. If this behavior is not desired, then pass a positive <paramref name="width"/> like <c>1.0</c>.</para>
    /// <para><b>Note:</b> <paramref name="width"/> is only effective if <paramref name="filled"/> is <see langword="false"/>.</para>
    /// </summary>
    void DrawRectRounded(Rect2 rect, Color color, float roundness, int segments, bool filled = true, float width = -1f);
    
    /// <summary>
    /// <para>Draws a circle. </para>
    /// <para>If <paramref name="filled"/> is <see langword="true"/>, the circle will be filled with the <paramref name="color"/> specified. If <paramref name="filled"/> is <see langword="false"/>, the circle will be drawn as a stroke with the <paramref name="color"/> and <paramref name="width"/> specified.</para>
    /// <para>If <paramref name="width"/> is negative, this means that when the CanvasItem is scaled, the lines will remain thin. If this behavior is not desired, then pass a positive <paramref name="width"/> like <c>1.0</c>.</para>
    /// <para><b>Note:</b> <paramref name="width"/> is only effective if <paramref name="filled"/> is <see langword="false"/>.</para>
    /// </summary>
    void DrawCircle(Vector2 position, float radius, Color color, bool filled = true, float width = -1f);

    void DrawText(
        _Font font, Vector2 position, string text, float spacing, float size = 16, Color? modulate = null, 
        TextOutline? outline = null
    );
    
    internal void DrawTextureRect(_Texture2D texture, Rect2 rect, Color modulate);
    
    internal void DrawTextureRectRegion(_Texture2D texture, Rect2 rect, Rect2 sourceRect, Color modulate);
    
    void DrawFps();
}

public interface IMaterialTarget
{
    Color Modulate { get; }
    
    public void Draw();
}