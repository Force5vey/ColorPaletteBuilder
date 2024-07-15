using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Microsoft.UI.Xaml;
using System.Drawing;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Windowing;
using System.Drawing.Text;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using Windows.Security.Authentication.Web.Core;

namespace ColorPaletteBuilder
{
     internal class MainViewModel
     {
          public ColorPalette ColorPaletteData { get; set; } = new ColorPalette();
          public string SelectedState { get; set; }
          public string SelectedGroup { get; set; }
          private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

          internal WriteableBitmap DefaultColorSelectorImage { get; private set; }


          public MainViewModel()
          {
               // Initialize default values
               SelectedState = AppConstants.DefaultComboBoxText;
               SelectedGroup = AppConstants.DefaultComboBoxText;
          }


          public async Task<AppConstants.ReturnCode> OpenPalette_Async( Window window )
          {
               var picker = new FileOpenPicker();
               var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
               WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
               picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
               picker.FileTypeFilter.Add(".cpb");

               StorageFile file = await picker.PickSingleFileAsync();
               if ( file != null )
               {
                    await LoadPalette_Async(file.Path);

                    localSettings.Values[AppConstants.LastOpenedFilePath] = file.Path;

                    return AppConstants.ReturnCode.Success;
               }

               return AppConstants.ReturnCode.FileNotFound;
          }

          public async Task LoadPalette_Async( string paletteFilePath )
          {
               if ( !string.IsNullOrEmpty(paletteFilePath) )
               {
                    ColorPalette colorPalette = await FileService.DeserializePaletteFile_Async(paletteFilePath);
                    if ( colorPalette != null )
                    {
                         ClearColorPaletteData(true);

                         // Set Initial values for a palette that is loaded from file
                         ColorPaletteData.ColorPaletteName = colorPalette.ColorPaletteName;
                         ColorPaletteData.ColorPaletteFile = colorPalette.ColorPaletteFile;

                         ColorPaletteData.ColorSelectorSource = colorPalette.ColorSelectorSource;

                         foreach ( var entry in colorPalette.ColorEntries )
                         {
                              ColorPaletteData.ColorEntries.Add(entry);
                         }
                         foreach ( var group in colorPalette.ElementGroups )
                         {
                              if ( group != AppConstants.DefaultComboBoxText )
                              {
                                   ColorPaletteData.ElementGroups.Add(group);
                              }
                         }
                         foreach ( var state in colorPalette.ElementStates )
                         {
                              if ( state != AppConstants.DefaultComboBoxText )
                              {
                                   ColorPaletteData.ElementStates.Add(state);
                              }
                         }


                         // Check for entries, if it is zero then the loop can't run; just assign zero to start at beginning.
                         if ( ColorPaletteData.ElementStates.Count > 0 )
                         {
                              int maxIndex = int.MinValue;

                              foreach ( var entry in ColorPaletteData.ColorEntries )
                              {
                                   if ( entry.ElementIndex > maxIndex )
                                   {
                                        maxIndex = entry.ElementIndex;
                                   }
                              }
                              ColorPaletteData.CurrentEntryIndex = maxIndex + 1;
                         }
                         else
                         {
                              ColorPaletteData.CurrentEntryIndex = 0;
                         }

                         ApplyFilter();
                    }
               }
               else
               {
                    ClearColorPaletteData(false);
                    ApplyFilter();
               }

          }


          public async Task<AppConstants.ReturnCode> SavePaletteAs_Async( Window window )
          {
               try
               {
                    var picker = new FileSavePicker();
                    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                    WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
                    picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    picker.FileTypeChoices.Add("Color Palette Builder", new List<string> { ".cpb" });
                    picker.DefaultFileExtension = ".cpb";

                    StorageFile file = picker.PickSaveFileAsync().AsTask().Result;
                    if ( file != null )
                    {
                         ColorPaletteData.ColorPaletteFile = file.Path;
                         ColorPaletteData.ColorPaletteName = file.Name;

                         await SavePaletteToFile_Async();
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

          public async Task<AppConstants.ReturnCode> SavePaletteToFile_Async()
          {
               StorageFile file = await StorageFile.GetFileFromPathAsync(ColorPaletteData.ColorPaletteFile);
               if ( file != null )
               {
                    ColorPaletteData.IsSaved = true;
                    await FileService.SavePalette_Async(file.Path, ColorPaletteData);

                    localSettings.Values[AppConstants.LastOpenedFilePath] = file.Path;
                    return AppConstants.ReturnCode.Success;
               }
               else
               {
                    ColorPaletteData.IsSaved = false;
                    return AppConstants.ReturnCode.GeneralFailure;
               }
          }


          public async Task<AppConstants.ReturnCode> AutoSaveBackup_Async()
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


          public void ClearColorPaletteData( bool isSaved )
          {
               ColorPaletteData.CurrentEntryIndex = 0;
               ColorPaletteData.ColorPaletteName = "New Palette";
               ColorPaletteData.ColorPaletteFile = "New Palette";


               ColorPaletteData.ElementStates.Clear();
               ColorPaletteData.ElementGroups.Clear();

               ColorPaletteData.ElementStates.Add(AppConstants.DefaultComboBoxText);
               ColorPaletteData.ElementGroups.Add(AppConstants.DefaultComboBoxText);

               ColorPaletteData.ColorEntries.Clear();
               ColorPaletteData.FilteredColorEntries.Clear();

               SelectedState = AppConstants.DefaultComboBoxText;
               SelectedGroup = AppConstants.DefaultComboBoxText;

               ColorPaletteData.IsSaved = isSaved;

          }

          public void ApplyFilter()
          {
               ColorPaletteData.FilteredColorEntries.Clear();
               foreach ( var colorEntry in ColorPaletteData.ColorEntries )
               {
                    if ( SelectedState == AppConstants.DefaultComboBoxText && SelectedGroup == AppConstants.DefaultComboBoxText )
                    {
                         ColorPaletteData.FilteredColorEntries.Add(colorEntry);
                    }
                    else if ( SelectedState == AppConstants.DefaultComboBoxText && colorEntry.ElementGroup == SelectedGroup )
                    {
                         ColorPaletteData.FilteredColorEntries.Add(colorEntry);
                    }
                    else if ( SelectedGroup == AppConstants.DefaultComboBoxText && colorEntry.ElementState == SelectedState )
                    {
                         ColorPaletteData.FilteredColorEntries.Add(colorEntry);
                    }
                    else if ( colorEntry.ElementState == SelectedState && colorEntry.ElementGroup == SelectedGroup )
                    {
                         ColorPaletteData.FilteredColorEntries.Add(colorEntry);
                    }
               }
          }

          public void SortFilteredColorEntries( AppConstants.SortCriteria criteria, bool isAscending )
          {
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

          internal void RemoveColorEntry( ColorEntry selectedEntry )
          {
               if ( selectedEntry != null )
               {
                    ColorPaletteData.ColorEntries.Remove(selectedEntry);
               }
          }

          internal AppConstants.ReturnCode RemoveState( string stateToRemove )
          {
               AppConstants.ReturnCode returnCode;
               // Set each ColorEntry with the state to remove to an empty string
               foreach ( var colorEntry in ColorPaletteData.ColorEntries )
               {
                    if ( colorEntry.ElementState == stateToRemove )
                    {
                         colorEntry.ElementState = string.Empty;
                    }
               }

               if ( stateToRemove != AppConstants.DefaultComboBoxText )
               {
                    ColorPaletteData.ElementStates.Remove(stateToRemove);
                    returnCode = AppConstants.ReturnCode.Success;
               }
               else
               {
                    returnCode = AppConstants.ReturnCode.Unauthorized;
               }

               return returnCode;

          }

          internal AppConstants.ReturnCode RemoveGroup( string groupToRemove )
          {
               AppConstants.ReturnCode returnCode;
               // Set each ColorEntry with the group to remove to an empty string
               foreach ( var colorEntry in ColorPaletteData.ColorEntries )
               {
                    if ( colorEntry.ElementGroup == groupToRemove )
                    {
                         colorEntry.ElementGroup = string.Empty;
                    }
               }

               if ( groupToRemove != AppConstants.DefaultComboBoxText )
               {
                    ColorPaletteData.ElementGroups.Remove(groupToRemove);
                    returnCode = AppConstants.ReturnCode.Success;
               }
               else
               {
                    returnCode = AppConstants.ReturnCode.Unauthorized;
               }

               return returnCode;
          }

          internal async Task LoadDefaultColorSelectorImage()
          {
               DefaultColorSelectorImage = await FileService.LoadDefaultColorSelectorImage();
          }

          internal async Task<AppConstants.ReturnCode> SelectColorSelectorPhoto( Window window )
          {
               AppConstants.ReturnCode returnCode;
               // Initialize the picker
               FileOpenPicker picker = new FileOpenPicker
               {
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary // More appropriate start location for images
               };

               // Initialize with window handle
               var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
               WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

               // Add file type filters
               picker.FileTypeFilter.Add(".bmp");
               picker.FileTypeFilter.Add(".jpg");
               picker.FileTypeFilter.Add(".jpeg");
               picker.FileTypeFilter.Add(".png");

               // Pick a single file
               StorageFile file = await picker.PickSingleFileAsync();
               if ( file != null )
               {
                    // Store the path in palette data for last used retrieval
                    ColorPaletteData.ColorSelectorSource = file.Path;
                    returnCode = AppConstants.ReturnCode.Success;
               }
               else
               {
                    returnCode = AppConstants.ReturnCode.FileNotFound;
               }

               return returnCode;
          }

          internal async Task<AppConstants.ReturnCode> ProcessColorSelectorImage_Async( string imagePath )
          {
               AppConstants.ReturnCode returnCode;
               try
               {
                    if ( File.Exists(imagePath) || !string.IsNullOrEmpty(imagePath) )
                    {
                         using ( IRandomAccessStream fileStream = await StorageFile.GetFileFromPathAsync(imagePath).AsTask().Result.OpenAsync(FileAccessMode.Read) )
                         {
                              BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);
                              WriteableBitmap bitmap = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                              await bitmap.SetSourceAsync(fileStream);

                              // Set the properties
                              App.ColorSelectorBitmap = bitmap;
                              ColorPaletteData.ColorSelectorSource = imagePath;
                         }

                         returnCode = AppConstants.ReturnCode.Success;
                    }
                    else
                    {
                         //TODO: Error handling back to CodeBehind
                         Debug.WriteLine($"File does not exist: {imagePath}");

                         App.ColorSelectorBitmap = DefaultColorSelectorImage;
                         ColorPaletteData.ColorSelectorSource = string.Empty;
                         returnCode = AppConstants.ReturnCode.FileNotFound;
                    }
               }
               catch ( Exception ex )
               {
                    Debug.WriteLine($"Error loading thumbnail image: {ex.Message}");
                    Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                    returnCode = AppConstants.ReturnCode.ProcessingError;
               }

               return returnCode;
          }
     }
}
