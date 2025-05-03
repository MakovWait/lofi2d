using Lofi2D.Math;

namespace Lofi2D.Audio;

public readonly record struct Decibels(float Value)
{
    public float Linear => Mathf.DbToLinear(Value);

    public static Decibels operator +(Decibels a, Decibels b) => new(a.Value + b.Value);
    
    public static Decibels operator -(Decibels a, Decibels b) => new(a.Value - b.Value);
    
    public static implicit operator Decibels(float value) => new(value);
    
    public static Decibels FromLinear(float linear) => new(Mathf.LinearToDb(linear));
    
    public static Decibels Default => new(0.0f);
    
    public static Decibels Mute => FromLinear(0.0f);
}

public static class DecibelsEx
{
    public static Decibels LinToDb(this float linear) => Decibels.FromLinear(linear);
    
    public static Decibels LinToDb(this int linear) => Decibels.FromLinear(linear);
}
