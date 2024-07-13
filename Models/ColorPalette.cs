using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ColorPaletteBuilder
{
     public class ColorPalette :INotifyPropertyChanged
     {
          private int currentEntryIndex;
          private string colorPaletteName;
          private string colorPaletteFile;
          private bool isSaved;

          private ObservableCollection<string> elementStates;
          private ObservableCollection<string> elementGroups;
          private ObservableCollection<ColorEntry> colorEntries;
          private ObservableCollection<ColorEntry> filteredColorEntries;
          private string _colorSelectorSource;



          public ColorPalette()
          {
               FileHeader = MakeFileHeader();

               CurrentEntryIndex = 0;

               ColorPaletteName = "Color Palette";
               ColorPaletteFile = "New Palette";

               IsSaved = false;

               ElementStates = new ObservableCollection<string>();
               ElementGroups = new ObservableCollection<string>();
               ColorEntries = new ObservableCollection<ColorEntry>();
               FilteredColorEntries = new ObservableCollection<ColorEntry>();

          }

          public int CurrentEntryIndex
          {
               get => currentEntryIndex;
               set => SetProperty(ref currentEntryIndex, value);
          }

          public string FileHeader { get; set; }

          public string ColorSelectorSource
          {
               get => _colorSelectorSource;
               set => SetProperty(ref _colorSelectorSource, value);
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

          public bool IsSaved
          {
               get => isSaved;
               set => SetProperty(ref isSaved, value);
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

          public ObservableCollection<ColorEntry> FilteredColorEntries
          {
               get => filteredColorEntries;
               set => SetProperty(ref filteredColorEntries, value);
          }



          public event PropertyChangedEventHandler PropertyChanged;

          protected bool SetProperty<T>( ref T storage, T value, [CallerMemberName] string propertyName = null )
          {
               if ( Equals(storage, value) )
                    return false;

               storage = value;
               OnPropertyChanged(propertyName);
               return true;
          }

          protected void OnPropertyChanged( string propertyName )
          {
               PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
          }

          private string MakeFileHeader()
          {
               return "Color Palette Builder - Force5 Development - apps@force5dev.com";
          }
     }
}