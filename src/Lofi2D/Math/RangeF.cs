namespace Lofi2D.Math;

/// <summary>
/// Range of float values.
/// </summary>
public readonly struct RangeF(float start, float end)
{
    public float Start { get; init; } = start;
    public float End { get; init; } = end;

    public bool Contains(float v) => v >= Start && v <= End;

    public float Lerp(float progress)
    {
        return Mathf.Lerp(Start, End, progress);
    }

    public RangeF Clamped(float min, float max)
    {
        return new RangeF(Mathf.Clamp(Start, min, max), Mathf.Clamp(End, min, max));
    }
    
    public RangeF Ordered => new(
        Mathf.Min(Start, End),
        Mathf.Max(Start, End)
    );
    
    public static bool operator ==(RangeF left, RangeF right) => left.Equals(right);
    
    public static bool operator !=(RangeF left, RangeF right) => !left.Equals(right);
    
    public bool Equals(RangeF other)
    {
        return Start == other.Start &&
               End == other.End;
    }

    public bool IsEqualApprox(RangeF other)
    {
        return Mathf.IsEqualApprox(Start, other.Start) &&
               Mathf.IsEqualApprox(End, other.End);
    }
    
    public override string ToString()
    {
        return $"[{Start}, {End}]";
    }
}