using Microsoft.UI.Xaml.Markup;
using System;
using System.Collections.Generic;
using Windows.UI;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;


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
                    var color = (Color)XamlBindingHelper.ConvertValue(typeof(Color), hex);
                    var darkerColor = Color.FromArgb(color.A, (byte)(color.R * 0.8), (byte)(color.G * 0.8), (byte)(color.B * 0.8));
                    return new SolidColorBrush(darkerColor);
                }
                catch
                {
                    return new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                }
            }
            return new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

    }
}
