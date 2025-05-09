namespace Lofi2D.Math;

public readonly struct Margins(float left, float top, float right, float bottom)
{
    public readonly float Top = top;
    public readonly float Bottom = bottom;
    public readonly float Left = left;
    public readonly float Right = right;

    public Margins() : this(0f, 0f, 0f, 0f)
    {
    }

    public Margins(float margin) : this(margin, margin, margin, margin)
    {
    }

    public Margins(float horizontal, float vertical) : this(vertical, horizontal, vertical, horizontal)
    {
    }

    public Margins(Vector2 horizontal, Vector2 vertical) : this(vertical.X, horizontal.Y, vertical.Y, horizontal.X)
    {
    }

    public Rect2 Apply(Rect2 rect)
    {
        return rect.GrowIndividual(Top, Right, Bottom, Left);
    }
}