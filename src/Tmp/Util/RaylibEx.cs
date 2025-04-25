using System.Globalization;
using Raylib_cs;

namespace Tmp.Util;

public static class RaylibEx
{
    public static Color ColorFromHex(string hex)
    {
        if (hex.StartsWith("#"))
            hex = hex[1..];

        if (hex.Length == 6)
        {
            byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            return new Color(r, g, b, (byte)255);
        }
        else if (hex.Length == 8)
        {
            byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            byte a = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
            return new Color(r, g, b, a);
        }
        else
        {
            throw new ArgumentException("Hex string must be 6 or 8 characters long (after #)");
        }
    }
}