using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace ColorPaletteBuilder
{
     internal class MainViewModel
     {
          public ColorPalette ColorPaletteData { get; set; } = new ColorPalette();
          public string SelectedState { get; set; }
          public string SelectedGroup { get; set; }
          private const string defaultComboBoxText = "Any";
          private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;




          public MainViewModel()
          {
               // Initialize default values
               SelectedState = defaultComboBoxText;
               SelectedGroup = defaultComboBoxText;
          }

          public async Task<AppConstants.ReturnCode> SavePaletteAs_Async()
          {
               try
               {
                    var picker = new FileSavePicker();
                    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                    WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
                    picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    picker.FileTypeChoices.Add("Color Palette Builder", new List<string> { ".cpb" });
                    picker.DefaultFileExtension = ".cpb";

                    StorageFile file = picker.PickSaveFileAsync().AsTask().Result;
                    if ( file != null )
                    {
                         ColorPaletteData.ColorPaletteFile = file.Path;
                         ColorPaletteData.ColorPaletteName = file.Name;

                         await SavePaletteToFile_Async(file.Path);
                         return AppConstants.ReturnCode.Success;
                    }
                    else
                    {
                         return AppConstants.ReturnCode.FileNotFound;
                    }
               }
               catch
               {
                    //TODO: Add additional return codes for null, etc.
                    return AppConstants.ReturnCode.GeneralFailure;
               }
          }

          public async Task<AppConstants.ReturnCode> AutoSaveBackupAsync()
          {
               AppConstants.ReturnCode saveBackupReturnCode;
               saveBackupReturnCode = await BackupService.SaveBackupAsync(ColorPaletteData);

               if ( saveBackupReturnCode == AppConstants.ReturnCode.Success )
               {
                    return AppConstants.ReturnCode.Success;
               }
               else
               {
                    return AppConstants.ReturnCode.GeneralFailure;
               }
          }

          public async Task LoadPalette_Async( string paletteFilePath )
          {
               if ( !string.IsNullOrEmpty(paletteFilePath) )
               {
                    ColorPalette colorPalette = await FileService.LoadPaletteFile_Async(paletteFilePath);
                    if ( colorPalette != null )
                    {
                         ClearColorPaletteData();

                         ColorPaletteData.ColorPaletteName = colorPalette.ColorPaletteName;
                         ColorPaletteData.ColorPaletteFile = colorPalette.ColorPaletteFile;
                         ColorPaletteData.ColorSelectorSource = colorPalette.ColorSelectorSource;

                         foreach ( var entry in colorPalette.ColorEntries )
                         {
                              ColorPaletteData.ColorEntries.Add(entry);
                         }
                         foreach ( var group in colorPalette.ElementGroups )
                         {
                              if ( group != defaultComboBoxText )
                              {
                                   ColorPaletteData.ElementGroups.Add(group);
                              }
                         }
                         foreach ( var state in colorPalette.ElementStates )
                         {
                              if ( state != defaultComboBoxText )
                              {
                                   ColorPaletteData.ElementStates.Add(state);
                              }
                         }

                         //Color Entries is never decrmented; so counting doesn't work. So get highest value in the Index and use that next
                         //This is primarily for sorting by order addded and is not a unique ID for the entry.
                         ColorPaletteData.CurrentIndex = ColorPaletteData.ColorEntries.Max(entry => entry.ElementIndex) + 1;
                    }
               }
          }

          public void ClearColorPaletteData()
          {
               ColorPaletteData.CurrentIndex = 0;
               ColorPaletteData.ColorPaletteName = "New Palette";
               ColorPaletteData.ColorPaletteFile = "New Palette";

               ColorPaletteData.ElementStates.Clear();
               ColorPaletteData.ElementGroups.Clear();

               ColorPaletteData.ElementStates.Add(defaultComboBoxText);
               ColorPaletteData.ElementGroups.Add(defaultComboBoxText);

               ColorPaletteData.ColorEntries.Clear();
               ColorPaletteData.FilteredColorEntries.Clear();

               SelectedState = defaultComboBoxText;
               SelectedGroup = defaultComboBoxText;

               //ColorSelectorSource to remain, to retain image between Palettes

          }

          public void ApplyFilter()
          {
               ColorPaletteData.FilteredColorEntries.Clear();
               foreach ( var colorEntry in ColorPaletteData.ColorEntries )
               {
                    if ( SelectedState == defaultComboBoxText && SelectedGroup == defaultComboBoxText )
                    {
                         ColorPaletteData.FilteredColorEntries.Add(colorEntry);
                    }
                    else if ( SelectedState == defaultComboBoxText && colorEntry.ElementGroup == SelectedGroup )
                    {
                         ColorPaletteData.FilteredColorEntries.Add(colorEntry);
                    }
                    else if ( SelectedGroup == defaultComboBoxText && colorEntry.ElementState == SelectedState )
                    {
                         ColorPaletteData.FilteredColorEntries.Add(colorEntry);
                    }
                    else if ( colorEntry.ElementState == SelectedState && colorEntry.ElementGroup == SelectedGroup )
                    {
                         ColorPaletteData.FilteredColorEntries.Add(colorEntry);
                    }
               }
          }

          public void SortFilteredColorEntries( FontIcon sortIcon, AppConstants.SortCriteria criteria, ref bool isAscending )
          {
               ResetSortButtons(sortIcon, criteria, ref isAscending);

               var sortedEntries = new List<ColorEntry>(ColorPaletteData.FilteredColorEntries);
               switch ( criteria )
               {
                    case AppConstants.SortCriteria.Index:
                    sortedEntries = isAscending ?
                        sortedEntries.OrderBy(entry => entry.ElementIndex).ToList() :
                        sortedEntries.OrderByDescending(entry => entry.ElementIndex).ToList();
                    break;
                    case AppConstants.SortCriteria.Name:
                    sortedEntries = isAscending ?
                        sortedEntries.OrderBy(entry => entry.ElementName).ToList() :
                        sortedEntries.OrderByDescending(entry => entry.ElementName).ToList();
                    break;
                    case AppConstants.SortCriteria.State:
                    sortedEntries = isAscending ?
                        sortedEntries.OrderBy(entry => entry.ElementState).ToList() :
                        sortedEntries.OrderByDescending(entry => entry.ElementState).ToList();
                    break;
                    case AppConstants.SortCriteria.Group:
                    sortedEntries = isAscending ?
                        sortedEntries.OrderBy(entry => entry.ElementGroup).ToList() :
                        sortedEntries.OrderByDescending(entry => entry.ElementGroup).ToList();
                    break;
                    case AppConstants.SortCriteria.Color:
                    sortedEntries = isAscending ?
                        sortedEntries.OrderBy(entry => entry.HexCode).ToList() :
                        sortedEntries.OrderByDescending(entry => entry.HexCode).ToList();
                    break;
                    case AppConstants.SortCriteria.Note:
                    sortedEntries = isAscending ?
                        sortedEntries.OrderBy(entry => entry.Note).ToList() :
                        sortedEntries.OrderByDescending(entry => entry.Note).ToList();
                    break;
               }
               ColorPaletteData.FilteredColorEntries.Clear();
               foreach ( var entry in sortedEntries )
               {
                    ColorPaletteData.FilteredColorEntries.Add(entry);
               }
          }

          private void ResetSortButtons( FontIcon sortIcon, AppConstants.SortCriteria criteria, ref bool isAscending )
          {
               if ( isAscending )
               {
                    sortIcon.Glyph = "\uE70E";
               }
               else
               {
                    sortIcon.Glyph = "\uE70D";
               }
          }

          public async Task<AppConstants.ReturnCode> SavePaletteToFile_Async( string filePath )
          {
               StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
               if ( file != null )
               {
                    await FileService.SavePalette_Async(file.Path, ColorPaletteData);
                    localSettings.Values[AppConstants.LastOpenedFilePath] = file.Path;
                    return AppConstants.ReturnCode.Success;
               }
               else
               {
                    return AppConstants.ReturnCode.GeneralFailure;
               }
          }

          public async Task LoadLastSession_Async()
          {
               if ( localSettings.Values.TryGetValue(AppConstants.LastOpenedFilePath, out var lastOpenedFilePath) )
               {
                    if ( lastOpenedFilePath != null )
                    {
                         await LoadPalette_Async(lastOpenedFilePath as string);
                    }
               }
          }

          internal void RemoveColorEntry(ColorEntry selectedEntry)
          {
               if (selectedEntry != null )
               {
                    ColorPaletteData.ColorEntries.Remove(selectedEntry);
               }
          }
     }
}
