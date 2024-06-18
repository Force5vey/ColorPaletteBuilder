using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;

namespace ColorPaletteBuilder
{
    public class ColorPalette
    {
        public string ColorPaletteName { get; set; }
        public ObservableCollection<ColorEntry> ColorEntries { get; set; }
        public List<string> ElementStates { get; set; }
        public List<string> ElementGroups { get; set; }

    }
}
