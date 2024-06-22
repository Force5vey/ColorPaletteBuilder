using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Globalization;

namespace ColorPaletteBuilder
{
    public class DarkerHexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string hex)
            {
                try
                {
                    var color = FromHex(hex);
                    var darkerColor = DarkenColor(color);
                    return new SolidColorBrush(darkerColor);
                }
                catch
                {
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                }
            }
            return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private Windows.UI.Color FromHex(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                throw new ArgumentException("Invalid hex string", nameof(hex));
            }
            hex = hex.Replace("#", string.Empty);

            if (hex.Length == 6)
            {
                hex = "FF" + hex; // Adding full opacity alpha value
            }

            if (hex.Length != 8)
            {
                throw new ArgumentException("Invalid hex string", nameof(hex));
            }

            byte a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            byte r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);

            return Windows.UI.Color.FromArgb(a, r, g, b);
        }

        private Windows.UI.Color DarkenColor(Windows.UI.Color color)
        {
            return Windows.UI.Color.FromArgb(color.A, (byte)(color.R * 0.8), (byte)(color.G * 0.8), (byte)(color.B * 0.8));
        }
    }
}
