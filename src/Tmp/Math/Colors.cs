namespace Tmp.Math;

/// <summary>
/// This class contains color constants created from standardized color names.
/// The standardized color set is based on the X11 and .NET color names.
/// </summary>
public static class Colors
{
    internal static readonly Dictionary<string, Color> NamedColors = new Dictionary<string, Color>()
    {
        {
            "ALICEBLUE",
            Colors.AliceBlue
        },
        {
            "ANTIQUEWHITE",
            Colors.AntiqueWhite
        },
        {
            "AQUA",
            Colors.Aqua
        },
        {
            "AQUAMARINE",
            Colors.Aquamarine
        },
        {
            "AZURE",
            Colors.Azure
        },
        {
            "BEIGE",
            Colors.Beige
        },
        {
            "BISQUE",
            Colors.Bisque
        },
        {
            "BLACK",
            Colors.Black
        },
        {
            "BLANCHEDALMOND",
            Colors.BlanchedAlmond
        },
        {
            "BLUE",
            Colors.Blue
        },
        {
            "BLUEVIOLET",
            Colors.BlueViolet
        },
        {
            "BROWN",
            Colors.Brown
        },
        {
            "BURLYWOOD",
            Colors.Burlywood
        },
        {
            "CADETBLUE",
            Colors.CadetBlue
        },
        {
            "CHARTREUSE",
            Colors.Chartreuse
        },
        {
            "CHOCOLATE",
            Colors.Chocolate
        },
        {
            "CORAL",
            Colors.Coral
        },
        {
            "CORNFLOWERBLUE",
            Colors.CornflowerBlue
        },
        {
            "CORNSILK",
            Colors.Cornsilk
        },
        {
            "CRIMSON",
            Colors.Crimson
        },
        {
            "CYAN",
            Colors.Cyan
        },
        {
            "DARKBLUE",
            Colors.DarkBlue
        },
        {
            "DARKCYAN",
            Colors.DarkCyan
        },
        {
            "DARKGOLDENROD",
            Colors.DarkGoldenrod
        },
        {
            "DARKGRAY",
            Colors.DarkGray
        },
        {
            "DARKGREEN",
            Colors.DarkGreen
        },
        {
            "DARKKHAKI",
            Colors.DarkKhaki
        },
        {
            "DARKMAGENTA",
            Colors.DarkMagenta
        },
        {
            "DARKOLIVEGREEN",
            Colors.DarkOliveGreen
        },
        {
            "DARKORANGE",
            Colors.DarkOrange
        },
        {
            "DARKORCHID",
            Colors.DarkOrchid
        },
        {
            "DARKRED",
            Colors.DarkRed
        },
        {
            "DARKSALMON",
            Colors.DarkSalmon
        },
        {
            "DARKSEAGREEN",
            Colors.DarkSeaGreen
        },
        {
            "DARKSLATEBLUE",
            Colors.DarkSlateBlue
        },
        {
            "DARKSLATEGRAY",
            Colors.DarkSlateGray
        },
        {
            "DARKTURQUOISE",
            Colors.DarkTurquoise
        },
        {
            "DARKVIOLET",
            Colors.DarkViolet
        },
        {
            "DEEPPINK",
            Colors.DeepPink
        },
        {
            "DEEPSKYBLUE",
            Colors.DeepSkyBlue
        },
        {
            "DIMGRAY",
            Colors.DimGray
        },
        {
            "DODGERBLUE",
            Colors.DodgerBlue
        },
        {
            "FIREBRICK",
            Colors.Firebrick
        },
        {
            "FLORALWHITE",
            Colors.FloralWhite
        },
        {
            "FORESTGREEN",
            Colors.ForestGreen
        },
        {
            "FUCHSIA",
            Colors.Fuchsia
        },
        {
            "GAINSBORO",
            Colors.Gainsboro
        },
        {
            "GHOSTWHITE",
            Colors.GhostWhite
        },
        {
            "GOLD",
            Colors.Gold
        },
        {
            "GOLDENROD",
            Colors.Goldenrod
        },
        {
            "GRAY",
            Colors.Gray
        },
        {
            "GREEN",
            Colors.Green
        },
        {
            "GREENYELLOW",
            Colors.GreenYellow
        },
        {
            "HONEYDEW",
            Colors.Honeydew
        },
        {
            "HOTPINK",
            Colors.HotPink
        },
        {
            "INDIANRED",
            Colors.IndianRed
        },
        {
            "INDIGO",
            Colors.Indigo
        },
        {
            "IVORY",
            Colors.Ivory
        },
        {
            "KHAKI",
            Colors.Khaki
        },
        {
            "LAVENDER",
            Colors.Lavender
        },
        {
            "LAVENDERBLUSH",
            Colors.LavenderBlush
        },
        {
            "LAWNGREEN",
            Colors.LawnGreen
        },
        {
            "LEMONCHIFFON",
            Colors.LemonChiffon
        },
        {
            "LIGHTBLUE",
            Colors.LightBlue
        },
        {
            "LIGHTCORAL",
            Colors.LightCoral
        },
        {
            "LIGHTCYAN",
            Colors.LightCyan
        },
        {
            "LIGHTGOLDENROD",
            Colors.LightGoldenrod
        },
        {
            "LIGHTGRAY",
            Colors.LightGray
        },
        {
            "LIGHTGREEN",
            Colors.LightGreen
        },
        {
            "LIGHTPINK",
            Colors.LightPink
        },
        {
            "LIGHTSALMON",
            Colors.LightSalmon
        },
        {
            "LIGHTSEAGREEN",
            Colors.LightSeaGreen
        },
        {
            "LIGHTSKYBLUE",
            Colors.LightSkyBlue
        },
        {
            "LIGHTSLATEGRAY",
            Colors.LightSlateGray
        },
        {
            "LIGHTSTEELBLUE",
            Colors.LightSteelBlue
        },
        {
            "LIGHTYELLOW",
            Colors.LightYellow
        },
        {
            "LIME",
            Colors.Lime
        },
        {
            "LIMEGREEN",
            Colors.LimeGreen
        },
        {
            "LINEN",
            Colors.Linen
        },
        {
            "MAGENTA",
            Colors.Magenta
        },
        {
            "MAROON",
            Colors.Maroon
        },
        {
            "MEDIUMAQUAMARINE",
            Colors.MediumAquamarine
        },
        {
            "MEDIUMBLUE",
            Colors.MediumBlue
        },
        {
            "MEDIUMORCHID",
            Colors.MediumOrchid
        },
        {
            "MEDIUMPURPLE",
            Colors.MediumPurple
        },
        {
            "MEDIUMSEAGREEN",
            Colors.MediumSeaGreen
        },
        {
            "MEDIUMSLATEBLUE",
            Colors.MediumSlateBlue
        },
        {
            "MEDIUMSPRINGGREEN",
            Colors.MediumSpringGreen
        },
        {
            "MEDIUMTURQUOISE",
            Colors.MediumTurquoise
        },
        {
            "MEDIUMVIOLETRED",
            Colors.MediumVioletRed
        },
        {
            "MIDNIGHTBLUE",
            Colors.MidnightBlue
        },
        {
            "MINTCREAM",
            Colors.MintCream
        },
        {
            "MISTYROSE",
            Colors.MistyRose
        },
        {
            "MOCCASIN",
            Colors.Moccasin
        },
        {
            "NAVAJOWHITE",
            Colors.NavajoWhite
        },
        {
            "NAVYBLUE",
            Colors.NavyBlue
        },
        {
            "OLDLACE",
            Colors.OldLace
        },
        {
            "OLIVE",
            Colors.Olive
        },
        {
            "OLIVEDRAB",
            Colors.OliveDrab
        },
        {
            "ORANGE",
            Colors.Orange
        },
        {
            "ORANGERED",
            Colors.OrangeRed
        },
        {
            "ORCHID",
            Colors.Orchid
        },
        {
            "PALEGOLDENROD",
            Colors.PaleGoldenrod
        },
        {
            "PALEGREEN",
            Colors.PaleGreen
        },
        {
            "PALETURQUOISE",
            Colors.PaleTurquoise
        },
        {
            "PALEVIOLETRED",
            Colors.PaleVioletRed
        },
        {
            "PAPAYAWHIP",
            Colors.PapayaWhip
        },
        {
            "PEACHPUFF",
            Colors.PeachPuff
        },
        {
            "PERU",
            Colors.Peru
        },
        {
            "PINK",
            Colors.Pink
        },
        {
            "PLUM",
            Colors.Plum
        },
        {
            "POWDERBLUE",
            Colors.PowderBlue
        },
        {
            "PURPLE",
            Colors.Purple
        },
        {
            "REBECCAPURPLE",
            Colors.RebeccaPurple
        },
        {
            "RED",
            Colors.Red
        },
        {
            "ROSYBROWN",
            Colors.RosyBrown
        },
        {
            "ROYALBLUE",
            Colors.RoyalBlue
        },
        {
            "SADDLEBROWN",
            Colors.SaddleBrown
        },
        {
            "SALMON",
            Colors.Salmon
        },
        {
            "SANDYBROWN",
            Colors.SandyBrown
        },
        {
            "SEAGREEN",
            Colors.SeaGreen
        },
        {
            "SEASHELL",
            Colors.Seashell
        },
        {
            "SIENNA",
            Colors.Sienna
        },
        {
            "SILVER",
            Colors.Silver
        },
        {
            "SKYBLUE",
            Colors.SkyBlue
        },
        {
            "SLATEBLUE",
            Colors.SlateBlue
        },
        {
            "SLATEGRAY",
            Colors.SlateGray
        },
        {
            "SNOW",
            Colors.Snow
        },
        {
            "SPRINGGREEN",
            Colors.SpringGreen
        },
        {
            "STEELBLUE",
            Colors.SteelBlue
        },
        {
            "TAN",
            Colors.Tan
        },
        {
            "TEAL",
            Colors.Teal
        },
        {
            "THISTLE",
            Colors.Thistle
        },
        {
            "TOMATO",
            Colors.Tomato
        },
        {
            "TRANSPARENT",
            Colors.Transparent
        },
        {
            "TURQUOISE",
            Colors.Turquoise
        },
        {
            "VIOLET",
            Colors.Violet
        },
        {
            "WEBGRAY",
            Colors.WebGray
        },
        {
            "WEBGREEN",
            Colors.WebGreen
        },
        {
            "WEBMAROON",
            Colors.WebMaroon
        },
        {
            "WEBPURPLE",
            Colors.WebPurple
        },
        {
            "WHEAT",
            Colors.Wheat
        },
        {
            "WHITE",
            Colors.White
        },
        {
            "WHITESMOKE",
            Colors.WhiteSmoke
        },
        {
            "YELLOW",
            Colors.Yellow
        },
        {
            "YELLOWGREEN",
            Colors.YellowGreen
        }
    };

    public static Color AliceBlue => new Color(4042850303U);

    public static Color AntiqueWhite => new Color(4209760255U);

    public static Color Aqua => new Color(16777215U /*0xFFFFFF*/);

    public static Color Aquamarine => new Color(2147472639U);

    public static Color Azure => new Color(4043309055U);

    public static Color Beige => new Color(4126530815U);

    public static Color Bisque => new Color(4293182719U);

    public static Color Black => new Color((uint)byte.MaxValue);

    public static Color BlanchedAlmond => new Color(4293643775U);

    public static Color Blue => new Color((uint)ushort.MaxValue);

    public static Color BlueViolet => new Color(2318131967U);

    public static Color Brown => new Color(2771004159U);

    public static Color Burlywood => new Color(3736635391U);

    public static Color CadetBlue => new Color(1604231423U);

    public static Color Chartreuse => new Color(2147418367U);

    public static Color Chocolate => new Color(3530104575U);

    public static Color Coral => new Color(4286533887U);

    public static Color CornflowerBlue => new Color(1687547391U);

    public static Color Cornsilk => new Color(4294499583U);

    public static Color Crimson => new Color(3692313855U);

    public static Color Cyan => new Color(16777215U /*0xFFFFFF*/);

    public static Color DarkBlue => new Color(35839U);

    public static Color DarkCyan => new Color(9145343U);

    public static Color DarkGoldenrod => new Color(3095792639U);

    public static Color DarkGray => new Color(2846468607U);

    public static Color DarkGreen => new Color(6553855U);

    public static Color DarkKhaki => new Color(3182914559U);

    public static Color DarkMagenta => new Color(2332068863U);

    public static Color DarkOliveGreen => new Color(1433087999U);

    public static Color DarkOrange => new Color(4287365375U);

    public static Color DarkOrchid => new Color(2570243327U);

    public static Color DarkRed => new Color(2332033279U);

    public static Color DarkSalmon => new Color(3918953215U);

    public static Color DarkSeaGreen => new Color(2411499519U);

    public static Color DarkSlateBlue => new Color(1211993087U);

    public static Color DarkSlateGray => new Color(793726975U);

    public static Color DarkTurquoise => new Color(13554175U);

    public static Color DarkViolet => new Color(2483082239U);

    public static Color DeepPink => new Color(4279538687U);

    public static Color DeepSkyBlue => new Color(12582911U /*0xBFFFFF*/);

    public static Color DimGray => new Color(1768516095U);

    public static Color DodgerBlue => new Color(512819199U);

    public static Color Firebrick => new Color(2988581631U);

    public static Color FloralWhite => new Color(4294635775U);

    public static Color ForestGreen => new Color(579543807U);

    public static Color Fuchsia => new Color(4278255615U);

    public static Color Gainsboro => new Color(3705462015U);

    public static Color GhostWhite => new Color(4177068031U);

    public static Color Gold => new Color(4292280575U);

    public static Color Goldenrod => new Color(3668254975U);

    public static Color Gray => new Color(3200171775U);

    public static Color Green => new Color(16711935U);

    public static Color GreenYellow => new Color(2919182335U);

    public static Color Honeydew => new Color(4043305215U /*0xF0FFF0FF*/);

    public static Color HotPink => new Color(4285117695U);

    public static Color IndianRed => new Color(3445382399U);

    public static Color Indigo => new Color(1258324735U);

    public static Color Ivory => new Color(4294963455U);

    public static Color Khaki => new Color(4041641215U);

    public static Color Lavender => new Color(3873897215U);

    public static Color LavenderBlush => new Color(4293981695U);

    public static Color LawnGreen => new Color(2096890111U);

    public static Color LemonChiffon => new Color(4294626815U);

    public static Color LightBlue => new Color(2916673279U);

    public static Color LightCoral => new Color(4034953471U);

    public static Color LightCyan => new Color(3774873599U);

    public static Color LightGoldenrod => new Color(4210742015U);

    public static Color LightGray => new Color(3553874943U);

    public static Color LightGreen => new Color(2431553791U);

    public static Color LightPink => new Color(4290167295U);

    public static Color LightSalmon => new Color(4288707327U);

    public static Color LightSeaGreen => new Color(548580095U);

    public static Color LightSkyBlue => new Color(2278488831U);

    public static Color LightSlateGray => new Color(2005441023U);

    public static Color LightSteelBlue => new Color(2965692159U);

    public static Color LightYellow => new Color(4294959359U);

    public static Color Lime => new Color(16711935U);

    public static Color LimeGreen => new Color(852308735U);

    public static Color Linen => new Color(4210091775U);

    public static Color Magenta => new Color(4278255615U);

    public static Color Maroon => new Color(2955960575U);

    public static Color MediumAquamarine => new Color(1724754687U);

    public static Color MediumBlue => new Color(52735U);

    public static Color MediumOrchid => new Color(3126187007U);

    public static Color MediumPurple => new Color(2473647103U);

    public static Color MediumSeaGreen => new Color(1018393087U);

    public static Color MediumSlateBlue => new Color(2070474495U);

    public static Color MediumSpringGreen => new Color(16423679U);

    public static Color MediumTurquoise => new Color(1221709055U);

    public static Color MediumVioletRed => new Color(3340076543U);

    public static Color MidnightBlue => new Color(421097727U);

    public static Color MintCream => new Color(4127193855U);

    public static Color MistyRose => new Color(4293190143U);

    public static Color Moccasin => new Color(4293178879U);

    public static Color NavajoWhite => new Color(4292783615U);

    public static Color NavyBlue => new Color(33023U);

    public static Color OldLace => new Color(4260751103U);

    public static Color Olive => new Color(2155872511U);

    public static Color OliveDrab => new Color(1804477439U);

    public static Color Orange => new Color(4289003775U);

    public static Color OrangeRed => new Color(4282712319U);

    public static Color Orchid => new Color(3664828159U);

    public static Color PaleGoldenrod => new Color(4008225535U);

    public static Color PaleGreen => new Color(2566625535U);

    public static Color PaleTurquoise => new Color(2951671551U);

    public static Color PaleVioletRed => new Color(3681588223U);

    public static Color PapayaWhip => new Color(4293907967U);

    public static Color PeachPuff => new Color(4292524543U);

    public static Color Peru => new Color(3448061951U);

    public static Color Pink => new Color(4290825215U);

    public static Color Plum => new Color(3718307327U);

    public static Color PowderBlue => new Color(2967529215U);

    public static Color Purple => new Color(2686513407U);

    public static Color RebeccaPurple => new Color(1714657791U);

    public static Color Red => new Color(4278190335U);

    public static Color RosyBrown => new Color(3163525119U);

    public static Color RoyalBlue => new Color(1097458175U);

    public static Color SaddleBrown => new Color(2336560127U);

    public static Color Salmon => new Color(4202722047U);

    public static Color SandyBrown => new Color(4104413439U);

    public static Color SeaGreen => new Color(780883967U);

    public static Color Seashell => new Color(4294307583U);

    public static Color Sienna => new Color(2689740287U);

    public static Color Silver => new Color(3233857791U);

    public static Color SkyBlue => new Color(2278484991U);

    public static Color SlateBlue => new Color(1784335871U);

    public static Color SlateGray => new Color(1887473919U);

    public static Color Snow => new Color(4294638335U);

    public static Color SpringGreen => new Color(16744447U);

    public static Color SteelBlue => new Color(1182971135U);

    public static Color Tan => new Color(3535047935U);

    public static Color Teal => new Color(8421631U);

    public static Color Thistle => new Color(3636451583U);

    public static Color Tomato => new Color(4284696575U);

    public static Color Transparent => new Color(4294967040U);

    public static Color Turquoise => new Color(1088475391U);

    public static Color Violet => new Color(4001558271U);

    public static Color WebGray => new Color(2155905279U);

    public static Color WebGreen => new Color(8388863U);

    public static Color WebMaroon => new Color(2147483903U /*0x800000FF*/);

    public static Color WebPurple => new Color(2147516671U);

    public static Color Wheat => new Color(4125012991U);

    public static Color White => new Color(uint.MaxValue);

    public static Color WhiteSmoke => new Color(4126537215U);

    public static Color Yellow => new Color(4294902015U);

    public static Color YellowGreen => new Color(2597139199U);
}