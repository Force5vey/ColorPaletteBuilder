using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ColorPaletteBuilder
{
     public class ColorPalette :INotifyPropertyChanged
     {
          private int highestEntryIndex;
          private string fileName;
          private string fullFilePath;
          private bool isSaved;

          private ObservableCollection<string> elementStates;
          private ObservableCollection<string> elementGroups;
          private ObservableCollection<ColorEntry> colorEntries;
          private ObservableCollection<ColorEntry> filteredColorEntries;
          private string _colorSelectorSource;



          public ColorPalette()
          {
               HighestEntryIndex = 0;

               FileName = "Color Palette";
               FullFilePath = "New Palette";

               IsSaved = false;

               ElementStates = new ObservableCollection<string>();
               ElementGroups = new ObservableCollection<string>();
               ColorEntries = new ObservableCollection<ColorEntry>();
               FilteredColorEntries = new ObservableCollection<ColorEntry>();

          }

          public int HighestEntryIndex
          {
               get => highestEntryIndex;
               set => SetProperty(ref highestEntryIndex, value);
          }

          public string ColorSelectorSource
          {
               get => _colorSelectorSource;
               set => SetProperty(ref _colorSelectorSource, value);
          }

          public string FileName
          {
               get => fileName;
               set => SetProperty(ref fileName, value);
          }

          public string FullFilePath
          {
               get => fullFilePath;
               set => SetProperty(ref fullFilePath, value);
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

     }
}