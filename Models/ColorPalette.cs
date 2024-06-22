using ColorPaletteBuilder;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace ColorPaletteBuilder
{
    public class ColorPalette : INotifyPropertyChanged
    {
        private string colorPaletteName;
        private string colorPaletteFile;
        private ObservableCollection<string> elementStates;
        private ObservableCollection<string> elementGroups;
        private ObservableCollection<ColorEntry> colorEntries;

        //Display and usage control variables
        private bool isColorAssignEnabled;


        public ColorPalette()
        {
            ColorPaletteName = "Color Palette";
            ColorPaletteFile = "New Palette";
            ElementStates = new ObservableCollection<string>();
            ElementGroups = new ObservableCollection<string>();
            ColorEntries = new ObservableCollection<ColorEntry>();
        }


        public bool IsColorAssignEnabled
        {
            get => isColorAssignEnabled;
            set => SetProperty(ref isColorAssignEnabled, value);
        }

        public string ColorPaletteName
        {
            get => colorPaletteName;
            set => SetProperty(ref colorPaletteName, value);
        }

        public string ColorPaletteFile
        {
            get => colorPaletteFile;
            set => SetProperty(ref colorPaletteFile, value);
        }


        public ObservableCollection<string> ElementStates
        {
            get => elementStates;
            set => SetProperty(ref elementStates, value);
        }

        public ObservableCollection<string> ElementGroups
        {
            get => elementGroups;
            set => SetProperty(ref elementGroups, value);
        }
        public ObservableCollection<ColorEntry> ColorEntries
        {
            get => colorEntries;
            set => SetProperty(ref colorEntries, value);
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
    }
}