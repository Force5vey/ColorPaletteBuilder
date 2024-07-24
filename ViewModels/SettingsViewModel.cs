using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Security.Cryptography.X509Certificates;
using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;
using Windows.Storage;

namespace ColorPaletteBuilder
{
     internal class SettingsViewModel :INotifyPropertyChanged
     {
          private UserSettings _userSettings;

          public SettingsViewModel( UserSettings userSettings )
          {
               _userSettings = userSettings;
               UpdateSnippet();
          }

          #region // Theme ...
          public int ThemeSelection
          {
               get => (int)_userSettings.Theme;
               set
               {
                    if ( (int)_userSettings.Theme != value )
                    {
                         _userSettings.Theme = (ApplicationTheme)value;
                         OnPropertyChanged();
                         OnPropertyChanged(nameof(ThemeMessage));
                    }
               }
          }

          public string ThemeMessage
          {
               get
               {
                    return ThemeSelection switch
                    {
                         0 => _userSettings.Theme == ApplicationTheme.Light ? "" : "Requires restart.",
                         1 => _userSettings.Theme == ApplicationTheme.Dark ? "" : "Requires restart.",
                         _ => ""
                    };
               }
          }

          #endregion

          #region // Auto Save and BackupSave ...

          public bool AutoSave
          {
               get => _userSettings.AutoSave;
               set
               {
                    if ( _userSettings.AutoSave != value )
                    {
                         _userSettings.AutoSave = value;
                         OnPropertyChanged();
                         OnPropertyChanged(nameof(AutoSave));
                    }
               }
          }

          public int AutoSaveInterval
          {
               get => _userSettings.AutoSaveInterval;
               set
               {
                    if ( _userSettings.AutoSaveInterval != value )
                    {
                         _userSettings.AutoSaveInterval = value;
                         OnPropertyChanged();
                         OnPropertyChanged(nameof(AutoSaveIntervalMinutes));
                         OnPropertyChanged(nameof(AutoSaveIntervalRemainingSeconds));
                    }
               }
          }

          public int AutoSaveIntervalMinutes => (int)Math.Floor((double)_userSettings.AutoSaveInterval / 60);

          public int AutoSaveIntervalRemainingSeconds
          {
               get
               {
                    double totalMinutes = (double)_userSettings.AutoSaveInterval / 60;
                    double fractionalMinutes = totalMinutes - Math.Floor(totalMinutes);
                    return (int)Math.Round(fractionalMinutes * 60);
               }
          }

          public bool BackupSave
          {
               get => _userSettings.BackupSave;
               set
               {
                    if ( _userSettings.BackupSave != value )
                    {
                         _userSettings.BackupSave = value;
                         OnPropertyChanged();
                         OnPropertyChanged(nameof(_userSettings.BackupSave));
                    }
               }
          }

          public int BackupSaveInterval
          {
               get => _userSettings.BackupSaveInterval;
               set
               {
                    if ( _userSettings.BackupSaveInterval != value )
                    {
                         _userSettings.BackupSaveInterval = value;
                         OnPropertyChanged();
                         OnPropertyChanged(nameof(BackupSaveIntervalMinutes));
                         OnPropertyChanged(nameof(BackupSaveIntervalRemainingSeconds));
                    }
               }
          }

          public double BackupSaveIntervalMinutes => (int)Math.Floor((double)_userSettings.BackupSaveInterval / 60);

          public int BackupSaveIntervalRemainingSeconds
          {
               get
               {
                    double totalMinutes = (double)_userSettings.BackupSaveInterval / 60;
                    double fractionalMinutes = totalMinutes - Math.Floor(totalMinutes);
                    return (int)Math.Round(fractionalMinutes * 60);
               }
          }

          #endregion

          public bool CopyWithHashtag
          {
               get => _userSettings.CopyWithHashtag;
               set
               {
                    if ( _userSettings.CopyWithHashtag != value )
                    {
                         _userSettings.CopyWithHashtag = value;
                         OnPropertyChanged();
                         OnPropertyChanged(nameof(_userSettings.CopyWithHashtag));
                    }
               }
          }


          public string PreferredPaletteSaveFolder
          {
               get => _userSettings.PreferredPaletteSaveFolder;
               set
               {
                    if ( _userSettings.PreferredPaletteSaveFolder != value )
                    {
                         _userSettings.PreferredPaletteSaveFolder = value;
                         OnPropertyChanged();
                    }
               }
          }

          public async void BrowsePreferredPaletteSaveFolder( Window window )
          {
               try
               {
                    var picker = new FolderPicker();
                    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                    WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
                    picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;

                    StorageFolder folder = await picker.PickSingleFolderAsync();
                    if ( folder != null )
                    {
                         PreferredPaletteSaveFolder = folder.Path;

                    }
               }
               catch
               {
                    //TODO: Add additional return codes for null, etc.
               }
          }

          #region // Color Entries Options

          

          #endregion

          #region // Code Snippet ...

          public int SnippetLanguage
          {
               get => (int)_userSettings.SnippetLanguage;
               set
               {
                    if ( (int)_userSettings.SnippetLanguage != value )
                    {
                         _userSettings.SnippetLanguage = (AppConstants.SnippetLanguage)value;
                         OnPropertyChanged();
                         UpdateSnippet();
                    }
               }
          }

          public string Snippet
          {
               get => _userSettings.Snippet;
               set
               {
                    if ( _userSettings.Snippet != value )
                    {
                         _userSettings.Snippet = value;
                         SetSnippetForCurrentLanguage(value);
                         OnPropertyChanged();
                    }
               }
          }

          private string GetSnippetForCurrentLanguage()
          {
               return _userSettings.SnippetLanguage switch
               {
                    AppConstants.SnippetLanguage.CSharp => _userSettings.SnippetCSharp,
                    AppConstants.SnippetLanguage.Javascript => _userSettings.SnippetJavascript,
                    AppConstants.SnippetLanguage.Python => _userSettings.SnippetPython,
                    _ => _userSettings.SnippetCustom,
               };
          }

          private void SetSnippetForCurrentLanguage( string value )
          {
               switch ( _userSettings.SnippetLanguage )
               {
                    case AppConstants.SnippetLanguage.CSharp:
                    _userSettings.SnippetCSharp = value;
                    break;
                    case AppConstants.SnippetLanguage.Javascript:
                    _userSettings.SnippetJavascript = value;
                    break;
                    case AppConstants.SnippetLanguage.Python:
                    _userSettings.SnippetPython = value;
                    break;
                    default:
                    _userSettings.SnippetCustom = value;
                    break;
               }
          }

          private void UpdateSnippet()
          {
               Snippet = GetSnippetForCurrentLanguage();
          }

          #endregion

          #region // Window Controls

          public async Task SaveSettings()
          {
               await SettingsService.SerializeUserSettings_Async();
               App.UserSettings = _userSettings;
          }


          #endregion

          #region // Property Change Events

          public event PropertyChangedEventHandler PropertyChanged;

          protected bool SetProperty<T>( ref T storage, T value, [CallerMemberName] string propertyName = null )
          {
               if ( Equals(storage, value) )
                    return false;

               storage = value;
               OnPropertyChanged(propertyName);
               return true;
          }

          protected void OnPropertyChanged( [CallerMemberName] string propertyName = null )
          {
               PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
          }

          #endregion

     }
}
