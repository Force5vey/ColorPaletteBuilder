using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorPaletteBuilder
{
    public class ColorEntry
    {
        public string Name { get; set; }
        public string State { get; set; }
        public string ElementGroup { get; set; }
        public string HexCode { get; set; }
        public double Alpha { get; set; }
      public  Microsoft.UI.Xaml.Media.Brush SelectedColor { get; set; }
    }
}
