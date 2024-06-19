using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Windows.UI;


namespace ColorPaletteBuilder
{

    public class ColorEntry : INotifyPropertyChanged
    {
        private string elementName;
        private string elementState;
        private string elementGroup;
        private string hexCode;
        private double alpha;
        private string displayColor; // This is a placeholder for the actual color value

        public string ElementName
        {
            get => elementName;
            set => SetProperty(ref elementName, value);
        }

        public string ElementState
        {
            get => elementState;
            set => SetProperty(ref elementState, value);
        }

        public string ElementGroup
        {
            get => elementGroup;
            set => SetProperty(ref elementGroup, value);
        }

        public string HexCode
        {
            get => hexCode;
            set => SetProperty(ref hexCode, value);
        }

        public double Alpha
        {
            get => alpha;
            set => SetProperty(ref alpha, value);
        }

        public string DisplayColor
        {
            get => displayColor;
            set => SetProperty(ref displayColor, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ColorEntry()
        {
            ElementName = string.Empty;
            ElementState = string.Empty;
            ElementGroup = string.Empty;
            HexCode = string.Empty;
            Alpha = 1.0;
            DisplayColor = string.Empty;
        }
    }
}