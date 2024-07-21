using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorPaletteBuilder.Services.Converters
{
   public class BoolToTextWrappingConverter : IValueConverter
    {
          public object Convert( object value, Type targetType, object parameter, string language )
          {
               if ( value is bool boolValue )
               {
                    return boolValue ? TextWrapping.Wrap : TextWrapping.NoWrap;
               }
               return TextWrapping.NoWrap;
          }

          public object ConvertBack( object value, Type targetType, object parameter, string language )
          {
               if ( value is TextWrapping wrapping )
               {
                    return wrapping == TextWrapping.Wrap;
               }
               return false;
          }
     }
}
