using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorPaletteBuilder
{
     public class UserSettings
     {
          public string Theme { get; set; }

          public UserSettings()
          {
               Theme = "Dark";
          }
     }
}
