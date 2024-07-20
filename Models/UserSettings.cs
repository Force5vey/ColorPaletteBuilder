using Microsoft.UI.System;
using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ColorPaletteBuilder
{
     public class UserSettings :INotifyPropertyChanged
     {
          ApplicationTheme _theme;
          bool _autoSave;
          int _autoSaveInterval; // In Seconds
          bool _backupSave;
          int _backupSaveInterval; // In Seconds
          bool _copyWithHashtag;
          string _preferredPaletteSaveFolder;

          AppConstants.SnippetLanguage _snippetLanguage;
          string _snippet;
          string _snippetCustom;
          string _snippetCSharp;
          string _snippetJavascript;
          string _snippetPython;


          public ApplicationTheme Theme
          {
               get => _theme;
               set => SetProperty(ref _theme, value);
          }

          public bool AutoSave
          {
               get => _autoSave;
               set => SetProperty(ref _autoSave, value);
          }

          public int AutoSaveInterval
          {
               get => _autoSaveInterval;
               set => SetProperty(ref _autoSaveInterval, value);
          }
          public bool BackupSave
          {
               get => _backupSave;
               set => SetProperty(ref _backupSave, value);
          }

          public int BackupSaveInterval
          {
               get => _backupSaveInterval;
               set => SetProperty(ref _backupSaveInterval, value);
          }

          public bool CopyWithHashtag
          {
               get => _copyWithHashtag;
               set => SetProperty(ref _copyWithHashtag, value);
          }

          public string PreferredPaletteSaveFolder
          {
               get => _preferredPaletteSaveFolder;
               set => SetProperty(ref _preferredPaletteSaveFolder, value);
          }

          public AppConstants.SnippetLanguage SnippetLanguage
          {
               get => _snippetLanguage;
               set => SetProperty(ref _snippetLanguage, value);
          }

          public string Snippet
          {
               get => _snippet;
               set => SetProperty(ref _snippet, value);
          }

          public string SnippetCustom
          {
               get => _snippetCustom;
               set => SetProperty(ref _snippetCustom, value);
          }

          public string SnippetCSharp
          {
               get => _snippetCSharp;
               set => SetProperty(ref _snippetCSharp, value);
          }

          public string SnippetJavascript
          {
               get => _snippetJavascript;
               set => SetProperty(ref _snippetJavascript, value);
          }

          public string SnippetPython
          {
               get => _snippetPython;
               set => SetProperty(ref _snippetPython, value);
          }

          // Constructor
          public UserSettings()
          {
               Theme = ApplicationTheme.Dark;
               AutoSave = true;
               AutoSaveInterval = 60;
               BackupSaveInterval = 60;
               PreferredPaletteSaveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
               CopyWithHashtag = true;
               SnippetLanguage = AppConstants.SnippetLanguage.CSharp;
               Snippet = "new SolidColorBrush(Color.FromArgb($a, $r, $g, $b));";
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