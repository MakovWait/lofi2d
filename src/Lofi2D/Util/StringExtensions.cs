using System.Globalization;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Lofi2D.Math;

namespace Lofi2D.Util;

public static class StringExtensions
{
    private static readonly char[] _invalidFileNameCharacters = new char[10]
    {
        ':',
        '/',
        '\\',
        '?',
        '*',
        '"',
        '|',
        '%',
        '<',
        '>'
    };

    private static readonly char[] _nonPrintable = new char[33]
    {
        char.MinValue,
        '\u0001',
        '\u0002',
        '\u0003',
        '\u0004',
        '\u0005',
        '\u0006',
        '\a',
        '\b',
        '\t',
        '\n',
        '\v',
        '\f',
        '\r',
        '\u000E',
        '\u000F',
        '\u0010',
        '\u0011',
        '\u0012',
        '\u0013',
        '\u0014',
        '\u0015',
        '\u0016',
        '\u0017',
        '\u0018',
        '\u0019',
        '\u001A',
        '\u001B',
        '\u001C',
        '\u001D',
        '\u001E',
        '\u001F',
        ' '
    };

    private const string UniqueNodePrefix = "%";

    private static readonly string[] _invalidNodeNameCharacters = new string[6]
    {
        ".",
        ":",
        "@",
        "/",
        "\"",
        "%"
    };

    private static int GetSliceCount(this string instance, string splitter)
    {
        if (string.IsNullOrEmpty(instance) || string.IsNullOrEmpty(splitter))
            return 0;
        int from = 0;
        int sliceCount = 1;
        int num;
        for (; (num = instance.Find(splitter, from)) >= 0; from = num + splitter.Length)
            ++sliceCount;
        return sliceCount;
    }

    private static string GetSliceCharacter(this string instance, char splitter, int slice)
    {
        if (string.IsNullOrEmpty(instance) || slice < 0)
            return string.Empty;
        int index = 0;
        int startIndex = 0;
        int num = 0;
        while (true)
        {
            bool flag = instance.Length <= index;
            if (flag || (int)instance[index] == (int)splitter)
            {
                if (slice != num)
                {
                    if (!flag)
                    {
                        ++num;
                        startIndex = index + 1;
                    }
                    else
                        goto label_6;
                }
                else
                    break;
            }
            ++index;
        }
        return instance.Substring(startIndex, index - startIndex);
        label_6:
        return string.Empty;
    }

    /// <summary>
    /// Returns the bigrams (pairs of consecutive letters) of this string.
    /// </summary>
    /// <param name="instance">The string that will be used.</param>
    /// <returns>The bigrams of this string.</returns>
    public static string[] Bigrams(this string instance)
    {
        string[] strArray = new string[instance.Length - 1];
        for (int startIndex = 0; startIndex < strArray.Length; ++startIndex)
            strArray[startIndex] = instance.Substring(startIndex, 2);
        return strArray;
    }

    /// <summary>
    /// Converts a string containing a binary number into an integer.
    /// Binary strings can either be prefixed with <c>0b</c> or not,
    /// and they can also start with a <c>-</c> before the optional prefix.
    /// </summary>
    /// <param name="instance">The string to convert.</param>
    /// <returns>The converted string.</returns>
    public static int BinToInt(this string instance)
    {
        if (instance.Length == 0)
            return 0;
        int num = 1;
        if (instance[0] == '-')
        {
            num = -1;
            instance = instance.Substring(1);
        }
        if (instance.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
            instance = instance.Substring(2);
        return num * Convert.ToInt32(instance, 2);
    }

    /// <summary>
    /// Returns the number of occurrences of substring <paramref name="what" /> in the string.
    /// </summary>
    /// <param name="instance">The string where the substring will be searched.</param>
    /// <param name="what">The substring that will be counted.</param>
    /// <param name="from">Index to start searching from.</param>
    /// <param name="to">Index to stop searching at.</param>
    /// <param name="caseSensitive">If the search is case sensitive.</param>
    /// <returns>Number of occurrences of the substring in the string.</returns>
    public static int Count(
        this string instance,
        string what,
        int from = 0,
        int to = 0,
        bool caseSensitive = true
    )
    {
        if (what.Length == 0)
            return 0;
        int length1 = instance.Length;
        int length2 = what.Length;
        if (length1 < length2 || from < 0 || to < 0)
            return 0;
        if (to == 0)
            to = length1;
        else if (from >= to)
            return 0;
        string str = from != 0 || to != length1 ? instance.Substring(from, to - from) : instance;
        int num1 = 0;
        int num2;
        do
        {
            num2 = str.IndexOf(what, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
            if (num2 != -1)
            {
                str = str.Substring(num2 + length2);
                ++num1;
            }
        } while (num2 != -1);
        return num1;
    }

    /// <summary>
    /// Returns the number of occurrences of substring <paramref name="what" /> (ignoring case)
    /// between <paramref name="from" /> and <paramref name="to" /> positions. If <paramref name="from" />
    /// and <paramref name="to" /> equals 0 the whole string will be used. If only <paramref name="to" />
    /// equals 0 the remained substring will be used.
    /// </summary>
    /// <param name="instance">The string where the substring will be searched.</param>
    /// <param name="what">The substring that will be counted.</param>
    /// <param name="from">Index to start searching from.</param>
    /// <param name="to">Index to stop searching at.</param>
    /// <returns>Number of occurrences of the substring in the string.</returns>
    public static int CountN(this string instance, string what, int from = 0, int to = 0)
    {
        return instance.Count(what, from, to, false);
    }

    /// <summary>
    /// Returns a copy of the string with indentation (leading tabs and spaces) removed.
    /// See also <see cref="M:Godot.StringExtensions.Indent(System.String,System.String)" /> to add indentation.
    /// </summary>
    /// <param name="instance">The string to remove the indentation from.</param>
    /// <returns>The string with the indentation removed.</returns>
    public static string Dedent(this string instance)
    {
        StringBuilder stringBuilder = new StringBuilder();
        string str = "";
        bool flag1 = false;
        bool flag2 = false;
        int startIndex = 0;
        int start = -1;
        for (int index1 = 0; index1 < instance.Length; ++index1)
        {
            char ch = instance[index1];
            if (ch == '\n')
            {
                if (flag2)
                    stringBuilder.Append(instance.AsSpan(start, index1 - start));
                stringBuilder.Append('\n');
                flag2 = false;
                startIndex = index1 + 1;
                start = -1;
            }
            else if (!flag2)
            {
                if (ch > ' ')
                {
                    flag2 = true;
                    if (!flag1)
                    {
                        flag1 = true;
                        str = instance.Substring(startIndex, index1 - startIndex);
                        start = index1;
                    }
                }
                if (flag1 && start < 0)
                {
                    int index2 = index1 - startIndex;
                    if (index2 >= str.Length || (int)ch != (int)str[index2])
                        start = index1;
                }
            }
        }
        if (flag2)
            stringBuilder.Append(instance.AsSpan(start, instance.Length - start));
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Returns a copy of the string with special characters escaped using the C language standard.
    /// </summary>
    /// <param name="instance">The string to escape.</param>
    /// <returns>The escaped string.</returns>
    public static string CEscape(this string instance)
    {
        StringBuilder stringBuilder = new StringBuilder(instance);
        stringBuilder.Replace("\\", "\\\\");
        stringBuilder.Replace("\a", "\\a");
        stringBuilder.Replace("\b", "\\b");
        stringBuilder.Replace("\f", "\\f");
        stringBuilder.Replace("\n", "\\n");
        stringBuilder.Replace("\r", "\\r");
        stringBuilder.Replace("\t", "\\t");
        stringBuilder.Replace("\v", "\\v");
        stringBuilder.Replace("'", "\\'");
        stringBuilder.Replace("\"", "\\\"");
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Returns a copy of the string with escaped characters replaced by their meanings
    /// according to the C language standard.
    /// </summary>
    /// <param name="instance">The string to unescape.</param>
    /// <returns>The unescaped string.</returns>
    public static string CUnescape(this string instance)
    {
        StringBuilder stringBuilder = new StringBuilder(instance);
        stringBuilder.Replace("\\a", "\a");
        stringBuilder.Replace("\\b", "\b");
        stringBuilder.Replace("\\f", "\f");
        stringBuilder.Replace("\\n", "\n");
        stringBuilder.Replace("\\r", "\r");
        stringBuilder.Replace("\\t", "\t");
        stringBuilder.Replace("\\v", "\v");
        stringBuilder.Replace("\\'", "'");
        stringBuilder.Replace("\\\"", "\"");
        stringBuilder.Replace("\\\\", "\\");
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Changes the case of some letters. Replace underscores with spaces, convert all letters
    /// to lowercase then capitalize first and every letter following the space character.
    /// For <c>capitalize camelCase mixed_with_underscores</c> it will return
    /// <c>Capitalize Camelcase Mixed With Underscores</c>.
    /// </summary>
    /// <param name="instance">The string to capitalize.</param>
    /// <returns>The capitalized string.</returns>
    public static string Capitalize(this string instance)
    {
        string instance1 = instance.CamelcaseToUnderscore(true).Replace("_", " ", StringComparison.Ordinal).Trim();
        string empty = string.Empty;
        for (int slice = 0; slice < instance1.GetSliceCount(" "); ++slice)
        {
            string sliceCharacter = instance1.GetSliceCharacter(' ', slice);
            if (sliceCharacter.Length > 0)
            {
                string str = char.ToUpperInvariant(sliceCharacter[0]).ToString() + sliceCharacter.Substring(1);
                if (slice > 0)
                    empty += " ";
                empty += str;
            }
        }
        return empty;
    }

    /// <summary>
    /// Returns the string converted to <c>camelCase</c>.
    /// </summary>
    /// <param name="instance">The string to convert.</param>
    /// <returns>The converted string.</returns>
    public static string ToCamelCase(this string instance)
    {
        var s = instance.ToPascalCase();
        if (!string.IsNullOrEmpty(s))
        {
            s = char.ToLowerInvariant(s[0]) + s.Substring(1);
        }
        return s;
    }

    /// <summary>
    /// Returns the string converted to <c>PascalCase</c>.
    /// </summary>
    /// <param name="instance">The string to convert.</param>
    /// <returns>The converted string.</returns>
    public static string ToPascalCase(this string instance)
    {
        return instance.Capitalize().Replace(" ", "");
    }

    /// <summary>
    /// Returns the string converted to <c>snake_case</c>.
    /// </summary>
    /// <param name="instance">The string to convert.</param>
    /// <returns>The converted string.</returns>
    public static string ToSnakeCase(this string str)
    {
        return str.CamelcaseToUnderscore(true)
            .Replace(" ", "_")
            .Trim();
    }
    
    private static string CamelcaseToUnderscore(this string instance, bool lowerCase)
    {
        string empty = string.Empty;
        int num1 = 0;
        for (int index = 1; index < instance.Length; ++index)
        {
            bool flag1 = char.IsUpper(instance[index]);
            int num2 = char.IsDigit(instance[index]) ? 1 : 0;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = char.IsUpper(instance[index - 1]);
            bool flag6 = char.IsDigit(instance[index - 1]);
            if (index + 2 < instance.Length)
                flag2 = char.IsLower(instance[index + 1]) && char.IsLower(instance[index + 2]);
            if (index + 1 < instance.Length)
            {
                flag3 = char.IsLower(instance[index + 1]);
                flag4 = char.IsDigit(instance[index + 1]);
            }
            if ((flag1 && !flag5 && !flag6) | flag5 & flag1 & flag2 | (num2 != 0 && !flag6) | ((num2 == 0 ? 0 : (!flag6 ? 1 : 0)) & (flag3 ? 1 : 0)) != 0 | (num2 == 0 & flag6 && flag3 | flag4))
            {
                empty += instance.AsSpan(num1, index - num1).ToString() + "_";
                num1 = index;
            }
        }
        string str = empty + instance.Substring(num1, instance.Length - num1);
        return !lowerCase ? str : str.ToLowerInvariant();
    }

    /// <summary>
    /// Performs a case-sensitive comparison to another string and returns an integer that indicates their relative position in the sort order.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.NocasecmpTo(System.String,System.String)" />
    /// <seealso cref="M:Godot.StringExtensions.CompareTo(System.String,System.String,System.Boolean)" />
    /// <param name="instance">The string to compare.</param>
    /// <param name="to">The other string to compare.</param>
    /// <returns>An integer that indicates the lexical relationship between the two comparands.</returns>
    public static int CasecmpTo(this string instance, string to)
    {
        return string.Compare(instance, to, false, (CultureInfo)null);
    }

    /// <summary>
    /// Performs a comparison to another string and returns an integer that indicates their relative position in the sort order.
    /// </summary>
    /// <param name="instance">The string to compare.</param>
    /// <param name="to">The other string to compare.</param>
    /// <param name="caseSensitive">
    /// If <see langword="true" />, the comparison will be case sensitive.
    /// </param>
    /// <returns>An integer that indicates the lexical relationship between the two comparands.</returns>
    [Obsolete("Use string.Compare instead.")]
    public static int CompareTo(this string instance, string to, bool caseSensitive = true)
    {
        return string.Compare(instance, to, !caseSensitive, (CultureInfo)null);
    }

    /// <summary>
    /// Returns the extension without the leading period character (<c>.</c>)
    /// if the string is a valid file name or path. If the string does not contain
    /// an extension, returns an empty string instead.
    /// </summary>
    /// <example>
    /// <code>
    /// GD.Print("/path/to/file.txt".GetExtension())  // "txt"
    /// GD.Print("file.txt".GetExtension())  // "txt"
    /// GD.Print("file.sample.txt".GetExtension())  // "txt"
    /// GD.Print(".txt".GetExtension())  // "txt"
    /// GD.Print("file.txt.".GetExtension())  // "" (empty string)
    /// GD.Print("file.txt..".GetExtension())  // "" (empty string)
    /// GD.Print("txt".GetExtension())  // "" (empty string)
    /// GD.Print("".GetExtension())  // "" (empty string)
    /// </code>
    /// </example>
    /// <seealso cref="M:Godot.StringExtensions.GetBaseName(System.String)" />
    /// <seealso cref="M:Godot.StringExtensions.GetBaseDir(System.String)" />
    /// <seealso cref="M:Godot.StringExtensions.GetFile(System.String)" />
    /// <param name="instance">The path to a file.</param>
    /// <returns>The extension of the file or an empty string.</returns>
    public static string GetExtension(this string instance)
    {
        int num = instance.RFind(".");
        return num < 0 ? instance : instance.Substring(num + 1);
    }

    /// <summary>
    /// Returns the index of the first occurrence of the specified string in this instance,
    /// or <c>-1</c>. Optionally, the starting search index can be specified, continuing
    /// to the end of the string.
    /// Note: If you just want to know whether a string contains a substring, use the
    /// <see cref="M:System.String.Contains(System.String)" /> method.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.Find(System.String,System.Char,System.Int32,System.Boolean)" />
    /// <seealso cref="M:Godot.StringExtensions.FindN(System.String,System.String,System.Int32)" />
    /// <seealso cref="M:Godot.StringExtensions.RFind(System.String,System.String,System.Int32,System.Boolean)" />
    /// <seealso cref="M:Godot.StringExtensions.RFindN(System.String,System.String,System.Int32)" />
    /// <param name="instance">The string that will be searched.</param>
    /// <param name="what">The substring to find.</param>
    /// <param name="from">The search starting position.</param>
    /// <param name="caseSensitive">If <see langword="true" />, the search is case sensitive.</param>
    /// <returns>The starting position of the substring, or -1 if not found.</returns>
    public static int Find(this string instance, string what, int from = 0, bool caseSensitive = true)
    {
        return instance.IndexOf(what, from, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Find the first occurrence of a char. Optionally, the search starting position can be passed.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.Find(System.String,System.String,System.Int32,System.Boolean)" />
    /// <seealso cref="M:Godot.StringExtensions.FindN(System.String,System.String,System.Int32)" />
    /// <seealso cref="M:Godot.StringExtensions.RFind(System.String,System.String,System.Int32,System.Boolean)" />
    /// <seealso cref="M:Godot.StringExtensions.RFindN(System.String,System.String,System.Int32)" />
    /// <param name="instance">The string that will be searched.</param>
    /// <param name="what">The substring to find.</param>
    /// <param name="from">The search starting position.</param>
    /// <param name="caseSensitive">If <see langword="true" />, the search is case sensitive.</param>
    /// <returns>The first instance of the char, or -1 if not found.</returns>
    public static int Find(this string instance, char what, int from = 0, bool caseSensitive = true)
    {
        return caseSensitive ? instance.IndexOf(what, from) : CultureInfo.InvariantCulture.CompareInfo.IndexOf(instance, what, from, CompareOptions.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns the index of the first case-insensitive occurrence of the specified string in this instance,
    /// or <c>-1</c>. Optionally, the starting search index can be specified, continuing
    /// to the end of the string.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.Find(System.String,System.String,System.Int32,System.Boolean)" />
    /// <seealso cref="M:Godot.StringExtensions.Find(System.String,System.Char,System.Int32,System.Boolean)" />
    /// <seealso cref="M:Godot.StringExtensions.RFind(System.String,System.String,System.Int32,System.Boolean)" />
    /// <seealso cref="M:Godot.StringExtensions.RFindN(System.String,System.String,System.Int32)" />
    /// <param name="instance">The string that will be searched.</param>
    /// <param name="what">The substring to find.</param>
    /// <param name="from">The search starting position.</param>
    /// <returns>The starting position of the substring, or -1 if not found.</returns>
    public static int FindN(this string instance, string what, int from = 0)
    {
        return instance.IndexOf(what, from, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// If the string is a path to a file, return the base directory.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.GetBaseName(System.String)" />
    /// <seealso cref="M:Godot.StringExtensions.GetExtension(System.String)" />
    /// <seealso cref="M:Godot.StringExtensions.GetFile(System.String)" />
    /// <param name="instance">The path to a file.</param>
    /// <returns>The base directory.</returns>
    public static string GetBaseDir(this string instance)
    {
        int num1 = instance.Find("://");
        string str = string.Empty;
        string instance1;
        if (num1 != -1)
        {
            int num2 = num1 + 3;
            instance1 = instance.Substring(num2);
            str = instance.Substring(0, num2);
        }
        else if (instance.StartsWith('/'))
        {
            instance1 = instance.Substring(1);
            str = "/";
        }
        else
            instance1 = instance;
        int len = Mathf.Max(instance1.RFind("/"), instance1.RFind("\\"));
        return len == -1 ? str : str + instance1.Substr(0, len);
    }

    /// <summary>
    /// If the string is a path to a file, return the path to the file without the extension.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.GetExtension(System.String)" />
    /// <seealso cref="M:Godot.StringExtensions.GetBaseDir(System.String)" />
    /// <seealso cref="M:Godot.StringExtensions.GetFile(System.String)" />
    /// <param name="instance">The path to a file.</param>
    /// <returns>The path to the file without the extension.</returns>
    public static string GetBaseName(this string instance)
    {
        int length = instance.RFind(".");
        return length > 0 ? instance.Substring(0, length) : instance;
    }

    /// <summary>
    /// If the string is a path to a file, return the file and ignore the base directory.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.GetBaseName(System.String)" />
    /// <seealso cref="M:Godot.StringExtensions.GetExtension(System.String)" />
    /// <seealso cref="M:Godot.StringExtensions.GetBaseDir(System.String)" />
    /// <param name="instance">The path to a file.</param>
    /// <returns>The file name.</returns>
    public static string GetFile(this string instance)
    {
        int num = Mathf.Max(instance.RFind("/"), instance.RFind("\\"));
        return num == -1 ? instance : instance.Substring(num + 1);
    }

    /// <summary>
    /// Converts ASCII encoded array to string.
    /// Fast alternative to <see cref="M:Godot.StringExtensions.GetStringFromUtf8(System.Byte[])" /> if the
    /// content is ASCII-only. Unlike the UTF-8 function this function
    /// maps every byte to a character in the array. Multibyte sequences
    /// will not be interpreted correctly. For parsing user input always
    /// use <see cref="M:Godot.StringExtensions.GetStringFromUtf8(System.Byte[])" />.
    /// </summary>
    /// <param name="bytes">A byte array of ASCII characters (on the range of 0-127).</param>
    /// <returns>A string created from the bytes.</returns>
    public static string GetStringFromAscii(this byte[] bytes) => Encoding.ASCII.GetString(bytes);

    /// <summary>
    /// Converts UTF-16 encoded array to string using the little endian byte order.
    /// </summary>
    /// <param name="bytes">A byte array of UTF-16 characters.</param>
    /// <returns>A string created from the bytes.</returns>
    public static string GetStringFromUtf16(this byte[] bytes) => Encoding.Unicode.GetString(bytes);

    /// <summary>
    /// Converts UTF-32 encoded array to string using the little endian byte order.
    /// </summary>
    /// <param name="bytes">A byte array of UTF-32 characters.</param>
    /// <returns>A string created from the bytes.</returns>
    public static string GetStringFromUtf32(this byte[] bytes) => Encoding.UTF32.GetString(bytes);

    /// <summary>
    /// Converts UTF-8 encoded array to string.
    /// Slower than <see cref="M:Godot.StringExtensions.GetStringFromAscii(System.Byte[])" /> but supports UTF-8
    /// encoded data. Use this function if you are unsure about the
    /// source of the data. For user input this function
    /// should always be preferred.
    /// </summary>
    /// <param name="bytes">
    /// A byte array of UTF-8 characters (a character may take up multiple bytes).
    /// </param>
    /// <returns>A string created from the bytes.</returns>
    public static string GetStringFromUtf8(this byte[] bytes) => Encoding.UTF8.GetString(bytes);

    /// <summary>Hash the string and return a 32 bits unsigned integer.</summary>
    /// <param name="instance">The string to hash.</param>
    /// <returns>The calculated hash of the string.</returns>
    public static uint Hash(this string instance)
    {
        uint num1 = 5381;
        foreach (uint num2 in instance)
            num1 = (num1 << 5) + num1 + num2;
        return num1;
    }

    /// <summary>Decodes a hexadecimal string.</summary>
    /// <param name="instance">The hexadecimal string.</param>
    /// <returns>The byte array representation of this string.</returns>
    public static byte[] HexDecode(this string instance)
    {
        if (instance.Length % 2 != 0)
            throw new ArgumentException("Hexadecimal string of uneven length.", nameof(instance));
        int length = instance.Length / 2;
        byte[] numArray = new byte[length];
        for (int index = 0; index < length; ++index)
            numArray[index] = (byte)int.Parse(instance.AsSpan(index * 2, 2), NumberStyles.AllowHexSpecifier, (IFormatProvider)CultureInfo.InvariantCulture);
        return numArray;
    }

    /// <summary>
    /// Returns a hexadecimal representation of this byte as a string.
    /// </summary>
    /// <param name="b">The byte to encode.</param>
    /// <returns>The hexadecimal representation of this byte.</returns>
    internal static string HexEncode(this byte b)
    {
        string str = string.Empty;
        for (int index = 0; index < 2; ++index)
        {
            int num = (int)b & 15;
            char ch = num >= 10 ? (char)(97 + num - 10) : (char)(48 /*0x30*/ + num);
            b >>= 4;
            str = ch.ToString() + str;
        }
        return str;
    }

    /// <summary>
    /// Returns a hexadecimal representation of this byte array as a string.
    /// </summary>
    /// <param name="bytes">The byte array to encode.</param>
    /// <returns>The hexadecimal representation of this byte array.</returns>
    public static string HexEncode(this byte[] bytes)
    {
        string empty = string.Empty;
        foreach (byte b in bytes)
            empty += b.HexEncode();
        return empty;
    }

    /// <summary>
    /// Converts a string containing a hexadecimal number into an integer.
    /// Hexadecimal strings can either be prefixed with <c>0x</c> or not,
    /// and they can also start with a <c>-</c> before the optional prefix.
    /// </summary>
    /// <param name="instance">The string to convert.</param>
    /// <returns>The converted string.</returns>
    public static int HexToInt(this string instance)
    {
        if (instance.Length == 0)
            return 0;
        int num = 1;
        if (instance[0] == '-')
        {
            num = -1;
            instance = instance.Substring(1);
        }
        if (instance.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            instance = instance.Substring(2);
        return num * int.Parse(instance, NumberStyles.HexNumber, (IFormatProvider)CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Returns a copy of the string with lines indented with <paramref name="prefix" />.
    /// For example, the string can be indented with two tabs using <c>"\t\t"</c>,
    /// or four spaces using <c>"    "</c>. The prefix can be any string so it can
    /// also be used to comment out strings with e.g. <c>"// </c>.
    /// See also <see cref="M:Godot.StringExtensions.Dedent(System.String)" /> to remove indentation.
    /// Note: Empty lines are kept empty.
    /// </summary>
    /// <param name="instance">The string to add indentation to.</param>
    /// <param name="prefix">The string to use as indentation.</param>
    /// <returns>The string with indentation added.</returns>
    public static string Indent(this string instance, string prefix)
    {
        StringBuilder stringBuilder = new StringBuilder();
        int start = 0;
        for (int index = 0; index < instance.Length; ++index)
        {
            char ch = instance[index];
            if (ch == '\n')
            {
                if (index == start)
                {
                    stringBuilder.Append(ch);
                }
                else
                {
                    stringBuilder.Append(prefix);
                    stringBuilder.Append(instance.AsSpan(start, index - start + 1));
                }
                start = index + 1;
            }
        }
        if (start != instance.Length)
        {
            stringBuilder.Append(prefix);
            stringBuilder.Append(instance.AsSpan(start));
        }
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Returns <see langword="true" /> if the string is a path to a file or
    /// directory and its starting point is explicitly defined. This includes
    /// <c>res://</c>, <c>user://</c>, <c>C:\</c>, <c>/</c>, etc.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.IsRelativePath(System.String)" />
    /// <param name="instance">The string to check.</param>
    /// <returns>If the string is an absolute path.</returns>
    public static bool IsAbsolutePath(this string instance)
    {
        if (string.IsNullOrEmpty(instance))
            return false;
        return instance.Length > 1
            ? instance[0] == '/' || instance[0] == '\\' || instance.Contains(":/", StringComparison.Ordinal) || instance.Contains(":\\", StringComparison.Ordinal)
            : instance[0] == '/' || instance[0] == '\\';
    }

    /// <summary>
    /// Returns <see langword="true" /> if the string is a path to a file or
    /// directory and its starting point is implicitly defined within the
    /// context it is being used. The starting point may refer to the current
    /// directory (<c>./</c>), or the current <see cref="T:Godot.Node" />.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.IsAbsolutePath(System.String)" />
    /// <param name="instance">The string to check.</param>
    /// <returns>If the string is a relative path.</returns>
    public static bool IsRelativePath(this string instance) => !instance.IsAbsolutePath();

    /// <summary>
    /// Check whether this string is a subsequence of the given string.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.IsSubsequenceOfN(System.String,System.String)" />
    /// <param name="instance">The subsequence to search.</param>
    /// <param name="text">The string that contains the subsequence.</param>
    /// <param name="caseSensitive">If <see langword="true" />, the check is case sensitive.</param>
    /// <returns>If the string is a subsequence of the given string.</returns>
    public static bool IsSubsequenceOf(this string instance, string text, bool caseSensitive = true)
    {
        int length = instance.Length;
        if (length == 0)
            return true;
        if (length > text.Length)
            return false;
        int index1 = 0;
        for (int index2 = 0; index1 < length && index2 < text.Length; ++index2)
        {
            if (caseSensitive ? (int)instance[index1] == (int)text[index2] : (int)char.ToLowerInvariant(instance[index1]) == (int)char.ToLowerInvariant(text[index2]))
            {
                ++index1;
                if (index1 >= length)
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Check whether this string is a subsequence of the given string, ignoring case differences.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.IsSubsequenceOf(System.String,System.String,System.Boolean)" />
    /// <param name="instance">The subsequence to search.</param>
    /// <param name="text">The string that contains the subsequence.</param>
    /// <returns>If the string is a subsequence of the given string.</returns>
    public static bool IsSubsequenceOfN(this string instance, string text)
    {
        return instance.IsSubsequenceOf(text, false);
    }

    /// <summary>
    /// Returns <see langword="true" /> if this string is free from characters that
    /// aren't allowed in file names.
    /// </summary>
    /// <param name="instance">The string to check.</param>
    /// <returns>If the string contains a valid file name.</returns>
    public static bool IsValidFileName(this string instance)
    {
        string str = instance.Trim();
        return !(instance != str) && !string.IsNullOrEmpty(str) && instance.IndexOfAny(StringExtensions._invalidFileNameCharacters) == -1;
    }

    /// <summary>
    /// Returns <see langword="true" /> if this string contains a valid <see langword="float" />.
    /// This is inclusive of integers, and also supports exponents.
    /// </summary>
    /// <example>
    /// <code>
    /// GD.Print("1.7".IsValidFloat())  // Prints "True"
    /// GD.Print("24".IsValidFloat())  // Prints "True"
    /// GD.Print("7e3".IsValidFloat())  // Prints "True"
    /// GD.Print("Hello".IsValidFloat())  // Prints "False"
    /// </code>
    /// </example>
    /// <param name="instance">The string to check.</param>
    /// <returns>If the string contains a valid floating point number.</returns>
    public static bool IsValidFloat(this string instance) => float.TryParse(instance, out float _);

    /// <summary>
    /// Returns <see langword="true" /> if this string contains a valid hexadecimal number.
    /// If <paramref name="withPrefix" /> is <see langword="true" />, then a validity of the
    /// hexadecimal number is determined by <c>0x</c> prefix, for instance: <c>0xDEADC0DE</c>.
    /// </summary>
    /// <param name="instance">The string to check.</param>
    /// <param name="withPrefix">If the string must contain the <c>0x</c> prefix to be valid.</param>
    /// <returns>If the string contains a valid hexadecimal number.</returns>
    public static bool IsValidHexNumber(this string instance, bool withPrefix = false)
    {
        if (string.IsNullOrEmpty(instance))
            return false;
        int index1 = 0;
        if (instance.Length != 1 && instance[0] == '+' || instance[0] == '-')
            ++index1;
        if (withPrefix)
        {
            if (instance.Length < 3 || instance[index1] != '0' || instance[index1 + 1] != 'x')
                return false;
            index1 += 2;
        }
        for (int index2 = index1; index2 < instance.Length; ++index2)
        {
            if (!IsHexDigit(instance[index2]))
                return false;
        }
        return true;

        static bool IsHexDigit(char c)
        {
            if (char.IsDigit(c) || c >= 'a' && c <= 'f')
                return true;
            return c >= 'A' && c <= 'F';
        }
    }

    /// <summary>
    /// Returns <see langword="true" /> if this string contains a valid color in hexadecimal
    /// HTML notation. Other HTML notations such as named colors or <c>hsl()</c> aren't
    /// considered valid by this method and will return <see langword="false" />.
    /// </summary>
    /// <param name="instance">The string to check.</param>
    /// <returns>If the string contains a valid HTML color.</returns>
    public static bool IsValidHtmlColor(this string instance)
    {
        return Color.HtmlIsValid((ReadOnlySpan<char>)instance);
    }

    /// <summary>
    /// Returns <see langword="true" /> if this string is a valid identifier.
    /// A valid identifier may contain only letters, digits and underscores (<c>_</c>)
    /// and the first character may not be a digit.
    /// </summary>
    /// <example>
    /// <code>
    /// GD.Print("good_ident_1".IsValidIdentifier())  // Prints "True"
    /// GD.Print("1st_bad_ident".IsValidIdentifier())  // Prints "False"
    /// GD.Print("bad_ident_#2".IsValidIdentifier())  // Prints "False"
    /// </code>
    /// </example>
    /// <param name="instance">The string to check.</param>
    /// <returns>If the string contains a valid identifier.</returns>
    public static bool IsValidIdentifier(this string instance)
    {
        int length = instance.Length;
        if (length == 0 || instance[0] >= '0' && instance[0] <= '9')
            return false;
        for (int index = 0; index < length; ++index)
        {
            if ((instance[index] == '_' || instance[index] >= 'a' && instance[index] <= 'z' || instance[index] >= 'A' && instance[index] <= 'Z'
                    ? 1
                    : (instance[index] < '0' ? 0 : (instance[index] <= '9' ? 1 : 0))) == 0)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Returns <see langword="true" /> if this string contains a valid <see langword="int" />.
    /// </summary>
    /// <example>
    /// <code>
    /// GD.Print("7".IsValidInt())  // Prints "True"
    /// GD.Print("14.6".IsValidInt())  // Prints "False"
    /// GD.Print("L".IsValidInt())  // Prints "False"
    /// GD.Print("+3".IsValidInt())  // Prints "True"
    /// GD.Print("-12".IsValidInt())  // Prints "True"
    /// </code>
    /// </example>
    /// <param name="instance">The string to check.</param>
    /// <returns>If the string contains a valid integer.</returns>
    public static bool IsValidInt(this string instance) => int.TryParse(instance, out int _);

    /// <summary>
    /// Returns <see langword="true" /> if this string contains only a well-formatted
    /// IPv4 or IPv6 address. This method considers reserved IP addresses such as
    /// <c>0.0.0.0</c> as valid.
    /// </summary>
    /// <param name="instance">The string to check.</param>
    /// <returns>If the string contains a valid IP address.</returns>
    public static bool IsValidIPAddress(this string instance)
    {
        if (instance.Contains(':', StringComparison.Ordinal))
        {
            foreach (string instance1 in instance.Split(':'))
            {
                if (instance1.Length != 0)
                {
                    if (instance1.IsValidHexNumber())
                    {
                        long num = (long)instance1.HexToInt();
                        if (num < 0L || num > (long)ushort.MaxValue)
                            return false;
                    }
                    else if (!instance1.IsValidIPAddress())
                        return false;
                }
            }
        }
        else
        {
            string[] strArray = instance.Split('.');
            if (strArray.Length != 4)
                return false;
            for (int index = 0; index < strArray.Length; ++index)
            {
                string instance2 = strArray[index];
                if (!instance2.IsValidInt())
                    return false;
                int num = instance2.ToInt();
                if (num < 0 || num > (int)byte.MaxValue)
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Returns a copy of the string with special characters escaped using the JSON standard.
    /// </summary>
    /// <param name="instance">The string to escape.</param>
    /// <returns>The escaped string.</returns>
    public static string JSONEscape(this string instance)
    {
        StringBuilder stringBuilder = new StringBuilder(instance);
        stringBuilder.Replace("\\", "\\\\");
        stringBuilder.Replace("\b", "\\b");
        stringBuilder.Replace("\f", "\\f");
        stringBuilder.Replace("\n", "\\n");
        stringBuilder.Replace("\r", "\\r");
        stringBuilder.Replace("\t", "\\t");
        stringBuilder.Replace("\v", "\\v");
        stringBuilder.Replace("\"", "\\\"");
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Returns an amount of characters from the left of the string.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.Right(System.String,System.Int32)" />
    /// <param name="instance">The original string.</param>
    /// <param name="pos">The position in the string where the left side ends.</param>
    /// <returns>The left side of the string from the given position.</returns>
    public static string Left(this string instance, int pos)
    {
        if (pos <= 0)
            return string.Empty;
        return pos >= instance.Length ? instance : instance.Substring(0, pos);
    }

    /// <summary>
    /// Do a simple expression match, where '*' matches zero or more
    /// arbitrary characters and '?' matches any single character except '.'.
    /// </summary>
    /// <param name="str">The string to check.</param>
    /// <param name="pattern">Expression to check.</param>
    /// <param name="caseSensitive">
    /// If <see langword="true" />, the check will be case sensitive.
    /// </param>
    /// <returns>If the expression has any matches.</returns>
    private static bool WildcardMatch(
        ReadOnlySpan<char> str,
        ReadOnlySpan<char> pattern,
        bool caseSensitive
    )
    {
        if (pattern.IsEmpty)
            return str.IsEmpty;
        switch (pattern[0])
        {
            case '*':
                if (StringExtensions.WildcardMatch(str, pattern.Slice(1), caseSensitive))
                    return true;
                return !str.IsEmpty && StringExtensions.WildcardMatch(str.Slice(1), pattern, caseSensitive);
            case '?':
                return !str.IsEmpty && str[0] != '.' && StringExtensions.WildcardMatch(str.Slice(1), pattern.Slice(1), caseSensitive);
            default:
                return !str.IsEmpty && (caseSensitive ? ((int)str[0] == (int)pattern[0] ? 1 : 0) : ((int)char.ToUpperInvariant(str[0]) == (int)char.ToUpperInvariant(pattern[0]) ? 1 : 0)) != 0 &&
                       StringExtensions.WildcardMatch(str.Slice(1), pattern.Slice(1), caseSensitive);
        }
    }

    /// <summary>
    /// Do a simple case sensitive expression match, using ? and * wildcards.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.MatchN(System.String,System.String)" />
    /// <param name="instance">The string to check.</param>
    /// <param name="expr">Expression to check.</param>
    /// <param name="caseSensitive">
    /// If <see langword="true" />, the check will be case sensitive.
    /// </param>
    /// <returns>If the expression has any matches.</returns>
    public static bool Match(this string instance, string expr, bool caseSensitive = true)
    {
        return instance.Length != 0 && expr.Length != 0 && StringExtensions.WildcardMatch((ReadOnlySpan<char>)instance, (ReadOnlySpan<char>)expr, caseSensitive);
    }

    /// <summary>
    /// Do a simple case insensitive expression match, using ? and * wildcards.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.Match(System.String,System.String,System.Boolean)" />
    /// <param name="instance">The string to check.</param>
    /// <param name="expr">Expression to check.</param>
    /// <returns>If the expression has any matches.</returns>
    public static bool MatchN(this string instance, string expr)
    {
        return instance.Length != 0 && expr.Length != 0 && StringExtensions.WildcardMatch((ReadOnlySpan<char>)instance, (ReadOnlySpan<char>)expr, false);
    }

    /// <summary>
    /// Returns the MD5 hash of the string as an array of bytes.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.Md5Text(System.String)" />
    /// <param name="instance">The string to hash.</param>
    /// <returns>The MD5 hash of the string.</returns>
    public static byte[] Md5Buffer(this string instance)
    {
        return MD5.HashData(Encoding.UTF8.GetBytes(instance));
    }

    /// <summary>Returns the MD5 hash of the string as a string.</summary>
    /// <seealso cref="M:Godot.StringExtensions.Md5Buffer(System.String)" />
    /// <param name="instance">The string to hash.</param>
    /// <returns>The MD5 hash of the string.</returns>
    public static string Md5Text(this string instance) => instance.Md5Buffer().HexEncode();

    /// <summary>
    /// Performs a case-insensitive comparison to another string and returns an integer that indicates their relative position in the sort order.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.CasecmpTo(System.String,System.String)" />
    /// <seealso cref="M:Godot.StringExtensions.CompareTo(System.String,System.String,System.Boolean)" />
    /// <param name="instance">The string to compare.</param>
    /// <param name="to">The other string to compare.</param>
    /// <returns>An integer that indicates the lexical relationship between the two comparands.</returns>
    public static int NocasecmpTo(this string instance, string to)
    {
        return string.Compare(instance, to, true, (CultureInfo)null);
    }

    /// <summary>
    /// Format a number to have an exact number of <paramref name="digits" />
    /// after the decimal point.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.PadZeros(System.String,System.Int32)" />
    /// <param name="instance">The string to pad.</param>
    /// <param name="digits">Amount of digits after the decimal point.</param>
    /// <returns>The string padded with zeroes.</returns>
    public static string PadDecimals(this string instance, int digits)
    {
        int length = instance.Find(".");
        if (length == -1)
        {
            if (digits <= 0)
                return instance;
            instance += ".";
            length = instance.Length - 1;
        }
        else if (digits <= 0)
            return instance.Substring(0, length);
        if (instance.Length - (length + 1) > digits)
        {
            instance = instance.Substring(0, length + digits + 1);
        }
        else
        {
            while (instance.Length - (length + 1) < digits)
                instance += "0";
        }
        return instance;
    }

    /// <summary>
    /// Format a number to have an exact number of <paramref name="digits" />
    /// before the decimal point.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.PadDecimals(System.String,System.Int32)" />
    /// <param name="instance">The string to pad.</param>
    /// <param name="digits">Amount of digits before the decimal point.</param>
    /// <returns>The string padded with zeroes.</returns>
    public static string PadZeros(this string instance, int digits)
    {
        string instance1 = instance;
        int length = instance1.Find(".");
        if (length == -1)
            length = instance1.Length;
        if (length == 0)
            return instance1;
        int num = 0;
        while (num < length && (instance1[num] < '0' || instance1[num] > '9'))
            ++num;
        if (num >= length)
            return instance1;
        for (; length - num < digits; ++length)
            instance1 = instance1.Insert(num, "0");
        return instance1;
    }

    /// <summary>
    /// If the string is a path, this concatenates <paramref name="file" />
    /// at the end of the string as a subpath.
    /// E.g. <c>"this/is".PathJoin("path") == "this/is/path"</c>.
    /// </summary>
    /// <param name="instance">The path that will be concatenated.</param>
    /// <param name="file">File name to concatenate with the path.</param>
    /// <returns>The concatenated path with the given file name.</returns>
    public static string PathJoin(this string instance, string file)
    {
        return instance.Length > 0 && instance[instance.Length - 1] == '/' ? instance + file : $"{instance}/{file}";
    }

    /// <summary>
    /// Replace occurrences of a substring for different ones inside the string, but search case-insensitive.
    /// </summary>
    /// <seealso cref="M:System.String.Replace(System.String,System.String,System.StringComparison)" />
    /// <param name="instance">The string to modify.</param>
    /// <param name="what">The substring to be replaced in the string.</param>
    /// <param name="forwhat">The substring that replaces <paramref name="what" />.</param>
    /// <returns>The string with the substring occurrences replaced.</returns>
    public static string ReplaceN(this string instance, string what, string forwhat)
    {
        return Regex.Replace(instance, what, forwhat, RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// Returns the index of the last occurrence of the specified string in this instance,
    /// or <c>-1</c>. Optionally, the starting search index can be specified, continuing to
    /// the beginning of the string.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.Find(System.String,System.String,System.Int32,System.Boolean)" />
    /// <seealso cref="M:Godot.StringExtensions.Find(System.String,System.Char,System.Int32,System.Boolean)" />
    /// <seealso cref="M:Godot.StringExtensions.FindN(System.String,System.String,System.Int32)" />
    /// <seealso cref="M:Godot.StringExtensions.RFindN(System.String,System.String,System.Int32)" />
    /// <param name="instance">The string that will be searched.</param>
    /// <param name="what">The substring to search in the string.</param>
    /// <param name="from">The position at which to start searching.</param>
    /// <param name="caseSensitive">If <see langword="true" />, the search is case sensitive.</param>
    /// <returns>The position at which the substring was found, or -1 if not found.</returns>
    public static int RFind(this string instance, string what, int from = -1, bool caseSensitive = true)
    {
        if (from == -1)
            from = instance.Length - 1;
        return instance.LastIndexOf(what, from, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns the index of the last case-insensitive occurrence of the specified string in this instance,
    /// or <c>-1</c>. Optionally, the starting search index can be specified, continuing to
    /// the beginning of the string.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.Find(System.String,System.String,System.Int32,System.Boolean)" />
    /// <seealso cref="M:Godot.StringExtensions.Find(System.String,System.Char,System.Int32,System.Boolean)" />
    /// <seealso cref="M:Godot.StringExtensions.FindN(System.String,System.String,System.Int32)" />
    /// <seealso cref="M:Godot.StringExtensions.RFind(System.String,System.String,System.Int32,System.Boolean)" />
    /// <param name="instance">The string that will be searched.</param>
    /// <param name="what">The substring to search in the string.</param>
    /// <param name="from">The position at which to start searching.</param>
    /// <returns>The position at which the substring was found, or -1 if not found.</returns>
    public static int RFindN(this string instance, string what, int from = -1)
    {
        if (from == -1)
            from = instance.Length - 1;
        return instance.LastIndexOf(what, from, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns the right side of the string from a given position.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.Left(System.String,System.Int32)" />
    /// <param name="instance">The original string.</param>
    /// <param name="pos">The position in the string from which the right side starts.</param>
    /// <returns>The right side of the string from the given position.</returns>
    public static string Right(this string instance, int pos)
    {
        if (pos >= instance.Length)
            return instance;
        return pos < 0 ? string.Empty : instance.Substring(pos, instance.Length - pos);
    }

    /// <summary>
    /// Returns the SHA-1 hash of the string as an array of bytes.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.Sha1Text(System.String)" />
    /// <param name="instance">The string to hash.</param>
    /// <returns>The SHA-1 hash of the string.</returns>
    public static byte[] Sha1Buffer(this string instance)
    {
        return SHA1.HashData(Encoding.UTF8.GetBytes(instance));
    }

    /// <summary>Returns the SHA-1 hash of the string as a string.</summary>
    /// <seealso cref="M:Godot.StringExtensions.Sha1Buffer(System.String)" />
    /// <param name="instance">The string to hash.</param>
    /// <returns>The SHA-1 hash of the string.</returns>
    public static string Sha1Text(this string instance) => instance.Sha1Buffer().HexEncode();

    /// <summary>
    /// Returns the SHA-256 hash of the string as an array of bytes.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.Sha256Text(System.String)" />
    /// <param name="instance">The string to hash.</param>
    /// <returns>The SHA-256 hash of the string.</returns>
    public static byte[] Sha256Buffer(this string instance)
    {
        return SHA256.HashData(Encoding.UTF8.GetBytes(instance));
    }

    /// <summary>Returns the SHA-256 hash of the string as a string.</summary>
    /// <seealso cref="M:Godot.StringExtensions.Sha256Buffer(System.String)" />
    /// <param name="instance">The string to hash.</param>
    /// <returns>The SHA-256 hash of the string.</returns>
    public static string Sha256Text(this string instance) => instance.Sha256Buffer().HexEncode();

    /// <summary>
    /// Returns the similarity index of the text compared to this string.
    /// 1 means totally similar and 0 means totally dissimilar.
    /// </summary>
    /// <param name="instance">The string to compare.</param>
    /// <param name="text">The other string to compare.</param>
    /// <returns>The similarity index.</returns>
    public static float Similarity(this string instance, string text)
    {
        if (instance == text)
            return 1f;
        if (instance.Length < 2 || text.Length < 2)
            return 0.0f;
        string[] strArray1 = instance.Bigrams();
        string[] strArray2 = text.Bigrams();
        int length1 = strArray1.Length;
        int length2 = strArray2.Length;
        float num1 = (float)(length1 + length2);
        float num2 = 0.0f;
        for (int index1 = 0; index1 < length1; ++index1)
        {
            for (int index2 = 0; index2 < length2; ++index2)
            {
                if (strArray1[index1] == strArray2[index2])
                {
                    ++num2;
                    break;
                }
            }
        }
        return 2f * num2 / num1;
    }

    /// <summary>Returns a simplified canonical path.</summary>
    public static string SimplifyPath(this string path)
    {
        string s = path;
        string drive = "";

        // Check if we have a special path (like res://) or a protocol identifier.
        int p = s.IndexOf("://");
        bool found = false;
        if (p > 0)
        {
            bool onlyChars = true;
            for (int i = 0; i < p; i++)
            {
                if (!IsAsciiAlphanumericChar(s[i]))
                {
                    onlyChars = false;
                    break;
                }
            }
            if (onlyChars)
            {
                found = true;
                drive = s.Substring(0, p + 3);
                s = s.Substring(p + 3);
            }
        }

        if (!found)
        {
            if (IsNetworkSharePath(s))
            {
                // Network path, beginning with // or \\.
                drive = s.Substring(0, 2);
                s = s.Substring(2);
            }
            else if (s.StartsWith("/") || s.StartsWith("\\"))
            {
                // Absolute path.
                drive = s.Substring(0, 1);
                s = s.Substring(1);
            }
            else
            {
                // Windows-style drive path, like C:/ or C:\
                p = s.IndexOf(":/");
                if (p == -1)
                    p = s.IndexOf(":\\");
                
                if (p != -1 && (s.IndexOf('/') == -1 || p < s.IndexOf('/')))
                {
                    drive = s.Substring(0, p + 2);
                    s = s.Substring(p + 2);
                }
            }
        }

        s = s.Replace("\\", "/");

        // Collapse multiple slashes
        while (true)
        {
            string compare = s.Replace("//", "/");
            if (s == compare)
                break;
            s = compare;
        }

        var dirs = s.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();

        for (int i = 0; i < dirs.Count; i++)
        {
            if (dirs[i] == ".")
            {
                dirs.RemoveAt(i);
                i--;
            }
            else if (dirs[i] == "..")
            {
                if (i > 0)
                {
                    dirs.RemoveAt(i);
                    dirs.RemoveAt(i - 1);
                    i -= 2;
                }
            }
        }

        s = string.Join("/", dirs);

        return drive + s;
    }

    private static bool IsAsciiAlphanumericChar(char c)
    {
        return (c >= 'A' && c <= 'Z') ||
               (c >= 'a' && c <= 'z') ||
               (c >= '0' && c <= '9');
    }

    private static bool IsNetworkSharePath(string s)
    {
        return s.StartsWith("//") || s.StartsWith("\\\\");
    }

    /// <summary>
    /// Split the string by a divisor string, return an array of the substrings.
    /// Example "One,Two,Three" will return ["One","Two","Three"] if split by ",".
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.SplitFloats(System.String,System.String,System.Boolean)" />
    /// <param name="instance">The string to split.</param>
    /// <param name="divisor">The divisor string that splits the string.</param>
    /// <param name="allowEmpty">
    /// If <see langword="true" />, the array may include empty strings.
    /// </param>
    /// <returns>The array of strings split from the string.</returns>
    public static string[] Split(this string instance, string divisor, bool allowEmpty = true)
    {
        return instance.Split(divisor, allowEmpty ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// Split the string in floats by using a divisor string, return an array of the substrings.
    /// Example "1,2.5,3" will return [1,2.5,3] if split by ",".
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.Split(System.String,System.String,System.Boolean)" />
    /// <param name="instance">The string to split.</param>
    /// <param name="divisor">The divisor string that splits the string.</param>
    /// <param name="allowEmpty">
    /// If <see langword="true" />, the array may include empty floats.
    /// </param>
    /// <returns>The array of floats split from the string.</returns>
    public static float[] SplitFloats(this string instance, string divisor, bool allowEmpty = true)
    {
        List<float> floatList = new List<float>();
        int num1 = 0;
        int length = instance.Length;
        while (true)
        {
            int num2 = instance.Find(divisor, num1);
            if (num2 < 0)
                num2 = length;
            if (allowEmpty || num2 > num1)
                floatList.Add(float.Parse(instance.Substring(num1), (IFormatProvider)CultureInfo.InvariantCulture));
            if (num2 != length)
                num1 = num2 + divisor.Length;
            else
                break;
        }
        return floatList.ToArray();
    }

    /// <summary>
    /// Returns a copy of the string stripped of any non-printable character
    /// (including tabulations, spaces and line breaks) at the beginning and the end.
    /// The optional arguments are used to toggle stripping on the left and right
    /// edges respectively.
    /// </summary>
    /// <param name="instance">The string to strip.</param>
    /// <param name="left">If the left side should be stripped.</param>
    /// <param name="right">If the right side should be stripped.</param>
    /// <returns>The string stripped of any non-printable characters.</returns>
    public static string StripEdges(this string instance, bool left = true, bool right = true)
    {
        if (!left)
            return instance.TrimEnd(StringExtensions._nonPrintable);
        return right ? instance.Trim(StringExtensions._nonPrintable) : instance.TrimStart(StringExtensions._nonPrintable);
    }

    /// <summary>
    /// Returns a copy of the string stripped of any escape character.
    /// These include all non-printable control characters of the first page
    /// of the ASCII table (&lt; 32), such as tabulation (<c>\t</c>) and
    /// newline (<c>\n</c> and <c>\r</c>) characters, but not spaces.
    /// </summary>
    /// <param name="instance">The string to strip.</param>
    /// <returns>The string stripped of any escape characters.</returns>
    public static string StripEscapes(this string instance)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int index = 0; index < instance.Length; ++index)
        {
            if (instance[index] >= ' ')
                stringBuilder.Append(instance[index]);
        }
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Returns part of the string from the position <paramref name="from" />, with length <paramref name="len" />.
    /// </summary>
    /// <param name="instance">The string to slice.</param>
    /// <param name="from">The position in the string that the part starts from.</param>
    /// <param name="len">The length of the returned part.</param>
    /// <returns>
    /// Part of the string from the position <paramref name="from" />, with length <paramref name="len" />.
    /// </returns>
    public static string Substr(this string instance, int from, int len)
    {
        int num = instance.Length - from;
        return instance.Substring(from, len > num ? num : len);
    }

    /// <summary>
    /// Converts the String (which is a character array) to PackedByteArray (which is an array of bytes).
    /// The conversion is faster compared to <see cref="M:Godot.StringExtensions.ToUtf8Buffer(System.String)" />,
    /// as this method assumes that all the characters in the String are ASCII characters.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.ToUtf8Buffer(System.String)" />
    /// <seealso cref="M:Godot.StringExtensions.ToUtf16Buffer(System.String)" />
    /// <seealso cref="M:Godot.StringExtensions.ToUtf32Buffer(System.String)" />
    /// <param name="instance">The string to convert.</param>
    /// <returns>The string as ASCII encoded bytes.</returns>
    public static byte[] ToAsciiBuffer(this string instance) => Encoding.ASCII.GetBytes(instance);

    /// <summary>
    /// Converts a string, containing a decimal number, into a <see langword="float" />.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.ToInt(System.String)" />
    /// <param name="instance">The string to convert.</param>
    /// <returns>The number representation of the string.</returns>
    public static float ToFloat(this string instance)
    {
        return float.Parse(instance, (IFormatProvider)CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts a string, containing an integer number, into an <see langword="int" />.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.ToFloat(System.String)" />
    /// <param name="instance">The string to convert.</param>
    /// <returns>The number representation of the string.</returns>
    public static int ToInt(this string instance)
    {
        return int.Parse(instance, (IFormatProvider)CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the string (which is an array of characters) to a UTF-16 encoded array of bytes.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.ToAsciiBuffer(System.String)" />
    /// <seealso cref="M:Godot.StringExtensions.ToUtf32Buffer(System.String)" />
    /// <seealso cref="M:Godot.StringExtensions.ToUtf8Buffer(System.String)" />
    /// <param name="instance">The string to convert.</param>
    /// <returns>The string as UTF-16 encoded bytes.</returns>
    public static byte[] ToUtf16Buffer(this string instance) => Encoding.Unicode.GetBytes(instance);

    /// <summary>
    /// Converts the string (which is an array of characters) to a UTF-32 encoded array of bytes.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.ToAsciiBuffer(System.String)" />
    /// <seealso cref="M:Godot.StringExtensions.ToUtf16Buffer(System.String)" />
    /// <seealso cref="M:Godot.StringExtensions.ToUtf8Buffer(System.String)" />
    /// <param name="instance">The string to convert.</param>
    /// <returns>The string as UTF-32 encoded bytes.</returns>
    public static byte[] ToUtf32Buffer(this string instance) => Encoding.UTF32.GetBytes(instance);

    /// <summary>
    /// Converts the string (which is an array of characters) to a UTF-8 encoded array of bytes.
    /// The conversion is a bit slower than <see cref="M:Godot.StringExtensions.ToAsciiBuffer(System.String)" />,
    /// but supports all UTF-8 characters. Therefore, you should prefer this function
    /// over <see cref="M:Godot.StringExtensions.ToAsciiBuffer(System.String)" />.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.ToAsciiBuffer(System.String)" />
    /// <seealso cref="M:Godot.StringExtensions.ToUtf16Buffer(System.String)" />
    /// <seealso cref="M:Godot.StringExtensions.ToUtf32Buffer(System.String)" />
    /// <param name="instance">The string to convert.</param>
    /// <returns>The string as UTF-8 encoded bytes.</returns>
    public static byte[] ToUtf8Buffer(this string instance) => Encoding.UTF8.GetBytes(instance);

    /// <summary>
    /// Removes a given string from the start if it starts with it or leaves the string unchanged.
    /// </summary>
    /// <param name="instance">The string to remove the prefix from.</param>
    /// <param name="prefix">The string to remove from the start.</param>
    /// <returns>A copy of the string with the prefix string removed from the start.</returns>
    public static string TrimPrefix(this string instance, string prefix)
    {
        return instance.StartsWith(prefix, StringComparison.Ordinal) ? instance.Substring(prefix.Length) : instance;
    }

    /// <summary>
    /// Removes a given string from the end if it ends with it or leaves the string unchanged.
    /// </summary>
    /// <param name="instance">The string to remove the suffix from.</param>
    /// <param name="suffix">The string to remove from the end.</param>
    /// <returns>A copy of the string with the suffix string removed from the end.</returns>
    public static string TrimSuffix(this string instance, string suffix)
    {
        return instance.EndsWith(suffix, StringComparison.Ordinal) ? instance.Substring(0, instance.Length - suffix.Length) : instance;
    }

    /// <summary>
    /// Decodes a string in URL encoded format. This is meant to
    /// decode parameters in a URL when receiving an HTTP request.
    /// This mostly wraps around <see cref="M:System.Uri.UnescapeDataString(System.String)" />,
    /// but also handles <c>+</c>.
    /// See <see cref="M:Godot.StringExtensions.URIEncode(System.String)" /> for encoding.
    /// </summary>
    /// <param name="instance">The string to decode.</param>
    /// <returns>The unescaped string.</returns>
    public static string URIDecode(this string instance)
    {
        return Uri.UnescapeDataString(instance.Replace("+", "%20", StringComparison.Ordinal));
    }

    /// <summary>
    /// Encodes a string to URL friendly format. This is meant to
    /// encode parameters in a URL when sending an HTTP request.
    /// This wraps around <see cref="M:System.Uri.EscapeDataString(System.String)" />.
    /// See <see cref="M:Godot.StringExtensions.URIDecode(System.String)" /> for decoding.
    /// </summary>
    /// <param name="instance">The string to encode.</param>
    /// <returns>The escaped string.</returns>
    public static string URIEncode(this string instance) => Uri.EscapeDataString(instance);

    /// <summary>
    /// Removes any characters from the string that are prohibited in
    /// <see cref="T:Godot.Node" /> names (<c>.</c> <c>:</c> <c>@</c> <c>/</c> <c>"</c>).
    /// </summary>
    /// <param name="instance">The string to sanitize.</param>
    /// <returns>The string sanitized as a valid node name.</returns>
    public static string ValidateNodeName(this string instance)
    {
        string str = instance.Replace(StringExtensions._invalidNodeNameCharacters[0], "", StringComparison.Ordinal);
        for (int index = 1; index < StringExtensions._invalidNodeNameCharacters.Length; ++index)
            str = str.Replace(StringExtensions._invalidNodeNameCharacters[index], "", StringComparison.Ordinal);
        return str;
    }

    /// <summary>
    /// Returns a copy of the string with special characters escaped using the XML standard.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.XMLUnescape(System.String)" />
    /// <param name="instance">The string to escape.</param>
    /// <returns>The escaped string.</returns>
    public static string XMLEscape(this string instance) => SecurityElement.Escape(instance);

    /// <summary>
    /// Returns a copy of the string with escaped characters replaced by their meanings
    /// according to the XML standard.
    /// </summary>
    /// <seealso cref="M:Godot.StringExtensions.XMLEscape(System.String)" />
    /// <param name="instance">The string to unescape.</param>
    /// <returns>The unescaped string.</returns>
    public static string? XMLUnescape(this string instance)
    {
        return SecurityElement.FromString(instance)?.Text;
    }
}