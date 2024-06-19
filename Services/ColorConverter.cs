using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace ColorPaletteBuilder
{
    public class ColorConverter
    {
        public static string ToHex(Color color, bool includeAlpha = true)
        {
            return includeAlpha
                ? $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}"
                : $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        public static Color FromHex(string hex)
        {
            if(string.IsNullOrWhiteSpace(hex))
            {
                throw new ArgumentException("Invalid hex string", nameof(hex));
            }
            hex = hex.Replace("#", string.Empty);

            if (hex.Length == 6)
            {
                hex = "FF" + hex; // adding full alpha opacity alpha value
            }

            if (hex.Length != 8)
            {
                throw new ArgumentException("Invalid hex string", nameof(hex));
            }

            byte a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            byte r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);

            return Color.FromArgb(a, r, g, b);
        }

        public static SolidColorBrush ToBrush(string hex)
        {
            return new SolidColorBrush(FromHex(hex));
        }

    }
}
