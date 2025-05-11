using System.Diagnostics;
using Lofi2D.Core;
using Lofi2D.Math;
using Lofi2D.Math.Components;
using Lofi2D.Render;

namespace Lofi2D.GUI;

public class Control
{
    public enum VGrowDirection
    {
        Top,
        Bottom,
        Both
    }

    public enum HGrowDirection
    {
        Left,
        Right,
        Both
    }

    public enum LayoutMode
    {
        Position,
        Anchors,
        Container
    }

    public LayoutMode Layout { get; set; } = LayoutMode.Position;

    public AnchorPoints Anchor
    {
        get => _anchor;
        set => SetAnchor(value);
    }

    public AnchorOffsets Offset
    {
        get => _offset;
        set => SetOffset(value);
    }

    public HGrowDirection HGrow
    {
        get => _hGrow;
        set => SetHGrowDirection(value);
    }

    public VGrowDirection VGrow
    {
        get => _vGrow;
        set => SetVGrowDirection(value);
    }

    public Vector2 Position
    {
        get => _position;
        set => SetPosition(value);
    }

    public Vector2 Size
    {
        get => _size;
        set => SetSize(value);
    }

    public Vector2 MinSize { get; set; } = Vector2.Zero;

    public Rect2 Rect => new Rect2(_position, _size);

    private AnchorPoints _anchor = AnchorPoints.Zero;
    private HGrowDirection _hGrow = HGrowDirection.Right;
    private VGrowDirection _vGrow = VGrowDirection.Bottom;
    private Vector2 _position;
    private AnchorOffsets _offset = new(0, 0, 0, 0);
    private Vector2 _size;

    private readonly CNode2DTransform _transform;
    private readonly CanvasItem _canvasItem;
    private readonly Children _children;
    private readonly Parent _parent;

    public Control(CanvasItem canvasItem, CNode2DTransform transform)
    {
        _transform = transform;
        _canvasItem = canvasItem;
        _children = new Children(this);
        _parent = new Parent(this);
        _canvasItem.OnDraw(Draw);
    }

    public void PreDraw()
    {
        // UpdateRectFromAnchors();
        _transform.Local = new Transform2D(0, _position);
        _canvasItem.SetFinalTransform(_transform.Global);
    }

    public void Draw(IDrawContext ctx)
    {
        ctx.DrawRect(new Rect2(new Vector2(), _size), Colors.White, false);
    }

    public void AddChild(Control child)
    {
        _children.Add(child);
    }

    public void RemoveChild(Control child)
    {
        _children.Remove(child);
    }

    private void SizeChanged()
    {
        if (_parent.Value == null)
            return;

        var parentRect = GetParentAnchorableRect();

        var left = Anchor.Left(parentRect) + Offset.Left;
        var top = Anchor.Top(parentRect) + Offset.Top;
        var right = Anchor.Right(parentRect) + Offset.Right;
        var bottom = Anchor.Bottom(parentRect) + Offset.Bottom;

        var width = Mathf.Max(right - left, MinSize.X);
        var height = Mathf.Max(bottom - top, MinSize.Y);

        _size.X = width;
        _position.X = HGrow switch
        {
            HGrowDirection.Left => right - width,
            HGrowDirection.Right => left,
            HGrowDirection.Both => (left + right) / 2 - width / 2,
            _ => throw new ArgumentOutOfRangeException()
        };

        _size.Y = height;
        _position.Y = VGrow switch
        {
            VGrowDirection.Top => bottom - height,
            VGrowDirection.Bottom => top,
            VGrowDirection.Both => (top + bottom) / 2 - height / 2,
            _ => throw new ArgumentOutOfRangeException()
        };

        _children.PropagateSizeChanged();
    }

    private void SetRect(Rect2 rect)
    {
        Anchor = AnchorPoints.Zero;
        ComputeOffsets(rect);
        SizeChanged();
    }

    private void SetPosition(Vector2 position)
    {
        if (_position == position)
        {
            return;
        }
        _position = position;
        ComputeOffsets(Rect);
        SizeChanged();
    }

    private void SetSize(Vector2 size)
    {
        size = size.Max(MinSize);
        if (_size == size)
        {
            return;
        }
        _size = size;
        ComputeOffsets(Rect);
        SizeChanged();
    }

    private void SetOffset(AnchorOffsets value)
    {
        if (_offset == value)
        {
            return;
        }
        _offset = value;
        SizeChanged();
    }

    private void SetAnchor(AnchorPoints value)
    {
        if (_anchor == value)
        {
            return;
        }
        _anchor = value;
        SizeChanged();
    }

    private void SetHGrowDirection(HGrowDirection value)
    {
        if (_hGrow == value)
        {
            return;
        }
        _hGrow = value;
        SizeChanged();
    }

    private void SetVGrowDirection(VGrowDirection value)
    {
        if (_vGrow == value)
        {
            return;
        }
        _vGrow = value;
        SizeChanged();
    }

    private void ComputeOffsets(Rect2 rect)
    {
        var parentRectSize = GetParentAnchorableRect().Size;
        var x = rect.Position.X;
        var y = rect.Position.Y;
        _offset = new AnchorOffsets(
            x - (Anchor[Side.Left] * parentRectSize.X),
            y - (Anchor[Side.Top] * parentRectSize.Y),
            x + rect.Size.X - (Anchor[Side.Right] * parentRectSize.X),
            y + rect.Size.Y - (Anchor[Side.Bottom] * parentRectSize.Y)
        );
    }

    private Rect2 GetAnchorableRect()
    {
        return new Rect2(new Vector2(), _size);
    }

    private Rect2 GetParentAnchorableRect()
    {
        if (_parent.Value == null)
            return new Rect2();

        return _parent.Value.GetAnchorableRect();
    }

    private class Children(Control self)
    {
        private readonly List<Control> _children = [];

        public void Add(Control child)
        {
            Debug.Assert(child._parent.Value == null);
            ;
            _children.Add(child);
            child._parent.Value = self;
        }

        public void Remove(Control child)
        {
            Debug.Assert(child._parent.Value == self);
            _children.Remove(child);
            child._parent.Value = null;
        }

        public void PropagateSizeChanged()
        {
            foreach (var control in _children)
            {
                control.SizeChanged();
            }
        }
    }

    private class Parent(Control self)
    {
        public Control? Value { get; set; }
    }
}

public readonly struct AnchorPoints(RangeF x, RangeF y)
{
    public static readonly AnchorPoints Zero = new();

    public AnchorPoints(float left, float top, float right, float bottom) : this(new RangeF(left, right), new RangeF(top, bottom))
    {
    }

    public RangeF X
    {
        get => x.Ordered.Clamped(0, 1);
        init => x = value;
    }

    public RangeF Y
    {
        get => y.Ordered.Clamped(0, 1);
        init => y = value;
    }

    public Rect2 Rect(Rect2 rect)
    {
        var left = Left(rect);
        var top = Top(rect);
        var right = Right(rect);
        var bottom = Bottom(rect);
        return new Rect2(new Vector2(left, top), new Vector2(right - left, bottom - top));
    }

    public float this[Side side]
    {
        get
        {
            return side switch
            {
                Side.Left => X.Start,
                Side.Top => Y.Start,
                Side.Right => X.End,
                Side.Bottom => Y.End,
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
            };
        }
    }

    public float Left(Rect2 rect) => rect.Position.X + rect.Size.X * X.Start;

    public float Top(Rect2 rect) => rect.Position.Y + rect.Size.Y * Y.Start;

    public float Right(Rect2 rect) => rect.Position.X + rect.Size.X * X.End;

    public float Bottom(Rect2 rect) => rect.Position.Y + rect.Size.Y * Y.End;

    public static bool operator ==(AnchorPoints left, AnchorPoints right) => left.Equals(right);

    public static bool operator !=(AnchorPoints left, AnchorPoints right) => !left.Equals(right);

    public bool Equals(AnchorPoints other)
    {
        return X == other.X &&
               Y == other.Y;
    }

    public bool IsEqualApprox(AnchorPoints other)
    {
        return X.IsEqualApprox(other.X) &&
               Y.IsEqualApprox(other.Y);
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}

public readonly struct AnchorOffsets(float left, float top, float right, float bottom)
{
    public float Top { get; init; } = top;
    public float Bottom { get; init; } = bottom;
    public float Left { get; init; } = left;
    public float Right { get; init; } = right;

    public AnchorOffsets() : this(0f, 0f, 0f, 0f)
    {
    }

    public AnchorOffsets(float offset) : this(offset, offset, offset, offset)
    {
    }

    public AnchorOffsets(float horizontal, float vertical) : this(vertical, horizontal, vertical, horizontal)
    {
    }

    public AnchorOffsets(Vector2 horizontal, Vector2 vertical) : this(vertical.X, horizontal.Y, vertical.Y, horizontal.X)
    {
    }

    public static bool operator ==(AnchorOffsets left, AnchorOffsets right) => left.Equals(right);

    public static bool operator !=(AnchorOffsets left, AnchorOffsets right) => !left.Equals(right);

    public bool Equals(AnchorOffsets other)
    {
        return Left == other.Left &&
               Right == other.Right &&
               Top == other.Top &&
               Bottom == other.Bottom;
    }

    public bool IsEqualApprox(AnchorOffsets other)
    {
        return Mathf.IsEqualApprox(Left, other.Left) &&
               Mathf.IsEqualApprox(Right, other.Right) &&
               Mathf.IsEqualApprox(Top, other.Top) &&
               Mathf.IsEqualApprox(Bottom, other.Bottom);
    }

    public override string ToString()
    {
        return $"({Left}, {Top}, {Right}, {Bottom})";
    }
}