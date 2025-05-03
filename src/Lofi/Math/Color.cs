using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Lofi.Util;

namespace Lofi.Math;

public struct Color : IEquatable<Color>
{
    /// <summary>
    /// The color's red component, typically on the range of 0 to 1.
    /// </summary>
    public float R;

    /// <summary>
    /// The color's green component, typically on the range of 0 to 1.
    /// </summary>
    public float G;

    /// <summary>
    /// The color's blue component, typically on the range of 0 to 1.
    /// </summary>
    public float B;

    /// <summary>
    /// The color's alpha component, typically on the range of 0 to 1.
    /// A value of 0 means that the color is fully transparent.
    /// A value of 1 means that the color is fully opaque.
    /// </summary>
    public float A;

    /// <summary>
    /// Wrapper for <see cref="F:Godot.Color.R" /> that uses the range 0 to 255 instead of 0 to 1.
    /// </summary>
    /// <value>Getting is equivalent to multiplying by 255 and rounding. Setting is equivalent to dividing by 255.</value>
    public int R8
    {
        readonly get => (int)Mathf.Round((double)this.R * (double)byte.MaxValue);
        set => this.R = (float)value / (float)byte.MaxValue;
    }

    /// <summary>
    /// Wrapper for <see cref="F:Godot.Color.G" /> that uses the range 0 to 255 instead of 0 to 1.
    /// </summary>
    /// <value>Getting is equivalent to multiplying by 255 and rounding. Setting is equivalent to dividing by 255.</value>
    public int G8
    {
        readonly get => (int)Mathf.Round((double)this.G * (double)byte.MaxValue);
        set => this.G = (float)value / (float)byte.MaxValue;
    }

    /// <summary>
    /// Wrapper for <see cref="F:Godot.Color.B" /> that uses the range 0 to 255 instead of 0 to 1.
    /// </summary>
    /// <value>Getting is equivalent to multiplying by 255 and rounding. Setting is equivalent to dividing by 255.</value>
    public int B8
    {
        readonly get => (int)Mathf.Round((double)this.B * (double)byte.MaxValue);
        set => this.B = (float)value / (float)byte.MaxValue;
    }

    /// <summary>
    /// Wrapper for <see cref="F:Godot.Color.A" /> that uses the range 0 to 255 instead of 0 to 1.
    /// </summary>
    /// <value>Getting is equivalent to multiplying by 255 and rounding. Setting is equivalent to dividing by 255.</value>
    public int A8
    {
        readonly get => (int)Mathf.Round((double)this.A * (double)byte.MaxValue);
        set => this.A = (float)value / (float)byte.MaxValue;
    }

    public byte R8B => (byte)R8;
    public byte G8B => (byte)G8;
    public byte B8B => (byte)B8;
    public byte A8B => (byte)A8;
    
    /// <summary>The HSV hue of this color, on the range 0 to 1.</summary>
    /// <value>Getting is a long process, refer to the source code for details. Setting uses <see cref="M:Godot.Color.FromHsv(System.Single,System.Single,System.Single,System.Single)" />.</value>
    public float H
    {
        readonly get
        {
            float num1 = Mathf.Max(this.R, Mathf.Max(this.G, this.B));
            float num2 = Mathf.Min(this.R, Mathf.Min(this.G, this.B));
            float num3 = num1 - num2;
            if ((double)num3 == 0.0)
                return 0.0f;
            float h = ((double)this.R != (double)num1
                ? ((double)this.G != (double)num1 ? (float)(4.0 + ((double)this.R - (double)this.G) / (double)num3) : (float)(2.0 + ((double)this.B - (double)this.R) / (double)num3))
                : (this.G - this.B) / num3) / 6f;
            if ((double)h < 0.0)
                ++h;
            return h;
        }
        set => this = Color.FromHsv(value, this.S, this.V, this.A);
    }

    /// <summary>The HSV saturation of this color, on the range 0 to 1.</summary>
    /// <value>Getting is equivalent to the ratio between the min and max RGB value. Setting uses <see cref="M:Godot.Color.FromHsv(System.Single,System.Single,System.Single,System.Single)" />.</value>
    public float S
    {
        readonly get
        {
            float num1 = Mathf.Max(this.R, Mathf.Max(this.G, this.B));
            float num2 = Mathf.Min(this.R, Mathf.Min(this.G, this.B));
            float num3 = num1 - num2;
            return (double)num1 != 0.0 ? num3 / num1 : 0.0f;
        }
        set => this = Color.FromHsv(this.H, value, this.V, this.A);
    }

    /// <summary>
    /// The HSV value (brightness) of this color, on the range 0 to 1.
    /// </summary>
    /// <value>Getting is equivalent to using <see cref="M:System.Mathf.Max(System.Single,System.Single)" /> on the RGB components. Setting uses <see cref="M:Godot.Color.FromHsv(System.Single,System.Single,System.Single,System.Single)" />.</value>
    public float V
    {
        readonly get => Mathf.Max(this.R, Mathf.Max(this.G, this.B));
        set => this = Color.FromHsv(this.H, this.S, value, this.A);
    }

    /// <summary>
    /// Returns the light intensity of the color, as a value between 0.0 and 1.0 (inclusive).
    /// This is useful when determining light or dark color. Colors with a luminance smaller
    /// than 0.5 can be generally considered dark.
    /// Note: <see cref="P:Godot.Color.Luminance" /> relies on the color being in the linear color space to
    /// return an accurate relative luminance value. If the color is in the sRGB color space
    /// use <see cref="M:Godot.Color.SrgbToLinear" /> to convert it to the linear color space first.
    /// </summary>
    public readonly float Luminance
    {
        get { return (float)(0.2125999927520752 * (double)this.R + 0.7152000069618225 * (double)this.G + 0.0722000002861023 * (double)this.B); }
    }

    /// <summary>Access color components using their index.</summary>
    /// <value>
    /// <c>[0]</c> is equivalent to <see cref="F:Godot.Color.R" />,
    /// <c>[1]</c> is equivalent to <see cref="F:Godot.Color.G" />,
    /// <c>[2]</c> is equivalent to <see cref="F:Godot.Color.B" />,
    /// <c>[3]</c> is equivalent to <see cref="F:Godot.Color.A" />.
    /// </value>
    public float this[int index]
    {
        readonly get
        {
            switch (index)
            {
                case 0:
                    return this.R;
                case 1:
                    return this.G;
                case 2:
                    return this.B;
                case 3:
                    return this.A;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
        set
        {
            switch (index)
            {
                case 0:
                    this.R = value;
                    break;
                case 1:
                    this.G = value;
                    break;
                case 2:
                    this.B = value;
                    break;
                case 3:
                    this.A = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }

    /// <summary>
    /// Returns a new color resulting from blending this color over another.
    /// If the color is opaque, the result is also opaque.
    /// The second color may have a range of alpha values.
    /// </summary>
    /// <param name="over">The color to blend over.</param>
    /// <returns>This color blended over <paramref name="over" />.</returns>
    public readonly Color Blend(Color over)
    {
        float num = 1f - over.A;
        Color color;
        color.A = this.A * num + over.A;
        if ((double)color.A == 0.0)
            return new Color(0.0f, 0.0f, 0.0f, 0.0f);
        color.R = (float)((double)this.R * (double)this.A * (double)num + (double)over.R * (double)over.A) / color.A;
        color.G = (float)((double)this.G * (double)this.A * (double)num + (double)over.G * (double)over.A) / color.A;
        color.B = (float)((double)this.B * (double)this.A * (double)num + (double)over.B * (double)over.A) / color.A;
        return color;
    }

    /// <summary>
    /// Returns a new color with all components clamped between the
    /// components of <paramref name="min" /> and <paramref name="max" />
    /// using <see cref="M:Godot.Mathf.Clamp(System.Single,System.Single,System.Single)" />.
    /// </summary>
    /// <param name="min">The color with minimum allowed values.</param>
    /// <param name="max">The color with maximum allowed values.</param>
    /// <returns>The color with all components clamped.</returns>
    public readonly Color Clamp(Color? min = null, Color? max = null)
    {
        Color color1 = min ?? new Color(0.0f, 0.0f, 0.0f, 0.0f);
        Color color2 = max ?? new Color(1f, 1f, 1f);
        return new Color(Mathf.Clamp(this.R, color1.R, color2.R), Mathf.Clamp(this.G, color1.G, color2.G), Mathf.Clamp(this.B, color1.B, color2.B), Mathf.Clamp(this.A, color1.A, color2.A));
    }

    /// <summary>
    /// Returns a new color resulting from making this color darker
    /// by the specified ratio (on the range of 0 to 1).
    /// </summary>
    /// <param name="amount">The ratio to darken by.</param>
    /// <returns>The darkened color.</returns>
    public readonly Color Darkened(float amount)
    {
        Color color = this;
        color.R *= 1f - amount;
        color.G *= 1f - amount;
        color.B *= 1f - amount;
        return color;
    }

    /// <summary>
    /// Returns the inverted color: <c>(1 - r, 1 - g, 1 - b, a)</c>.
    /// </summary>
    /// <returns>The inverted color.</returns>
    public readonly Color Inverted() => new Color(1f - this.R, 1f - this.G, 1f - this.B, this.A);

    /// <summary>
    /// Returns a new color resulting from making this color lighter
    /// by the specified ratio (on the range of 0 to 1).
    /// </summary>
    /// <param name="amount">The ratio to lighten by.</param>
    /// <returns>The lightened color.</returns>
    public readonly Color Lightened(float amount)
    {
        Color color = this;
        color.R += (1f - color.R) * amount;
        color.G += (1f - color.G) * amount;
        color.B += (1f - color.B) * amount;
        return color;
    }

    /// <summary>
    /// Returns the result of the linear interpolation between
    /// this color and <paramref name="to" /> by amount <paramref name="weight" />.
    /// </summary>
    /// <param name="to">The destination color for interpolation.</param>
    /// <param name="weight">A value on the range of 0.0 to 1.0, representing the amount of interpolation.</param>
    /// <returns>The resulting color of the interpolation.</returns>
    public readonly Color Lerp(Color to, float weight)
    {
        return new Color(Mathf.Lerp(this.R, to.R, weight), Mathf.Lerp(this.G, to.G, weight), Mathf.Lerp(this.B, to.B, weight), Mathf.Lerp(this.A, to.A, weight));
    }

    /// <summary>
    /// Returns the color converted to the sRGB color space.
    /// This method assumes the original color is in the linear color space.
    /// See also <see cref="M:Godot.Color.SrgbToLinear" /> which performs the opposite operation.
    /// </summary>
    /// <returns>The sRGB color.</returns>
    public readonly Color LinearToSrgb()
    {
        return new Color((double)this.R < 0.0031308000907301903 ? 12.92f * this.R : (float)(1.0549999475479126 * (double)Mathf.Pow(this.R, 0.41666666f) - 0.054999999701976776),
            (double)this.G < 0.0031308000907301903 ? 12.92f * this.G : (float)(1.0549999475479126 * (double)Mathf.Pow(this.G, 0.41666666f) - 0.054999999701976776),
            (double)this.B < 0.0031308000907301903 ? 12.92f * this.B : (float)(1.0549999475479126 * (double)Mathf.Pow(this.B, 0.41666666f) - 0.054999999701976776), this.A);
    }

    /// <summary>
    /// Returns the color converted to linear color space.
    /// This method assumes the original color already is in sRGB color space.
    /// See also <see cref="M:Godot.Color.LinearToSrgb" /> which performs the opposite operation.
    /// </summary>
    /// <returns>The color in linear color space.</returns>
    public readonly Color SrgbToLinear()
    {
        return new Color((double)this.R < 0.040449999272823334 ? this.R * 0.07739938f : Mathf.Pow((float)(((double)this.R + 0.054999999701976776) * 0.9478672742843628), 2.4f),
            (double)this.G < 0.040449999272823334 ? this.G * 0.07739938f : Mathf.Pow((float)(((double)this.G + 0.054999999701976776) * 0.9478672742843628), 2.4f),
            (double)this.B < 0.040449999272823334 ? this.B * 0.07739938f : Mathf.Pow((float)(((double)this.B + 0.054999999701976776) * 0.9478672742843628), 2.4f), this.A);
    }

    /// <summary>
    /// Returns the color converted to an unsigned 32-bit integer in ABGR
    /// format (each byte represents a color channel).
    /// ABGR is the reversed version of the default format.
    /// </summary>
    /// <returns>A <see langword="uint" /> representing this color in ABGR32 format.</returns>
    public readonly uint ToAbgr32()
    {
        return (uint)((((int)(byte)Mathf.Round((double)this.A * (double)byte.MaxValue) << 8 | (int)(byte)Mathf.Round((double)this.B * (double)byte.MaxValue)) << 8 |
                       (int)(byte)Mathf.Round((double)this.G * (double)byte.MaxValue)) << 8) | (uint)(byte)Mathf.Round((double)this.R * (double)byte.MaxValue);
    }

    /// <summary>
    /// Returns the color converted to an unsigned 64-bit integer in ABGR
    /// format (each word represents a color channel).
    /// ABGR is the reversed version of the default format.
    /// </summary>
    /// <returns>A <see langword="ulong" /> representing this color in ABGR64 format.</returns>
    public readonly ulong ToAbgr64()
    {
        return (ulong)((((long)(ushort)Mathf.Round((double)this.A * (double)ushort.MaxValue) << 16 /*0x10*/ | (long)(ushort)Mathf.Round((double)this.B * (double)ushort.MaxValue)) << 16 /*0x10*/ |
                        (long)(ushort)Mathf.Round((double)this.G * (double)ushort.MaxValue)) << 16 /*0x10*/) | (ulong)(ushort)Mathf.Round((double)this.R * (double)ushort.MaxValue);
    }

    /// <summary>
    /// Returns the color converted to an unsigned 32-bit integer in ARGB
    /// format (each byte represents a color channel).
    /// ARGB is more compatible with DirectX, but not used much in Godot.
    /// </summary>
    /// <returns>A <see langword="uint" /> representing this color in ARGB32 format.</returns>
    public readonly uint ToArgb32()
    {
        return (uint)((((int)(byte)Mathf.Round((double)this.A * (double)byte.MaxValue) << 8 | (int)(byte)Mathf.Round((double)this.R * (double)byte.MaxValue)) << 8 |
                       (int)(byte)Mathf.Round((double)this.G * (double)byte.MaxValue)) << 8) | (uint)(byte)Mathf.Round((double)this.B * (double)byte.MaxValue);
    }

    /// <summary>
    /// Returns the color converted to an unsigned 64-bit integer in ARGB
    /// format (each word represents a color channel).
    /// ARGB is more compatible with DirectX, but not used much in Godot.
    /// </summary>
    /// <returns>A <see langword="ulong" /> representing this color in ARGB64 format.</returns>
    public readonly ulong ToArgb64()
    {
        return (ulong)((((long)(ushort)Mathf.Round((double)this.A * (double)ushort.MaxValue) << 16 /*0x10*/ | (long)(ushort)Mathf.Round((double)this.R * (double)ushort.MaxValue)) << 16 /*0x10*/ |
                        (long)(ushort)Mathf.Round((double)this.G * (double)ushort.MaxValue)) << 16 /*0x10*/) | (ulong)(ushort)Mathf.Round((double)this.B * (double)ushort.MaxValue);
    }

    /// <summary>
    /// Returns the color converted to an unsigned 32-bit integer in RGBA
    /// format (each byte represents a color channel).
    /// RGBA is Godot's default and recommended format.
    /// </summary>
    /// <returns>A <see langword="uint" /> representing this color in RGBA32 format.</returns>
    public readonly uint ToRgba32()
    {
        return (uint)((((int)(byte)Mathf.Round((double)this.R * (double)byte.MaxValue) << 8 | (int)(byte)Mathf.Round((double)this.G * (double)byte.MaxValue)) << 8 |
                       (int)(byte)Mathf.Round((double)this.B * (double)byte.MaxValue)) << 8) | (uint)(byte)Mathf.Round((double)this.A * (double)byte.MaxValue);
    }

    /// <summary>
    /// Returns the color converted to an unsigned 64-bit integer in RGBA
    /// format (each word represents a color channel).
    /// RGBA is Godot's default and recommended format.
    /// </summary>
    /// <returns>A <see langword="ulong" /> representing this color in RGBA64 format.</returns>
    public readonly ulong ToRgba64()
    {
        return (ulong)((((long)(ushort)Mathf.Round((double)this.R * (double)ushort.MaxValue) << 16 /*0x10*/ | (long)(ushort)Mathf.Round((double)this.G * (double)ushort.MaxValue)) << 16 /*0x10*/ |
                        (long)(ushort)Mathf.Round((double)this.B * (double)ushort.MaxValue)) << 16 /*0x10*/) | (ulong)(ushort)Mathf.Round((double)this.A * (double)ushort.MaxValue);
    }

    /// <summary>
    /// Returns the color's HTML hexadecimal color string in RGBA format.
    /// </summary>
    /// <param name="includeAlpha">
    /// Whether or not to include alpha. If <see langword="false" />, the color is RGB instead of RGBA.
    /// </param>
    /// <returns>A string for the HTML hexadecimal representation of this color.</returns>
    public readonly string ToHtml(bool includeAlpha = true)
    {
        string html = string.Empty + Color.ToHex32(this.R) + Color.ToHex32(this.G) + Color.ToHex32(this.B);
        if (includeAlpha)
            html += Color.ToHex32(this.A);
        return html;
    }

    /// <summary>
    /// Constructs a <see cref="T:Godot.Color" /> from RGBA values, typically on the range of 0 to 1.
    /// </summary>
    /// <param name="r">The color's red component, typically on the range of 0 to 1.</param>
    /// <param name="g">The color's green component, typically on the range of 0 to 1.</param>
    /// <param name="b">The color's blue component, typically on the range of 0 to 1.</param>
    /// <param name="a">
    /// The color's alpha value, typically on the range of 0 to 1.
    /// A value of 0 means that the color is fully transparent.
    /// A value of 1 means that the color is fully opaque.
    /// </param>
    public Color(float r, float g, float b, float a = 1f)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = a;
    }

    /// <summary>
    /// Constructs a <see cref="T:Godot.Color" /> from an existing color and an alpha value.
    /// </summary>
    /// <param name="c">The color to construct from. Only its RGB values are used.</param>
    /// <param name="a">
    /// The color's alpha value, typically on the range of 0 to 1.
    /// A value of 0 means that the color is fully transparent.
    /// A value of 1 means that the color is fully opaque.
    /// </param>
    public Color(Color c, float a = 1f)
    {
        this.R = c.R;
        this.G = c.G;
        this.B = c.B;
        this.A = a;
    }

    /// <summary>
    /// Constructs a <see cref="T:Godot.Color" /> from an unsigned 32-bit integer in RGBA format
    /// (each byte represents a color channel).
    /// </summary>
    /// <param name="rgba">The <see langword="uint" /> representing the color as 0xRRGGBBAA.</param>
    public Color(uint rgba)
    {
        this.A = (float)(rgba & (uint)byte.MaxValue) / (float)byte.MaxValue;
        rgba >>= 8;
        this.B = (float)(rgba & (uint)byte.MaxValue) / (float)byte.MaxValue;
        rgba >>= 8;
        this.G = (float)(rgba & (uint)byte.MaxValue) / (float)byte.MaxValue;
        rgba >>= 8;
        this.R = (float)(rgba & (uint)byte.MaxValue) / (float)byte.MaxValue;
    }

    /// <summary>
    /// Constructs a <see cref="T:Godot.Color" /> from an unsigned 64-bit integer in RGBA format
    /// (each word represents a color channel).
    /// </summary>
    /// <param name="rgba">The <see langword="ulong" /> representing the color as 0xRRRRGGGGBBBBAAAA.</param>
    public Color(ulong rgba)
    {
        this.A = (float)(rgba & (ulong)ushort.MaxValue) / (float)ushort.MaxValue;
        rgba >>= 16 /*0x10*/;
        this.B = (float)(rgba & (ulong)ushort.MaxValue) / (float)ushort.MaxValue;
        rgba >>= 16 /*0x10*/;
        this.G = (float)(rgba & (ulong)ushort.MaxValue) / (float)ushort.MaxValue;
        rgba >>= 16 /*0x10*/;
        this.R = (float)(rgba & (ulong)ushort.MaxValue) / (float)ushort.MaxValue;
    }

    /// <summary>
    /// Constructs a <see cref="T:Godot.Color" /> either from an HTML color code or from a
    /// standardized color name. Supported color names are the same as the
    /// <see cref="T:Godot.Colors" /> constants.
    /// </summary>
    /// <param name="code">The HTML color code or color name to construct from.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// A color cannot be inferred from the given <paramref name="code" />.
    /// It was invalid HTML and a color with that name was not found.
    /// </exception>
    public Color(string code)
    {
        if (Color.HtmlIsValid((ReadOnlySpan<char>)code))
            this = Color.FromHtml((ReadOnlySpan<char>)code);
        else
            this = Color.Named(code);
    }

    /// <summary>
    /// Constructs a <see cref="T:Godot.Color" /> either from an HTML color code or from a
    /// standardized color name, with <paramref name="alpha" /> on the range of 0 to 1. Supported
    /// color names are the same as the <see cref="T:Godot.Colors" /> constants.
    /// </summary>
    /// <param name="code">The HTML color code or color name to construct from.</param>
    /// <param name="alpha">The alpha (transparency) value, typically on the range of 0 to 1.</param>
    public Color(string code, float alpha)
        : this(code)
    {
        this.A = alpha;
    }

    /// <summary>
    /// Constructs a <see cref="T:Godot.Color" /> from the HTML hexadecimal color string in RGBA format.
    /// </summary>
    /// <param name="rgba">A string for the HTML hexadecimal representation of this color.</param>
    /// <exception name="ArgumentOutOfRangeException">
    /// <paramref name="rgba" /> color code is invalid.
    /// </exception>
    public static Color FromHtml(ReadOnlySpan<char> rgba)
    {
        if (rgba.Length == 0)
        {
            Color color;
            color.R = 0.0f;
            color.G = 0.0f;
            color.B = 0.0f;
            color.A = 1f;
            return color;
        }
        if (rgba[0] == '#')
            rgba = rgba.Slice(1);
        int num = rgba.Length < 5 ? 1 : 0;
        bool flag;
        if (rgba.Length == 8)
            flag = true;
        else if (rgba.Length == 6)
            flag = false;
        else if (rgba.Length == 4)
            flag = true;
        else if (rgba.Length == 3)
            flag = false;
        else
            throw new ArgumentOutOfRangeException($"Invalid color code. Length is {rgba.Length}, but a length of 6 or 8 is expected: {rgba}");
        Color color1;
        color1.A = 1f;
        if (num != 0)
        {
            color1.R = (float)Color.ParseCol4(rgba, 0) / 15f;
            color1.G = (float)Color.ParseCol4(rgba, 1) / 15f;
            color1.B = (float)Color.ParseCol4(rgba, 2) / 15f;
            if (flag)
                color1.A = (float)Color.ParseCol4(rgba, 3) / 15f;
        }
        else
        {
            color1.R = (float)Color.ParseCol8(rgba, 0) / (float)byte.MaxValue;
            color1.G = (float)Color.ParseCol8(rgba, 2) / (float)byte.MaxValue;
            color1.B = (float)Color.ParseCol8(rgba, 4) / (float)byte.MaxValue;
            if (flag)
                color1.A = (float)Color.ParseCol8(rgba, 6) / (float)byte.MaxValue;
        }
        if ((double)color1.R < 0.0)
            throw new ArgumentOutOfRangeException($"Invalid color code. Red part is not valid hexadecimal: {rgba}");
        if ((double)color1.G < 0.0)
            throw new ArgumentOutOfRangeException($"Invalid color code. Green part is not valid hexadecimal: {rgba}");
        if ((double)color1.B < 0.0)
            throw new ArgumentOutOfRangeException($"Invalid color code. Blue part is not valid hexadecimal: {rgba}");
        return (double)color1.A >= 0.0 ? color1 : throw new ArgumentOutOfRangeException($"Invalid color code. Alpha part is not valid hexadecimal: {rgba}");
    }

    /// <summary>
    /// Returns a color constructed from integer red, green, blue, and alpha channels.
    /// Each channel should have 8 bits of information ranging from 0 to 255.
    /// </summary>
    /// <param name="r8">The red component represented on the range of 0 to 255.</param>
    /// <param name="g8">The green component represented on the range of 0 to 255.</param>
    /// <param name="b8">The blue component represented on the range of 0 to 255.</param>
    /// <param name="a8">The alpha (transparency) component represented on the range of 0 to 255.</param>
    /// <returns>The constructed color.</returns>
    public static Color Color8(byte r8, byte g8, byte b8, byte a8 = 255 /*0xFF*/)
    {
        return new Color((float)r8 / (float)byte.MaxValue, (float)g8 / (float)byte.MaxValue, (float)b8 / (float)byte.MaxValue, (float)a8 / (float)byte.MaxValue);
    }

    /// <summary>
    /// Returns a color according to the standardized name, with the
    /// specified alpha value. Supported color names are the same as
    /// the constants defined in <see cref="T:Godot.Colors" />.
    /// </summary>
    /// <param name="name">The name of the color.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// A color with the given name is not found.
    /// </exception>
    /// <returns>The constructed color.</returns>
    private static Color Named(string name)
    {
        Color color;
        if (!Color.FindNamedColor(name, out color))
            throw new ArgumentOutOfRangeException("Invalid Color Name: " + name);
        return color;
    }

    /// <summary>
    /// Returns a color according to the standardized name, with the
    /// specified alpha value. Supported color names are the same as
    /// the constants defined in <see cref="T:Godot.Colors" />.
    /// If a color with the given name is not found, it returns
    /// <paramref name="default" />.
    /// </summary>
    /// <param name="name">The name of the color.</param>
    /// <param name="default">
    /// The default color to return when a color with the given name
    /// is not found.
    /// </param>
    /// <returns>The constructed color.</returns>
    private static Color Named(string name, Color @default)
    {
        Color color;
        return !Color.FindNamedColor(name, out color) ? @default : color;
    }

    private static bool FindNamedColor(string name, out Color color)
    {
        name = name.Replace(" ", string.Empty, StringComparison.Ordinal);
        name = name.Replace("-", string.Empty, StringComparison.Ordinal);
        name = name.Replace("_", string.Empty, StringComparison.Ordinal);
        name = name.Replace("'", string.Empty, StringComparison.Ordinal);
        name = name.Replace(".", string.Empty, StringComparison.Ordinal);
        name = name.ToUpperInvariant();
        return Colors.NamedColors.TryGetValue(name, out color);
    }

    /// <summary>
    /// Constructs a color from an HSV profile. The <paramref name="hue" />,
    /// <paramref name="saturation" />, and <paramref name="value" /> are typically
    /// between 0.0 and 1.0.
    /// </summary>
    /// <param name="hue">The HSV hue, typically on the range of 0 to 1.</param>
    /// <param name="saturation">The HSV saturation, typically on the range of 0 to 1.</param>
    /// <param name="value">The HSV value (brightness), typically on the range of 0 to 1.</param>
    /// <param name="alpha">The alpha (transparency) value, typically on the range of 0 to 1.</param>
    /// <returns>The constructed color.</returns>
    public static Color FromHsv(float hue, float saturation, float value, float alpha = 1f)
    {
        if ((double)saturation == 0.0)
            return new Color(value, value, value, alpha);
        hue *= 6f;
        hue %= 6f;
        int num1 = (int)hue;
        float num2 = hue - (float)num1;
        float num3 = value * (1f - saturation);
        float num4 = value * (float)(1.0 - (double)saturation * (double)num2);
        float num5 = value * (float)(1.0 - (double)saturation * (1.0 - (double)num2));
        switch (num1)
        {
            case 0:
                return new Color(value, num5, num3, alpha);
            case 1:
                return new Color(num4, value, num3, alpha);
            case 2:
                return new Color(num3, value, num5, alpha);
            case 3:
                return new Color(num3, num4, value, alpha);
            case 4:
                return new Color(num5, num3, value, alpha);
            default:
                return new Color(value, num3, num4, alpha);
        }
    }

    /// <summary>
    /// Converts a color to HSV values. This is equivalent to using each of
    /// the <c>h</c>/<c>s</c>/<c>v</c> properties, but much more efficient.
    /// </summary>
    /// <param name="hue">Output parameter for the HSV hue.</param>
    /// <param name="saturation">Output parameter for the HSV saturation.</param>
    /// <param name="value">Output parameter for the HSV value.</param>
    public readonly void ToHsv(out float hue, out float saturation, out float value)
    {
        float num1 = Mathf.Max(this.R, Mathf.Max(this.G, this.B));
        float num2 = Mathf.Min(this.R, Mathf.Min(this.G, this.B));
        float num3 = num1 - num2;
        if ((double)num3 == 0.0)
        {
            hue = 0.0f;
        }
        else
        {
            hue = (double)this.R != (double)num1
                ? ((double)this.G != (double)num1 ? (float)(4.0 + ((double)this.R - (double)this.G) / (double)num3) : (float)(2.0 + ((double)this.B - (double)this.R) / (double)num3))
                : (this.G - this.B) / num3;
            hue /= 6f;
            if ((double)hue < 0.0)
                ++hue;
        }
        saturation = (double)num1 != 0.0 ? (float)(1.0 - (double)num2 / (double)num1) : 0.0f;
        value = num1;
    }

    private static int ParseCol4(ReadOnlySpan<char> str, int index)
    {
        char ch = str[index];
        if (ch >= '0' && ch <= '9')
            return (int)ch - 48 /*0x30*/;
        if (ch >= 'a' && ch <= 'f')
            return (int)ch - 87;
        return ch >= 'A' && ch <= 'F' ? (int)ch - 55 : -1;
    }

    private static int ParseCol8(ReadOnlySpan<char> str, int index)
    {
        return Color.ParseCol4(str, index) * 16 /*0x10*/ + Color.ParseCol4(str, index + 1);
    }

    // /// <summary>
    // /// Constructs a color from an OK HSL profile. The <paramref name="hue" />,
    // /// <paramref name="saturation" />, and <paramref name="lightness" /> are typically
    // /// between 0.0 and 1.0.
    // /// </summary>
    // /// <param name="hue">The OK HSL hue, typically on the range of 0 to 1.</param>
    // /// <param name="saturation">The OK HSL saturation, typically on the range of 0 to 1.</param>
    // /// <param name="lightness">The OK HSL lightness, typically on the range of 0 to 1.</param>
    // /// <param name="alpha">The alpha (transparency) value, typically on the range of 0 to 1.</param>
    // /// <returns>The constructed color.</returns>
    // TODO
    // public static Color FromOkHsl(float hue, float saturation, float lightness, float alpha = 1f)
    // {
    //     return NativeFuncs.godotsharp_color_from_ok_hsl(hue, saturation, lightness, alpha);
    // }

    /// <summary>
    /// Encodes a <see cref="T:Godot.Color" /> from a RGBE9995 format integer.
    /// See <see cref="F:Godot.Image.Format.Rgbe9995" />.
    /// </summary>
    /// <param name="rgbe">The RGBE9995 encoded color.</param>
    /// <returns>The constructed color.</returns>
    public static Color FromRgbe9995(uint rgbe)
    {
        double num1 = (double)(rgbe & 511U /*0x01FF*/);
        float num2 = (float)(rgbe >> 9 & 511U /*0x01FF*/);
        float num3 = (float)(rgbe >> 18 & 511U /*0x01FF*/);
        float num4 = Mathf.Pow(2f, (float)((double)(rgbe >> 27) - 15.0 - 9.0));
        double num5 = (double)num4;
        double r = num1 * num5;
        float num6 = num2 * num4;
        float num7 = num3 * num4;
        double g = (double)num6;
        double b = (double)num7;
        return new Color((float)r, (float)g, (float)b);
    }

    /// <summary>
    /// Constructs a color from the given string, which can be either an HTML color
    /// code or a named color. Returns <paramref name="default" /> if the color cannot
    /// be inferred from the string. Supported color names are the same as the
    /// <see cref="T:Godot.Colors" /> constants.
    /// </summary>
    /// <param name="str">The HTML color code or color name.</param>
    /// <param name="default">The fallback color to return if the color cannot be inferred.</param>
    /// <returns>The constructed color.</returns>
    public static Color FromString(string str, Color @default)
    {
        return Color.HtmlIsValid((ReadOnlySpan<char>)str) ? Color.FromHtml((ReadOnlySpan<char>)str) : Color.Named(str, @default);
    }

    private static string ToHex32(float val)
    {
        return ((byte)Mathf.RoundToInt(Mathf.Clamp(val * (float)byte.MaxValue, 0.0f, (float)byte.MaxValue))).HexEncode();
    }

    /// <summary>
    /// Returns <see langword="true" /> if <paramref name="color" /> is a valid HTML hexadecimal
    /// color string. The string must be a hexadecimal value (case-insensitive) of either 3,
    /// 4, 6 or 8 digits, and may be prefixed by a hash sign (<c>#</c>). This method is
    /// identical to <see cref="M:Godot.StringExtensions.IsValidHtmlColor(System.String)" />.
    /// </summary>
    /// <param name="color">The HTML hexadecimal color string.</param>
    /// <returns>Whether or not the string was a valid HTML hexadecimal color string.</returns>
    public static bool HtmlIsValid(ReadOnlySpan<char> color)
    {
        if (color.IsEmpty)
            return false;
        if (color[0] == '#')
            color = color.Slice(1);
        int length = color.Length;
        switch (length)
        {
            case 3:
            case 4:
            case 6:
            case 8:
                for (int index = 0; index < length; ++index)
                {
                    if (Color.ParseCol4(color, index) == -1)
                        return false;
                }
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Adds each component of the <see cref="T:Godot.Color" />
    /// with the components of the given <see cref="T:Godot.Color" />.
    /// </summary>
    /// <param name="left">The left color.</param>
    /// <param name="right">The right color.</param>
    /// <returns>The added color.</returns>
    public static Color operator +(Color left, Color right)
    {
        left.R += right.R;
        left.G += right.G;
        left.B += right.B;
        left.A += right.A;
        return left;
    }

    /// <summary>
    /// Subtracts each component of the <see cref="T:Godot.Color" />
    /// by the components of the given <see cref="T:Godot.Color" />.
    /// </summary>
    /// <param name="left">The left color.</param>
    /// <param name="right">The right color.</param>
    /// <returns>The subtracted color.</returns>
    public static Color operator -(Color left, Color right)
    {
        left.R -= right.R;
        left.G -= right.G;
        left.B -= right.B;
        left.A -= right.A;
        return left;
    }

    /// <summary>
    /// Inverts the given color. This is equivalent to
    /// <c>Colors.White - c</c> or
    /// <c>new Color(1 - c.R, 1 - c.G, 1 - c.B, 1 - c.A)</c>.
    /// </summary>
    /// <param name="color">The color to invert.</param>
    /// <returns>The inverted color.</returns>
    public static Color operator -(Color color) => Colors.White - color;

    /// <summary>
    /// Multiplies each component of the <see cref="T:Godot.Color" />
    /// by the given <see langword="float" />.
    /// </summary>
    /// <param name="color">The color to multiply.</param>
    /// <param name="scale">The value to multiply by.</param>
    /// <returns>The multiplied color.</returns>
    public static Color operator *(Color color, float scale)
    {
        color.R *= scale;
        color.G *= scale;
        color.B *= scale;
        color.A *= scale;
        return color;
    }

    /// <summary>
    /// Multiplies each component of the <see cref="T:Godot.Color" />
    /// by the given <see langword="float" />.
    /// </summary>
    /// <param name="scale">The value to multiply by.</param>
    /// <param name="color">The color to multiply.</param>
    /// <returns>The multiplied color.</returns>
    public static Color operator *(float scale, Color color)
    {
        color.R *= scale;
        color.G *= scale;
        color.B *= scale;
        color.A *= scale;
        return color;
    }

    /// <summary>
    /// Multiplies each component of the <see cref="T:Godot.Color" />
    /// by the components of the given <see cref="T:Godot.Color" />.
    /// </summary>
    /// <param name="left">The left color.</param>
    /// <param name="right">The right color.</param>
    /// <returns>The multiplied color.</returns>
    public static Color operator *(Color left, Color right)
    {
        left.R *= right.R;
        left.G *= right.G;
        left.B *= right.B;
        left.A *= right.A;
        return left;
    }

    /// <summary>
    /// Divides each component of the <see cref="T:Godot.Color" />
    /// by the given <see langword="float" />.
    /// </summary>
    /// <param name="color">The dividend vector.</param>
    /// <param name="scale">The divisor value.</param>
    /// <returns>The divided color.</returns>
    public static Color operator /(Color color, float scale)
    {
        color.R /= scale;
        color.G /= scale;
        color.B /= scale;
        color.A /= scale;
        return color;
    }

    /// <summary>
    /// Divides each component of the <see cref="T:Godot.Color" />
    /// by the components of the given <see cref="T:Godot.Color" />.
    /// </summary>
    /// <param name="left">The dividend color.</param>
    /// <param name="right">The divisor color.</param>
    /// <returns>The divided color.</returns>
    public static Color operator /(Color left, Color right)
    {
        left.R /= right.R;
        left.G /= right.G;
        left.B /= right.B;
        left.A /= right.A;
        return left;
    }

    /// <summary>
    /// Returns <see langword="true" /> if the colors are exactly equal.
    /// Note: Due to floating-point precision errors, consider using
    /// <see cref="M:Godot.Color.IsEqualApprox(Godot.Color)" /> instead, which is more reliable.
    /// </summary>
    /// <param name="left">The left color.</param>
    /// <param name="right">The right color.</param>
    /// <returns>Whether or not the colors are equal.</returns>
    public static bool operator ==(Color left, Color right) => left.Equals(right);

    /// <summary>
    /// Returns <see langword="true" /> if the colors are not equal.
    /// Note: Due to floating-point precision errors, consider using
    /// <see cref="M:Godot.Color.IsEqualApprox(Godot.Color)" /> instead, which is more reliable.
    /// </summary>
    /// <param name="left">The left color.</param>
    /// <param name="right">The right color.</param>
    /// <returns>Whether or not the colors are equal.</returns>
    public static bool operator !=(Color left, Color right) => !left.Equals(right);

    /// <summary>
    /// Compares two <see cref="T:Godot.Color" />s by first checking if
    /// the red value of the <paramref name="left" /> color is less than
    /// the red value of the <paramref name="right" /> color.
    /// If the red values are exactly equal, then it repeats this check
    /// with the green values of the two colors, then with the blue values,
    /// and then with the alpha value.
    /// This operator is useful for sorting colors.
    /// </summary>
    /// <param name="left">The left color.</param>
    /// <param name="right">The right color.</param>
    /// <returns>Whether or not the left is less than the right.</returns>
    public static bool operator <(Color left, Color right)
    {
        if ((double)left.R != (double)right.R)
            return (double)left.R < (double)right.R;
        if ((double)left.G != (double)right.G)
            return (double)left.G < (double)right.G;
        return (double)left.B == (double)right.B ? (double)left.A < (double)right.A : (double)left.B < (double)right.B;
    }

    /// <summary>
    /// Compares two <see cref="T:Godot.Color" />s by first checking if
    /// the red value of the <paramref name="left" /> color is greater than
    /// the red value of the <paramref name="right" /> color.
    /// If the red values are exactly equal, then it repeats this check
    /// with the green values of the two colors, then with the blue values,
    /// and then with the alpha value.
    /// This operator is useful for sorting colors.
    /// </summary>
    /// <param name="left">The left color.</param>
    /// <param name="right">The right color.</param>
    /// <returns>Whether or not the left is greater than the right.</returns>
    public static bool operator >(Color left, Color right)
    {
        if ((double)left.R != (double)right.R)
            return (double)left.R > (double)right.R;
        if ((double)left.G != (double)right.G)
            return (double)left.G > (double)right.G;
        return (double)left.B == (double)right.B ? (double)left.A > (double)right.A : (double)left.B > (double)right.B;
    }

    /// <summary>
    /// Compares two <see cref="T:Godot.Color" />s by first checking if
    /// the red value of the <paramref name="left" /> color is less than
    /// or equal to the red value of the <paramref name="right" /> color.
    /// If the red values are exactly equal, then it repeats this check
    /// with the green values of the two colors, then with the blue values,
    /// and then with the alpha value.
    /// This operator is useful for sorting colors.
    /// </summary>
    /// <param name="left">The left color.</param>
    /// <param name="right">The right color.</param>
    /// <returns>Whether or not the left is less than or equal to the right.</returns>
    public static bool operator <=(Color left, Color right)
    {
        if ((double)left.R != (double)right.R)
            return (double)left.R < (double)right.R;
        if ((double)left.G != (double)right.G)
            return (double)left.G < (double)right.G;
        return (double)left.B == (double)right.B ? (double)left.A <= (double)right.A : (double)left.B < (double)right.B;
    }

    /// <summary>
    /// Compares two <see cref="T:Godot.Color" />s by first checking if
    /// the red value of the <paramref name="left" /> color is greater than
    /// or equal to the red value of the <paramref name="right" /> color.
    /// If the red values are exactly equal, then it repeats this check
    /// with the green values of the two colors, then with the blue values,
    /// and then with the alpha value.
    /// This operator is useful for sorting colors.
    /// </summary>
    /// <param name="left">The left color.</param>
    /// <param name="right">The right color.</param>
    /// <returns>Whether or not the left is greater than or equal to the right.</returns>
    public static bool operator >=(Color left, Color right)
    {
        if ((double)left.R != (double)right.R)
            return (double)left.R > (double)right.R;
        if ((double)left.G != (double)right.G)
            return (double)left.G > (double)right.G;
        return (double)left.B == (double)right.B ? (double)left.A >= (double)right.A : (double)left.B > (double)right.B;
    }

    public static implicit operator _Color(Color self) => new(self.R8, self.G8, self.B8, self.A8);
    
    /// <summary>
    /// Returns <see langword="true" /> if this color and <paramref name="obj" /> are equal.
    /// </summary>
    /// <param name="obj">The other object to compare.</param>
    /// <returns>Whether or not the color and the other object are equal.</returns>
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Color other && this.Equals(other);
    }

    /// <summary>
    /// Returns <see langword="true" /> if the colors are exactly equal.
    /// Note: Due to floating-point precision errors, consider using
    /// <see cref="M:Godot.Color.IsEqualApprox(Godot.Color)" /> instead, which is more reliable.
    /// </summary>
    /// <param name="other">The other color.</param>
    /// <returns>Whether or not the colors are equal.</returns>
    public readonly bool Equals(Color other)
    {
        return (double)this.R == (double)other.R && (double)this.G == (double)other.G && (double)this.B == (double)other.B && (double)this.A == (double)other.A;
    }

    /// <summary>
    /// Returns <see langword="true" /> if this color and <paramref name="other" /> are approximately equal,
    /// by running <see cref="M:Godot.Mathf.IsEqualApprox(System.Single,System.Single)" /> on each component.
    /// </summary>
    /// <param name="other">The other color to compare.</param>
    /// <returns>Whether or not the colors are approximately equal.</returns>
    public readonly bool IsEqualApprox(Color other)
    {
        return Mathf.IsEqualApprox(this.R, other.R) && Mathf.IsEqualApprox(this.G, other.G) && Mathf.IsEqualApprox(this.B, other.B) && Mathf.IsEqualApprox(this.A, other.A);
    }

    /// <summary>
    /// Serves as the hash function for <see cref="T:Godot.Color" />.
    /// </summary>
    /// <returns>A hash code for this color.</returns>
    public override readonly int GetHashCode()
    {
        return HashCode.Combine<float, float, float, float>(this.R, this.G, this.B, this.A);
    }

    /// <summary>
    /// Converts this <see cref="T:Godot.Color" /> to a string.
    /// </summary>
    /// <returns>A string representation of this color.</returns>
    public override readonly string ToString() => this.ToString((string)null);

    /// <summary>
    /// Converts this <see cref="T:Godot.Color" /> to a string with the given <paramref name="format" />.
    /// </summary>
    /// <returns>A string representation of this color.</returns>
    public readonly string ToString(string? format)
    {
        return
            $"({this.R.ToString(format, (IFormatProvider)CultureInfo.InvariantCulture)}, {this.G.ToString(format, (IFormatProvider)CultureInfo.InvariantCulture)}, {this.B.ToString(format, (IFormatProvider)CultureInfo.InvariantCulture)}, {this.A.ToString(format, (IFormatProvider)CultureInfo.InvariantCulture)})";
    }
}