using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace ColorPaletteBuilder
{
     public class EnumToStringConverter :IValueConverter
     {

          public object Convert( object value, Type targetType, object parameter, string language )
          {
               return value.ToString();
          }

          public object ConvertBack( object value, Type targetType, object parameter, string language )
          {
               return Enum.Parse(targetType, value.ToString());
          }
     }
}
