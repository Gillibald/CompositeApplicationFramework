#region Usings

using System.Windows.Media;

#endregion

namespace CompositeApplicationFramework.Utility.Localization.TypeConverters
{
    #region Dependencies

    

    #endregion

    /// <summary>
    ///     A class with color names.
    /// </summary>
    public static class ColorHelper
    {
        /// <summary>
        ///     Create a color from a 4 byte uint.
        /// </summary>
        /// <param name="code">The color code.</param>
        /// <returns>The color</returns>
        public static Color FromArgb(uint code)
        {
            var c = new Color();

            c.A = (byte) (0x000000FF & (code >> 24));
            c.R = (byte) (0x000000FF & (code >> 16));
            c.G = (byte) (0x000000FF & (code >> 8));
            c.B = (byte) (0x000000FF & code);

            return c;
        }

        /// <summary>
        ///     Select a color based on common names.
        /// </summary>
        /// <param name="name">The color name.</param>
        /// <returns>The color.</returns>
        public static Color FromName(string name)
        {
            switch (name)
            {
                case "Aliceblue":
                    return FromArgb(0xFFF0F8FF);
                case "AntiqueWhite":
                    return FromArgb(0xFFFAEBD7);
                case "Aqua":
                    return FromArgb(0xFF00FFFF);
                case "Aquamarine":
                    return FromArgb(0xFF7FFFD4);
                case "Azure":
                    return FromArgb(0xFFF0FFFF);
                case "Beige":
                    return FromArgb(0xFFF5F5DC);
                case "Bisque":
                    return FromArgb(0xFFFFE4C4);
                case "Black":
                    return FromArgb(0xFF000000);
                case "BlanchedAlmond":
                    return FromArgb(0xFFFFEBCD);
                case "Blue":
                    return FromArgb(0xFF0000FF);
                case "BlueViolet":
                    return FromArgb(0xFF8A2BE2);
                case "Brown":
                    return FromArgb(0xFFA52A2A);
                case "BurlyWood":
                    return FromArgb(0xFFDEB887);
                case "CadetBlue":
                    return FromArgb(0xFF5F9EA0);
                case "Chartreuse":
                    return FromArgb(0xFF7FFF00);
                case "Chocolate":
                    return FromArgb(0xFFD2691E);
                case "Coral":
                    return FromArgb(0xFFFF7F50);
                case "CornflowerBlue":
                    return FromArgb(0xFF6495ED);
                case "Cornsilk":
                    return FromArgb(0xFFFFF8DC);
                case "Crimson":
                    return FromArgb(0xFFDC143C);
                case "Cyan":
                    return FromArgb(0xFF00FFFF);
                case "DarkBlue":
                    return FromArgb(0xFF00008B);
                case "DarkCyan":
                    return FromArgb(0xFF008B8B);
                case "DarkGoldenrod":
                    return FromArgb(0xFFB8860B);
                case "DarkGray":
                    return FromArgb(0xFFA9A9A9);
                case "DarkGreen":
                    return FromArgb(0xFF006400);
                case "DarkKhaki":
                    return FromArgb(0xFFBDB76B);
                case "DarkMagenta":
                    return FromArgb(0xFF8B008B);
                case "DarkOliveGreen":
                    return FromArgb(0xFF556B2F);
                case "DarkOrange":
                    return FromArgb(0xFFFF8C00);
                case "DarkOrchid":
                    return FromArgb(0xFF9932CC);
                case "DarkRed":
                    return FromArgb(0xFF8B0000);
                case "DarkSalmon":
                    return FromArgb(0xFFE9967A);
                case "DarkSeaGreen":
                    return FromArgb(0xFF8FBC8F);
                case "DarkSlateBlue":
                    return FromArgb(0xFF483D8B);
                case "DarkSlateGray":
                    return FromArgb(0xFF2F4F4F);
                case "DarkTurquoise":
                    return FromArgb(0xFF00CED1);
                case "DarkViolet":
                    return FromArgb(0xFF9400D3);
                case "DeepPink":
                    return FromArgb(0xFFFF1493);
                case "DeepSkyBlue":
                    return FromArgb(0xFF00BFFF);
                case "DimGray":
                    return FromArgb(0xFF696969);
                case "DodgerBlue":
                    return FromArgb(0xFF1E90FF);
                case "Firebrick":
                    return FromArgb(0xFFB22222);
                case "FloralWhite":
                    return FromArgb(0xFFFFFAF0);
                case "ForestGreen":
                    return FromArgb(0xFF228B22);
                case "Gainsboro":
                    return FromArgb(0xFFDCDCDC);
                case "GhostWhite":
                    return FromArgb(0xFFF8F8FF);
                case "Gold":
                    return FromArgb(0xFFFFD700);
                case "Goldenrod":
                    return FromArgb(0xFFDAA520);
                case "Gray":
                    return FromArgb(0xFF808080);
                case "Green":
                    return FromArgb(0xFF008000);
                case "GreenYellow":
                    return FromArgb(0xFFADFF2F);
                case "Honeydew":
                    return FromArgb(0xFFF0FFF0);
                case "HotPink":
                    return FromArgb(0xFFFF69B4);
                case "IndianRed":
                    return FromArgb(0xFFCD5C5C);
                case "Indigo":
                    return FromArgb(0xFF4B0082);
                case "Ivory":
                    return FromArgb(0xFFFFFFF0);
                case "Khaki":
                    return FromArgb(0xFFF0E68C);
                case "Lavender":
                    return FromArgb(0xFFE6E6FA);
                case "LavenderBlush":
                    return FromArgb(0xFFFFF0F5);
                case "LawnGreen":
                    return FromArgb(0xFF7CFC00);
                case "LemonChiffon":
                    return FromArgb(0xFFFFFACD);
                case "LightBlue":
                    return FromArgb(0xFFADD8E6);
                case "LightCoral":
                    return FromArgb(0xFFF08080);
                case "LightCyan":
                    return FromArgb(0xFFE0FFFF);
                case "LightGoldenrodYellow":
                    return FromArgb(0xFFFAFAD2);
                case "LightGray":
                    return FromArgb(0xFFD3D3D3);
                case "LightGreen":
                    return FromArgb(0xFF90EE90);
                case "LightPink":
                    return FromArgb(0xFFFFB6C1);
                case "LightSalmon":
                    return FromArgb(0xFFFFA07A);
                case "LightSeaGreen":
                    return FromArgb(0xFF20B2AA);
                case "LightSkyBlue":
                    return FromArgb(0xFF87CEFA);
                case "LightSlateGray":
                    return FromArgb(0xFF778899);
                case "LightSteelBlue":
                    return FromArgb(0xFFB0C4DE);
                case "LightYellow":
                    return FromArgb(0xFFFFFFE0);
                case "Lime":
                    return FromArgb(0xFF00FF00);
                case "LimeGreen":
                    return FromArgb(0xFF32CD32);
                case "Linen":
                    return FromArgb(0xFFFAF0E6);
                case "Magenta":
                    return FromArgb(0xFFFF00FF);
                case "Maroon":
                    return FromArgb(0xFF800000);
                case "MediumAquamarine":
                    return FromArgb(0xFF66CDAA);
                case "MediumBlue":
                    return FromArgb(0xFF0000CD);
                case "MediumOrchid":
                    return FromArgb(0xFFBA55D3);
                case "MediumPurple":
                    return FromArgb(0xFF9370DB);
                case "MediumSeaGreen":
                    return FromArgb(0xFF3CB371);
                case "MediumSlateBlue":
                    return FromArgb(0xFF7B68EE);
                case "MediumSpringGreen":
                    return FromArgb(0xFF00FA9A);
                case "MediumTurquoise":
                    return FromArgb(0xFF48D1CC);
                case "MediumVioletRed":
                    return FromArgb(0xFFC71585);
                case "MidnightBlue":
                    return FromArgb(0xFF191970);
                case "MintCream":
                    return FromArgb(0xFFF5FFFA);
                case "MistyRose":
                    return FromArgb(0xFFFFE4E1);
                case "Moccasin":
                    return FromArgb(0xFFFFE4B5);
                case "NavajoWhite":
                    return FromArgb(0xFFFFDEAD);
                case "Navy":
                    return FromArgb(0xFF000080);
                case "OldLace":
                    return FromArgb(0xFFFDF5E6);
                case "Olive":
                    return FromArgb(0xFF808000);
                case "OliveDrab":
                    return FromArgb(0xFF6B8E23);
                case "Orange":
                    return FromArgb(0xFFFFA500);
                case "OrangeRed":
                    return FromArgb(0xFFFF4500);
                case "Orchid":
                    return FromArgb(0xFFDA70D6);
                case "PaleGoldenrod":
                    return FromArgb(0xFFEEE8AA);
                case "PaleGreen":
                    return FromArgb(0xFF98FB98);
                case "PaleTurquoise":
                    return FromArgb(0xFFAFEEEE);
                case "PaleVioletRed":
                    return FromArgb(0xFFDB7093);
                case "PapayaWhip":
                    return FromArgb(0xFFFFEFD5);
                case "PeachPuff":
                    return FromArgb(0xFFFFDAB9);
                case "Peru":
                    return FromArgb(0xFFCD853F);
                case "Pink":
                    return FromArgb(0xFFFFC0CB);
                case "Plum":
                    return FromArgb(0xFFDDA0DD);
                case "PowderBlue":
                    return FromArgb(0xFFB0E0E6);
                case "Purple":
                    return FromArgb(0xFF800080);
                case "Red":
                    return FromArgb(0xFFFF0000);
                case "RosyBrown":
                    return FromArgb(0xFFBC8F8F);
                case "RoyalBlue":
                    return FromArgb(0xFF4169E1);
                case "SaddleBrown":
                    return FromArgb(0xFF8B4513);
                case "Salmon":
                    return FromArgb(0xFFFA8072);
                case "SandyBrown":
                    return FromArgb(0xFFF4A460);
                case "SeaGreen":
                    return FromArgb(0xFF2E8B57);
                case "SeaShell":
                    return FromArgb(0xFFFFF5EE);
                case "Sienna":
                    return FromArgb(0xFFA0522D);
                case "Silver":
                    return FromArgb(0xFFC0C0C0);
                case "SkyBlue":
                    return FromArgb(0xFF87CEEB);
                case "SlateBlue":
                    return FromArgb(0xFF6A5ACD);
                case "SlateGray":
                    return FromArgb(0xFF708090);
                case "Snow":
                    return FromArgb(0xFFFFFAFA);
                case "SpringGreen":
                    return FromArgb(0xFF00FF7F);
                case "SteelBlue":
                    return FromArgb(0xFF4682B4);
                case "Tan":
                    return FromArgb(0xFFD2B48C);
                case "Teal":
                    return FromArgb(0xFF008080);
                case "Thistle":
                    return FromArgb(0xFFD8BFD8);
                case "Tomato":
                    return FromArgb(0xFFFF6347);
                case "Transparent":
                    return FromArgb(0x00FFFFFF);
                case "Turquoise":
                    return FromArgb(0xFF40E0D0);
                case "Violet":
                    return FromArgb(0xFFEE82EE);
                case "Wheat":
                    return FromArgb(0xFFF5DEB3);
                case "White":
                    return FromArgb(0xFFFFFFFF);
                case "WhiteSmoke":
                    return FromArgb(0xFFF5F5F5);
                case "Yellow":
                    return FromArgb(0xFFFFFF00);
                case "YellowGreen":
                    return FromArgb(0xFF9ACD32);
            }

            return new Color();
        }
    }
}