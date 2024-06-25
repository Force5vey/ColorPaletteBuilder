using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;

namespace ColorPaletteBuilder
{
    public class CustomGrid : Grid
    {
        public InputCursor InputCursor
        {
            get => ProtectedCursor;
            set => ProtectedCursor = value;
        }
    }
}
