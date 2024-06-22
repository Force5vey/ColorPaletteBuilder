using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using Microsoft.UI.Xaml.Markup;



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
                    return ColorConverter.ToBrush(hex);
                }
                catch
                {
                    // Conversion error returns deafult brush
                    return new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                }
            }
            return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
