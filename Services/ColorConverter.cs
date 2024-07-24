using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace ColorPaletteBuilder
{
     public class ColorConverter
     {

          public static Windows.UI.Color ConvertColorToWinUIColor( System.Drawing.Color color )
          {
               return Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B);
          }

          public static System.Drawing.Color ConvertColorToSysDrawColor( Windows.UI.Color color )
          {
               return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
          }

          public static string ToHex( System.Drawing.Color color, bool includeAlpha = true )
          {
               return includeAlpha
                   ? $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}"
                   : $"#{color.R:X2}{color.G:X2}{color.B:X2}";
          }

          public static System.Drawing.Color FromHex( string hex )
          {
               if ( string.IsNullOrWhiteSpace(hex) )
               {
                    hex = "00000000";
               }
               hex = hex.Replace("#", string.Empty);

               if ( hex.Length == 3 )
               {
                    hex = string.Concat(hex[0], hex[0], hex[1], hex[1], hex[2], hex[2]);
               }

               if ( hex.Length == 6 )
               {
                    hex = "FF" + hex; // adding full alpha opacity alpha value
               }

               if ( hex.Length != 8 || !IsValidHex(hex) )
               {
                    hex = "00000000";
               }

               byte a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
               byte r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
               byte g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
               byte b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);

               return System.Drawing.Color.FromArgb(a, r, g, b);
          }

          private static bool IsValidHex( string hex )
          {
               foreach ( char c in hex )
               {
                    bool isHexDigit = (c >= '0' && c <= '9') ||
                               (c >= 'A' && c <= 'F') ||
                               (c >= 'a' && c <= 'f');

                    if ( !isHexDigit )
                    {
                         return false;
                    }
               }
               return true;
          }

          public static SolidColorBrush ConvertToSolidColorBrush( System.Drawing.Color color )
          {
               byte a = color.A;
               byte r = color.R;
               byte g = color.G;
               byte b = color.B;

               return new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
          }
     }
}
