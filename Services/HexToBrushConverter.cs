using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Globalization;

namespace ColorPaletteBuilder
{
    public class HexToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string hex)
            {
                try
                {
                    return new SolidColorBrush(FromHex(hex));
                }
                catch
                {
                    // Conversion error returns default brush
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
    }
}
