namespace Lofi2D.Math;

/// <summary>
/// Range of float values.
/// </summary>
public readonly struct Range(float start, float end)
{
    public readonly float Start = start;

    public readonly float End = end;

    public bool Contains(float v) => v >= Start && v <= End;

    public float Lerp(float progress)
    {
        return Mathf.Lerp(Start, End, progress);
    }

    public Range Clamp(float min, float max)
    {
        return new Range(Mathf.Clamp(Start, min, max), Mathf.Clamp(End, min, max));
    }
}