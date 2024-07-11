using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.ApplicationModel.DataTransfer;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Drawing.Text;
using System.Diagnostics;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;


namespace ColorPaletteBuilder
{
     public sealed partial class MainWindow :Window
     {
          // View Model
          private MainViewModel mainViewModel = new MainViewModel();

          // Window and UI Elements
          private SettingsWindow settingsWindow;
          private ColorSelectorWindow colorSelectorWindow;

          // Constants
          private const int mainWindowMinWidth = 800;
          private const int mainWindowMinHeight = 625;

          private ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;


          // Events
          public event EventHandler SetColorSelectorImage;

          // Public Properties
          //public ColorPalette ColorPaletteData { get; set; } = new ColorPalette();
          public string SelectedState { get; set; }
          public string SelectedGroup { get; set; }

          //public static WriteableBitmap DefaultColorSelectorImage { get; private set; }

          // Private Fields
          private string currentColorPickerHex = string.Empty;
          private string currentColorPickerHexNoAlpha = string.Empty;
          private string currentColorPickerRGB = string.Empty;
          private string currentColorPickerCodeSnippet = string.Empty;

          // Sort and Filter Fields
          private string defaultComboBoxText = "Any";
          private AppConstants.SortCriteria activeSortCriteria = AppConstants.SortCriteria.Index;
          private bool isAscending = true;


          // Timers
          private DispatcherTimer titleBarMessageTimer = new DispatcherTimer();
          private int messageTimerInterval = 2; // seconds

          private DispatcherTimer autoSaveTimer = new DispatcherTimer();
          private int autoSaveInterval = 60; // seconds

          // Miscellaneous


          public MainWindow()
          {
               // Initialization
               this.InitializeComponent();
               InitializeAsync();

               // Setting Default Values
               SelectedState = defaultComboBoxText;
               SelectedGroup = defaultComboBoxText;
               CustomColorPicker.Color = ColorConverter.ConvertColorToWinUIColor(Color.FromArgb(255, Color.White));

               // Event Handlers Setup
               comboElementStates.SelectionChanged += OnFilterSelectionChanged;
               comboElementGroups.SelectionChanged += OnFilterSelectionChanged;
               this.AppWindow.Changed += MainWindow_Changed;
               this.Closed += MainWindow_Closed;

               // Window Size Configuration
               ConfigureWindowSize();

               // Timers
               InitializeMessageTimer();
               //
               InitializeAutoSaveTimer();
          }

          // Initialization Methods
          private async void InitializeAsync()
          {
               DefaultColorSelectorImage = await FileService.LoadDefaultColorSelectorImage();
               ColorSelectorImage.Source = DefaultColorSelectorImage;
               App.ColorSelectorBitmap = DefaultColorSelectorImage;

               await mainViewModel.LoadLastSession_Async();

               ApplyFilter();

               // Data Binding Setup
               ColorPaletteListView.ItemsSource = mainViewModel.ColorPaletteData.FilteredColorEntries;
               ColorPaletteListView.DataContext = mainViewModel.ColorPaletteData.FilteredColorEntries;

               // Last Color Picker Color Used
               if ( localSettings.Values.TryGetValue(AppConstants.LastColorPickerHex, out object lastColorPickerHex) )
               {
                    currentColorPickerHex = lastColorPickerHex as string;
                    CustomColorPicker.Color = ColorConverter.ConvertColorToWinUIColor(ColorConverter.FromHex(currentColorPickerHex));
               }

               //Do a default Sort by Index
               SortFilteredColorEntries(FontIconSortElementIndex, activeSortCriteria);

               LoadThumbNailImage(mainViewModel.ColorPaletteData.ColorSelectorSource);
          }

          private void ConfigureWindowSize()
          {
               try
               {
                    if ( localSettings.Values.TryGetValue(AppConstants.WindowWidth, out object width) &&
                         localSettings.Values.TryGetValue(AppConstants.WindowHeight, out object height) &&
                         localSettings.Values.TryGetValue(AppConstants.WindowLeft, out object left) &&
                         localSettings.Values.TryGetValue(AppConstants.WindowTop, out object top) )
                    {
                         this.AppWindow.Resize(new Windows.Graphics.SizeInt32 { Width = (int)width, Height = (int)height });
                         this.AppWindow.Move(new Windows.Graphics.PointInt32 { X = (int)left, Y = (int)top });
                    }
                    else
                    {
                         // If TryGetValue returns false, set to default size
                         this.AppWindow.Resize(new Windows.Graphics.SizeInt32(mainWindowMinWidth, mainWindowMinHeight));
                    }
               }
               catch
               {
                    // Just assign a default size if there is an error.
                    this.AppWindow.Resize(new Windows.Graphics.SizeInt32(mainWindowMinWidth, mainWindowMinHeight));
               }
          }

          // AppWindow Event Handlers
          private void MainWindow_Changed( Microsoft.UI.Windowing.AppWindow sender, object args )
          {
               if ( sender.Size.Width < mainWindowMinWidth )
               {
                    this.AppWindow.Resize(new Windows.Graphics.SizeInt32(mainWindowMinWidth, (int)sender.Size.Height));
               }
               if ( sender.Size.Height < mainWindowMinHeight )
               {
                    this.AppWindow.Resize(new Windows.Graphics.SizeInt32((int)sender.Size.Width, mainWindowMinHeight));
               }
          }

          private void MainWindow_Closed( object sender, WindowEventArgs e )
          {
               // Save Values to reopen windows at same size as close position.
               localSettings.Values[AppConstants.WindowWidth] = this.AppWindow.Size.Width;
               localSettings.Values[AppConstants.WindowHeight] = this.AppWindow.Size.Height;
               localSettings.Values[AppConstants.WindowLeft] = this.AppWindow.Position.X;
               localSettings.Values[AppConstants.WindowTop] = this.AppWindow.Position.Y;

               localSettings.Values[AppConstants.LastColorPickerHex] = currentColorPickerHex;

               if ( settingsWindow != null )
               {
                    settingsWindow.Close();
               }
               if ( colorSelectorWindow != null )
               {
                    colorSelectorWindow.Close();
               }
          }


          #region // Timer Event Handlers

          private void InitializeAutoSaveTimer()
          {
               autoSaveTimer.Interval = TimeSpan.FromSeconds(autoSaveInterval);
               autoSaveTimer.Tick += AutoSaveBackupTimer_Tick;
               autoSaveTimer.Tick += AutoSaveTimer_Tick;
               autoSaveTimer.Start();
          }

          private void InitializeMessageTimer()
          {
               titleBarMessageTimer.Interval = TimeSpan.FromSeconds(messageTimerInterval);
               titleBarMessageTimer.Tick += TitleMessageTimer_Tick;
          }

          private void TitleMessageTimer_Tick( object sender, object e )
          {
               TitleBarMessage.Text = "";
               titleBarMessageTimer.Stop();
          }

          private async void AutoSaveTimer_Tick( object sender, object e )
          {
               AppConstants.ReturnCode returnCode;
               returnCode = await mainViewModel.SavePaletteToFile_Async(mainViewModel.ColorPaletteData.ColorPaletteFile);

               if ( TitleBarMessage != null || titleBarMessageTimer != null )
               {
                    switch ( returnCode )
                    {
                         case AppConstants.ReturnCode.Success:
                         TitleBarMessage.Text = "Auto-Saved";
                         titleBarMessageTimer.Start();
                         break;
                         case AppConstants.ReturnCode.GeneralFailure:
                         TitleBarMessage.Text = "Auto-Save Failed";
                         titleBarMessageTimer.Start();
                         break;
                    }
               }
          }

          private async void AutoSaveBackupTimer_Tick( object sender, object e )
          {
               AppConstants.ReturnCode returnCode;
               returnCode = await mainViewModel.SavePaletteToFile_Async(AppConstants.BackupFilePath);

               if ( TitleBarMessage != null || titleBarMessageTimer != null )
               {
                    switch ( returnCode )
                    {
                         case AppConstants.ReturnCode.Success:
                         TitleBarMessage.Text = "Auto-Saved To Backup";
                         titleBarMessageTimer.Start();
                         break;
                         case AppConstants.ReturnCode.GeneralFailure:
                         TitleBarMessage.Text = "Auto-Save Failed";
                         titleBarMessageTimer.Start();
                         break;
                    }
               }
          }

          #endregion //End Timer Event Handlers

          #region // Button Click Event Handlers - Color Palette Actions

          private void NewPalette_Click( object sender, RoutedEventArgs e )
          {
               mainViewModel.ClearColorPaletteData();

               //TODO: any ui level clearing or changes
          }

          private async void OpenPalette_Click( object sender, RoutedEventArgs e )
          {
               var picker = new FileOpenPicker();
               var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
               WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
               picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
               picker.FileTypeFilter.Add(".cpb");

               StorageFile file = await picker.PickSingleFileAsync();
               if ( file != null )
               {
                    await mainViewModel.LoadPalette_Async(file.Path);

                    localSettings.Values[AppConstants.LastOpenedFilePath] = file.Path;
               }
          }

          private async void SavePalette_Click( object sender, RoutedEventArgs e )
          {
               if ( string.IsNullOrEmpty(mainViewModel.ColorPaletteData.ColorPaletteFile) || mainViewModel.ColorPaletteData.ColorPaletteFile == "New Palette" )
               {
                    SavePaletteAs_Click(sender, e);
                    return;
               }

               await mainViewModel.SavePaletteToFile_Async(mainViewModel.ColorPaletteData.ColorPaletteFile);
          }

          private async void SavePaletteAs_Click( object sender, RoutedEventArgs e )
          {
               var returnCode = await mainViewModel.SavePaletteAs_Async();
               if ( returnCode == AppConstants.ReturnCode.Success )
               {
                    TitleBarFileName.Text = mainViewModel.ColorPaletteData.ColorPaletteName;
                    TitleBarMessage.Text = "File Saved Succesfully";
                    titleBarMessageTimer.Start();
               }
          }


          // Button Click Event Handlers - Color Entries Management
          private void AddColorEntry_Click( object sender, RoutedEventArgs e )
          {
               ColorEntry newEntry = new ColorEntry
               {
                    ElementIndex = mainViewModel.ColorPaletteData.CurrentIndex++,
                    ElementName = "Name",
                    ElementGroup = mainViewModel.ColorPaletteData.ElementGroups.FirstOrDefault(),
                    ElementState = mainViewModel.ColorPaletteData.ElementStates.FirstOrDefault(),
                    HexCode = currentColorPickerHex
               };

               // TODO: Make user setting to add to top (0) or end (list Count)
               // add settingsValue to appConstants. viewModel method for loading all user settings. pull a value from mainViewModel UserSettings
               // Make a model UserSettings. this can get populated in MainViewModel on initialization us that value below to set insert location.
               // The setting will just be on top or bottom, this will need logic to get the last index then plus 1
               mainViewModel.ColorPaletteData.ColorEntries.Insert(0, newEntry);

               ApplyFilter();

          }

          private void RemoveColorEntry_Click( object sender, RoutedEventArgs e )
          {
               var selectedEntry = ColorPaletteListView.SelectedItem as ColorEntry;
               mainViewModel.RemoveColorEntry(selectedEntry);

               ApplyFilter();
          }

          private void AssignColor_Click( object sender, RoutedEventArgs e )
          {
               var button = sender as Button;
               if ( button != null && button.DataContext is ColorEntry colorEntry )
               {
                    System.Drawing.Color color = ColorConverter.ConvertColorToSysDrawColor(CustomColorPicker.Color);
                    colorEntry.HexCode = ColorConverter.ToHex(color, includeAlpha: true);
               }
          }

          private void SendColor_Click( object sender, RoutedEventArgs e )
          {
               var button = sender as Button;
               if ( button != null && button.DataContext is ColorEntry colorEntry )
               {
                    CustomColorPicker.Color = ColorConverter.ConvertColorToWinUIColor(ColorConverter.FromHex(colorEntry.HexCode));
               }
          }

          // Button Click Event Handlers - Clipboard Actions
          private void CopyHexCode_Click( object sender, RoutedEventArgs e )
          {
               //TODO: User setting for including # or not
               var button = sender as Button;
               if ( button != null && button.DataContext is ColorEntry colorEntry )
               {
                    var dataPackage = new DataPackage();
                    dataPackage.SetText(colorEntry.HexCode);
                    Clipboard.SetContent(dataPackage);

                    TitleBarMessage.Text = "Copied to Clipboard";
                    titleBarMessageTimer.Start();
               }
          }

          // Button Click Event Handlers - Application Actions
          private void MenuExit_Click( object sender, RoutedEventArgs e )
          {
               Application.Current.Exit();
          }

          private void OpenSettings_Click( object sender, RoutedEventArgs e )
          {
               if ( settingsWindow == null )
               {
                    settingsWindow = new SettingsWindow();
                    settingsWindow.Closed += SettingsWindow_Closed;
                    settingsWindow.Activate();
               }
          }


          // Button Click Event Handlers - State and Group Management
          private void AddState_Click( object sender, RoutedEventArgs e )
          {
               if ( mainViewModel.ColorPaletteData.ElementStates.Contains(comboElementStates.Text) )
               {
                    TitleBarMessage.Text = "State already exists";
                    titleBarMessageTimer.Start();
               }
               else
               {
                    mainViewModel.ColorPaletteData.ElementStates.Add(comboElementStates.Text);
               }
          }

          private void RemoveStateConfirmation_Click( object sender, RoutedEventArgs e )
          {
               string stateToRemove = comboElementStates.Text;
               AppConstants.ReturnCode returnCode = mainViewModel.RemoveState(stateToRemove);

               if ( returnCode == AppConstants.ReturnCode.Success )
               {
                    //TODO: Make a method that accepts the text and then starts the timer.
                    comboElementStates.SelectedItem = comboElementStates.Items.FirstOrDefault();

                    TitleBarMessage.Text = $"Removed State: {stateToRemove}";
                    titleBarMessageTimer.Start();
               }
               else
               {
                    TitleBarMessage.Text = "State cannot be removed";
                    titleBarMessageTimer.Start();
               }
               RemoveStateFlyout.Hide();
          }

          private void AddGroup_Click( object sender, RoutedEventArgs e )
          {
               if ( mainViewModel.ColorPaletteData.ElementGroups.Contains(comboElementGroups.Text) )
               {
                    TitleBarMessage.Text = "Group already exists";
                    titleBarMessageTimer.Start();
               }
               else
               {
                    mainViewModel.ColorPaletteData.ElementGroups.Add(comboElementGroups.Text);
               }
          }

          private void RemoveGroupConfirmation_Click( object sender, RoutedEventArgs e )
          {
               string groupToRemove = comboElementGroups.Text;
               AppConstants.ReturnCode returnCode = mainViewModel.RemoveGroup(groupToRemove);

               if ( returnCode == AppConstants.ReturnCode.Success )
               {
                    comboElementGroups.SelectedItem = comboElementGroups.Items.FirstOrDefault();

                    TitleBarMessage.Text = $"Removed Group: {groupToRemove}";
                    titleBarMessageTimer.Start();
               }
               else
               {
                    TitleBarMessage.Text = "Group cannot be removed";
                    titleBarMessageTimer.Start();
               }
               RemoveGroupFlyout.Hide();
          }

          // Button Click Event Handlers - Filters and Sorting
          private void ClearFilterButton_Click( object sender, RoutedEventArgs e )
          {
               comboElementStates.SelectedItem = defaultComboBoxText;
               comboElementGroups.SelectedItem = defaultComboBoxText;

               ApplyFilter();
          }

          private void RefreshFilterButton_Click( object sender, RoutedEventArgs e )
          {
               ApplyFilter();
          }

          private void ResetSortButtons()
          {
               FontIconSortElementIndex.Glyph = "\uE70D"; // Down Arrow - default glyph
               FontIconSortElementName.Glyph = "\uE70D";
               FontIconSortElementState.Glyph = "\uE70D";
               FontIconSortElementGroup.Glyph = "\uE70D";
               FontIconSortColor.Glyph = "\uE70D";
               FontIconSortNote.Glyph = "\uE70D";
          }

          private void ButtonSortElementIndex_Click( object sender, RoutedEventArgs e )
          {
               SortFilteredColorEntries(FontIconSortElementIndex, AppConstants.SortCriteria.Index);
          }

          private void ButtonSortElementName_Click( object sender, RoutedEventArgs e )
          {
               SortFilteredColorEntries(FontIconSortElementName, AppConstants.SortCriteria.Name);
          }

          private void ButtonSortElementState_Click( object sender, RoutedEventArgs e )
          {
               SortFilteredColorEntries(FontIconSortElementState, AppConstants.SortCriteria.State);
          }

          private void ButtonSortElementGroup_Click( object sender, RoutedEventArgs e )
          {
               SortFilteredColorEntries(FontIconSortElementGroup, AppConstants.SortCriteria.Group);
          }

          private void ButtonSortColor_Click( object sender, RoutedEventArgs e )
          {
               SortFilteredColorEntries(FontIconSortColor, AppConstants.SortCriteria.Color);
          }

          private void ButtonSortNote_Click( object sender, RoutedEventArgs e )
          {
               SortFilteredColorEntries(FontIconSortNote, AppConstants.SortCriteria.Note);
          }

          // Button Click Event Handlers - Color Selector Actions
          private void ColorSelectorButton_Click( object sender, RoutedEventArgs e )
          {
               StartColorSelector();
          }

          private void ColorSelectorSource_PointerPressed( object sender, RoutedEventArgs e )
          {
               ColorSelectorButton_Click(sender, e);
          }

          private async Task BrowseColorSelectorPhoto_Click( object sender, RoutedEventArgs e )
          {
               AppConstants.ReturnCode returnCode = await mainViewModel.SelectColorSelectorPhoto();

               LoadThumbNailImage(mainViewModel.ColorPaletteData.ColorSelectorSource);
          }

          private void ColorSelectorClearImage_Click( object sender, RoutedEventArgs e )
          {
               ColorSelectorImage.Source = mainViewModel.DefaultColorSelectorImage;
               App.ColorSelectorBitmap = mainViewModel.DefaultColorSelectorImage;
               mainViewModel.ColorPaletteData.ColorSelectorSource = string.Empty;
          }

          #endregion // Button Click Event Handlers

          private void ColorSelectorSource_DragOver( object sender, DragEventArgs e )
          {
               e.AcceptedOperation = DataPackageOperation.Copy;
          }

          private async void ColorSelectorSource_Drop( object sender, DragEventArgs e )
          {
               if ( e.DataView.Contains(StandardDataFormats.StorageItems) )
               {
                    var items = await e.DataView.GetStorageItemsAsync();
                    if ( items.Any() )
                    {
                         var storageFile = items[0] as StorageFile;
                         if ( FileService.IsImageFile(storageFile) )
                         {
                              // Load image into WriteableBitmap
                              LoadThumbNailImage(storageFile.Path);

                              TitleBarMessage.Text = $"Image dropped: {storageFile.Name}";
                              titleBarMessageTimer.Start();
                         }
                         else
                         {
                              TitleBarMessage.Text = "Invalid image / file type: .bmp .jpg .png only";
                              titleBarMessageTimer.Start();
                         }
                    }
               }
          }

          private async void LoadColorSelectorImage( string imagePath )
          {
               AppConstants.ReturnCode returnCode = await mainViewModel.ProcessColorSelectorImage_Async(imagePath);
               if ( App.ColorSelectorBitmap != null && returnCode == AppConstants.ReturnCode.Success )
               {
                    ColorSelectorImage.Source = App.ColorSelectorBitmap;
                    ColorSelectorImageUpdated();
               }
               else if ( returnCode == AppConstants.ReturnCode.FileNotFound )
               {
                    TitleBarMessage.Text = "Image File note found";
                    titleBarMessageTimer.Start();
               }
               else
               {
                    TitleBarMessage.Text = "Erro Processing Selected Image";
                    titleBarMessageTimer.Start();
               }
          }

          private void ColorSelectorImageUpdated()
          {
               SetColorSelectorImage?.Invoke(this, EventArgs.Empty);
          }

          #region // Combo Box Event Handlers

          // ComboBox Event Handlers
          private void ElementStateComboBox_Loaded( object sender, RoutedEventArgs e )
          {
               var comboBox = sender as ComboBox;
               if ( comboBox != null )
               {
                    comboBox.ItemsSource = mainViewModel.ColorPaletteData.ElementStates;
               }
          }

          private void ElementGroupComboBox_Loaded( object sender, RoutedEventArgs e )
          {
               var comboBox = sender as ComboBox;
               if ( comboBox != null )
               {
                    comboBox.ItemsSource = mainViewModel.ColorPaletteData.ElementGroups;
               }
          }

          private void OnFilterSelectionChanged( object sender, SelectionChangedEventArgs e )
          {
               if ( (ComboBox)sender == comboElementStates )
               {
                    SelectedState = comboElementStates.SelectedItem as string;
               }
               else if ( (ComboBox)sender == comboElementGroups )
               {
                    SelectedGroup = comboElementGroups.SelectedItem as string;
               }

               TitleBarMessage.Text = $"Filter Settings: {SelectedState} AND {SelectedGroup}";
               titleBarMessageTimer.Start();
          }

          #endregion // Combo Box Event Handlers

          // ScrollViewer Event Handlers
          private void LeftScrollViewerControl_ViewChanged( object sender, ScrollViewerViewChangedEventArgs e )
          {
               //TODO: Get the left scroll Viewer to scroll the left column
          }


          #region // Color Picker Event Handlers

          // ColorPicker Event Handlers
          private void CustomColorPicker_ColorChanged( ColorPicker sender, ColorChangedEventArgs args )
          {
               //Set Fields
               currentColorPickerHex = ColorConverter.ToHex(ColorConverter.ConvertColorToSysDrawColor(args.NewColor), includeAlpha: true);
               currentColorPickerHexNoAlpha = ColorConverter.ToHex(ColorConverter.ConvertColorToSysDrawColor(args.NewColor), includeAlpha: false);
               currentColorPickerRGB = $"{args.NewColor.A}, {args.NewColor.R}, {args.NewColor.G}, {args.NewColor.B}";
               //TODO: Settings will allow the desired snippet to be created and then use that to form the snippet with selected color
               currentColorPickerCodeSnippet = $"new SolidColorBrush(Color.FromArgb({args.NewColor.A}, {args.NewColor.R}, {args.NewColor.G}, {args.NewColor.B}));";

               // Set Controls
               TextBoxColorPickerHex.Text = currentColorPickerHex;
               TextBoxColorPickerHexNoAlpha.Text = currentColorPickerHexNoAlpha;
               TextBoxColorPickerRGB.Text = currentColorPickerRGB;
               TextBoxColorPickerCodeSnippet.Text = currentColorPickerCodeSnippet;
          }

          private void CopyColorPickerHex_Click( object sender, RoutedEventArgs e )
          {
               var dataPackage = new DataPackage();
               dataPackage.SetText(currentColorPickerHex);
               Clipboard.SetContent(dataPackage);

               TitleBarMessage.Text = "Copied Hex to Clipboard";
               titleBarMessageTimer.Start();
          }

          private void CopyColorPickerHexNoAlpha_Click( object sender, RoutedEventArgs e )
          {
               var dataPackage = new DataPackage();
               dataPackage.SetText(currentColorPickerHexNoAlpha);
               Clipboard.SetContent(dataPackage);

               TitleBarMessage.Text = "Copied Hex w/o Alpha to Clipboard";
               titleBarMessageTimer.Start();
          }

          private void CopyColorPickerRGB_Click( object sender, RoutedEventArgs e )
          {
               var dataPackage = new DataPackage();
               dataPackage.SetText(currentColorPickerRGB);
               Clipboard.SetContent(dataPackage);

               TitleBarMessage.Text = "Copied RGB to Clipboard";
               titleBarMessageTimer.Start();
          }

          private void CopyColorPickerCodeSnippet_Click( object sender, RoutedEventArgs e )
          {
               //TODO: Get from settings which language is selected
               //for now it is default C# code snippet
               //TODO: this is set in CustomColorPicker ColorChanged event handler

               var dataPackage = new DataPackage();
               dataPackage.SetText(currentColorPickerCodeSnippet);
               Clipboard.SetContent(dataPackage);

               TitleBarMessage.Text = "Copied Code Snippet to Clipboard";
               titleBarMessageTimer.Start();
          }

          #endregion // Color Picker Event Handlers

          // SettingsWindow Event Handlers
          private void SettingsWindow_Closed( object sender, WindowEventArgs e )
          {
               settingsWindow = null;
          }

          // ColorSelectorWindow Event Handlers
          private void ColorSelectorWindow_DataSelected( object sender, EventArgs e )
          {
               CustomColorPicker.Color = ColorConverter.ConvertColorToWinUIColor(App.colorSelectorColor);

          }

          private void ColorSelectorWindow_Closed( object sender, WindowEventArgs e )
          {
               colorSelectorWindow = null;

               CustomColorPicker.Color = ColorConverter.ConvertColorToWinUIColor(App.colorSelectorColor);

          }

          // Helper Methods
          private void ClearColorPaletteData()
          {
               ColorPaletteData.ColorPaletteFile = "New Palette";
               ColorPaletteData.ColorPaletteName = "New Palette";
               ColorPaletteData.ColorEntries.Clear();
               ColorPaletteData.FilteredColorEntries.Clear();

               ColorPaletteData.ElementStates.Clear();
               ColorPaletteData.ElementGroups.Clear();


               //Add back the default empty string for States and Group
               ColorPaletteData.ElementStates.Add(defaultComboBoxText);
               ColorPaletteData.ElementGroups.Add(defaultComboBoxText);

               SelectedState = defaultComboBoxText;
               SelectedGroup = defaultComboBoxText;

               comboElementGroups.SelectedItem = defaultComboBoxText;
               comboElementStates.SelectedItem = defaultComboBoxText;
          }

          private void ClearFilteredColorEntries()
          {
               ColorPaletteData.FilteredColorEntries.Clear();
          }

          private async Task<AppConstants.ReturnCode> SavePaletteToFile( string filePath )
          {
               StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
               if ( file != null )
               {
                    ColorPaletteData.ColorPaletteFile = file.Path;
                    await FileService.SavePaletteAsync(file.Path, ColorPaletteData);

                    localSettings.Values[AppConstants.LastOpenedFilePath] = file.Path;

                    ColorPaletteData.ColorPaletteName = file.DisplayName;

                    return AppConstants.ReturnCode.Success;
               }
               else
               {
                    return AppConstants.ReturnCode.GeneralFailure;
               }
          }

          private void ApplyFilter()
          {

               ClearFilteredColorEntries();

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

               ColorPaletteListView.ItemsSource = null;
               ColorPaletteListView.ItemsSource = ColorPaletteData.FilteredColorEntries;

          }

          private void SortFilteredColorEntries( FontIcon sortIcon, AppConstants.SortCriteria criteria )
          {
               ResetSortButtons();

               if ( activeSortCriteria == criteria )
               {
                    isAscending = !isAscending;
               }
               else
               {
                    activeSortCriteria = criteria;
                    isAscending = true;
               }

               sortIcon.Glyph = isAscending ? "\uE70E" : "\uE70D";


               var sortedEntries = new List<ColorEntry>(ColorPaletteData.FilteredColorEntries);

               switch ( activeSortCriteria )
               {
                    case SortCriteria.Index:
                    {
                         sortedEntries = isAscending ?
                              sortedEntries.OrderBy(entry => entry.ElementIndex).ToList() :
                              sortedEntries.OrderByDescending(entry => entry.ElementIndex).ToList();
                         break;
                    }
                    case SortCriteria.Name:
                    {
                         sortedEntries = isAscending ?
                              sortedEntries.OrderBy(entry => entry.ElementName).ToList() :
                              sortedEntries.OrderByDescending(entry => entry.ElementName).ToList();
                         break;
                    }
                    case SortCriteria.State:
                    {
                         sortedEntries = isAscending ?
                              sortedEntries.OrderBy(entry => entry.ElementState).ToList() :
                              sortedEntries.OrderByDescending(entry => entry.ElementState).ToList();
                         break;
                    }
                    case SortCriteria.Group:
                    {
                         sortedEntries = isAscending ?
                              sortedEntries.OrderBy(entry => entry.ElementGroup).ToList() :
                              sortedEntries.OrderByDescending(entry => entry.ElementGroup).ToList();
                         break;
                    }
                    case SortCriteria.Color:
                    {
                         sortedEntries = isAscending ?
                              sortedEntries.OrderBy(entry => entry.HexCode).ToList() :
                              sortedEntries.OrderByDescending(entry => entry.HexCode).ToList();
                         break;
                    }
                    case SortCriteria.Note:
                    {
                         sortedEntries = isAscending ?
                              sortedEntries.OrderBy(entry => entry.Note).ToList() :
                              sortedEntries.OrderByDescending(entry => entry.Note).ToList();
                         break;
                    }
               }

               //Clear and update existing
               ClearFilteredColorEntries();

               foreach ( var entry in sortedEntries )
               {
                    ColorPaletteData.FilteredColorEntries.Add(entry);
               }

               ColorPaletteListView.ItemsSource = null;
               ColorPaletteListView.ItemsSource = ColorPaletteData.FilteredColorEntries;

          }

          private void StartColorSelector()
          {
               if ( App.ColorSelectorBitmap == null )
               {
                    App.ColorSelectorBitmap = ColorSelectorProcessor.GetBitmap();
               }

               if ( colorSelectorWindow == null )
               {
                    colorSelectorWindow = new ColorSelectorWindow(this);
                    colorSelectorWindow.AppWindow.SetPresenter(AppWindowPresenterKind.Overlapped);

                    colorSelectorWindow.Closed += ColorSelectorWindow_Closed;
                    colorSelectorWindow.DataSelected += ColorSelectorWindow_DataSelected;
                    colorSelectorWindow.Activate();
               }
               else
               {
                    TitleBarMessage.Text = "Color Selector already open";
                    titleBarMessageTimer.Start();
               }
          }

     }
}
