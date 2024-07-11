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
          private enum SortCriteria
          {
               Index,
               Name,
               State,
               Group,
               Color,
               Note
          }

          // Events
          public event EventHandler SetColorSelectorImage;

          // Public Properties
          public ColorPalette ColorPaletteData { get; set; } = new ColorPalette();
          public string SelectedState { get; set; }
          public string SelectedGroup { get; set; }

          public static WriteableBitmap DefaultColorSelectorImage { get; private set; }

          // Private Fields
          private string currentColorPickerHex = "#FFFFFFFF";
          private string currentColorPickerHexNoAlpha = "#FFFFFF";
          private string currentColorPickerRGB = "";
          private string currentColorPickerCodeSnippet = "";
          private int currentElementIndex = 0;

          // Sort and Filter Fields
          private string defaultComboBoxText = "Any";
          private SortCriteria activeSortCriteria = SortCriteria.Index;
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
               ColorPaletteData = new ColorPalette();

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
               // a 'blank' default image loaded from assets to be a place holder in the color selector image
               // also serves as a sizing mechanism to allow for drag and drop over the image control
               DefaultColorSelectorImage = await FileService.LoadDefaultColorSelectorImage();
               ColorSelectorImage.Source = DefaultColorSelectorImage;
               App.ColorSelectorBitmap = DefaultColorSelectorImage;

               // Data Binding Setup
               ColorPaletteListView.ItemsSource = ColorPaletteData.FilteredColorEntries;

               // Load last used values from previous session
               await LoadLastSession();

               //Do a default Sort by Index
               SortFilteredColorEntries(FontIconSortElementIndex, activeSortCriteria);
          }

          private void ConfigureWindowSize()
          {
               var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
               try
               {
                    if ( localSettings.Values.TryGetValue("WindowWidth", out object width) &&
                         localSettings.Values.TryGetValue("WindowHeight", out object height) &&
                         localSettings.Values.TryGetValue("WindowLeft", out object left) &&
                         localSettings.Values.TryGetValue("WindowTop", out object top) )
                    {
                         this.AppWindow.Resize(new Windows.Graphics.SizeInt32 { Width = (int)width, Height = (int)height });
                         this.AppWindow.Move(new Windows.Graphics.PointInt32 { X = (int)left, Y = (int)top });
                    }
               }
               catch
               {
                    // Just assign a default size if there is an error.
                    this.AppWindow.Resize(new Windows.Graphics.SizeInt32(mainWindowMinWidth, mainWindowMinHeight));
               }
          }

          /// <summary>
          /// Get's Designated Palette Loaded from file and loads ColorPaletteData
          /// </summary>
          /// <param name="paletteFilePath"></param>
          /// <returns></returns>
          private async Task LoadPalette_Async( string paletteFilePath )
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

                         TitleBarFileName.Text = ColorPaletteData.ColorPaletteName;

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

                         currentElementIndex = ColorPaletteData.ColorEntries.Max(entry => entry.ElementIndex) + 1;

                         // force a rebind of the ListView to ensure it updates
                         ColorPaletteListView.ItemsSource = null;
                         ApplyFilter();
                         ColorPaletteListView.ItemsSource = ColorPaletteData.FilteredColorEntries;


                    }
                    else
                    {
                         //TODO: hand null loaded colorpalette
                    }
               }
               else
               {
                    //TODO: Display error and load default palette
                    // this is null or empty.

               }
          }

          private async Task LoadLastSession()
          {
               // Load last palette
               // load last thumbnail.
               // load last color picker hex


               var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

               if ( localSettings.Values.TryGetValue(AppConstants.LastOpenedFilePath, out var lastOpenedFilePath) )
               {
                    if ( lastOpenedFilePath != null )
                    {
                         await LoadPalette_Async(lastOpenedFilePath as string);
                    }
               }

               if ( localSettings.Values.TryGetValue(AppConstants.LastColorPickerHex, out object lastColorPickerHex) )
               {
                    currentColorPickerHex = lastColorPickerHex as string;
                    CustomColorPicker.Color = ColorConverter.ConvertColorToWinUIColor(ColorConverter.FromHex(currentColorPickerHex));
               }

               LoadThumbNailImage(ColorPaletteData.ColorSelectorSource);

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
               var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
               localSettings.Values["WindowWidth"] = this.AppWindow.Size.Width;
               localSettings.Values["WindowHeight"] = this.AppWindow.Size.Height;
               localSettings.Values["WindowLeft"] = this.AppWindow.Position.X;
               localSettings.Values["WindowTop"] = this.AppWindow.Position.Y;

               localSettings.Values["LastColorPickerHex"] = currentColorPickerHex;

               if ( settingsWindow != null )
               {
                    settingsWindow.Close();
               }
               if ( colorSelectorWindow != null )
               {
                    colorSelectorWindow.Close();
               }
          }

          // Timer Event Handlers
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
               returnCode = await SavePaletteToFile(ColorPaletteData.ColorPaletteFile);

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
               returnCode = await mainViewModel.AutoSaveBackup_Async(ColorPaletteData);

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

          // Button Click Event Handlers - Color Palette Actions

          private void NewPalette_Click( object sender, RoutedEventArgs e )
          {
               //TODO: Reset Filename etc to default.
               ClearColorPaletteData();
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
                    await LoadPalette_Async(file.Path);

                    var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                    localSettings.Values[AppConstants.LastOpenedFilePath] = file.Path;

               }
          }

          private async void SavePalette_Click( object sender, RoutedEventArgs e )
          {
               if ( string.IsNullOrEmpty(ColorPaletteData.ColorPaletteFile) || ColorPaletteData.ColorPaletteFile == "New Palette" )
               {
                    SavePaletteAs_Click(sender, e);
                    return;
               }

               await SavePaletteToFile(ColorPaletteData.ColorPaletteFile);
          }

          private async void SavePaletteAs_Click( object sender, RoutedEventArgs e )
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
                    await SavePaletteToFile(file.Path);
               }
          }


          // Button Click Event Handlers - Color Entries Management
          private void AddColorEntry_Click( object sender, RoutedEventArgs e )
          {
               ColorEntry newEntry = new ColorEntry
               {
                    ElementIndex = currentElementIndex++,
                    ElementName = "Name",
                    ElementGroup = ColorPaletteData.ElementGroups.FirstOrDefault(),
                    ElementState = ColorPaletteData.ElementStates.FirstOrDefault(),
                    HexCode = currentColorPickerHex
               };

               // TODO: Make user setting to add to top (0) or end (list Count)
               ColorPaletteData.ColorEntries.Insert(0, newEntry);

               ApplyFilter();

          }

          private void RemoveColorEntry_Click( object sender, RoutedEventArgs e )
          {
               var selectedEntry = ColorPaletteListView.SelectedItem as ColorEntry;
               if ( selectedEntry != null )
               {
                    ColorPaletteData.ColorEntries.Remove(selectedEntry);
               }

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
               if ( ColorPaletteData.ElementStates.Contains(comboElementStates.Text) )
               {
                    TitleBarMessage.Text = "State already exists";
                    titleBarMessageTimer.Start();
               }
               else
               {
                    ColorPaletteData.ElementStates.Add(comboElementStates.Text);
               }

          }

          private void RemoveStateConfirmation_Click( object sender, RoutedEventArgs e )
          {
               string stateToRemove = comboElementStates.Text;

               // Set each ColorEntry with the state to remove to an empty string
               foreach ( var colorEntry in ColorPaletteData.ColorEntries )
               {
                    if ( colorEntry.ElementState == stateToRemove )
                    {
                         colorEntry.ElementState = string.Empty;
                    }
               }

               // Now Remove from the ElementStates collection
               // Check if it is an empty string, for this cannot be removed
               if ( stateToRemove != defaultComboBoxText )
               {
                    ColorPaletteData.ElementStates.Remove(stateToRemove);
               }
               else // Give a message to know it worked but just can't be removed
               {
                    TitleBarMessage.Text = "State cannot be removed";
                    titleBarMessageTimer.Start();
               }

               comboElementStates.SelectedItem = comboElementStates.Items.FirstOrDefault();

               TitleBarMessage.Text = $"Removed State: {stateToRemove}";
               titleBarMessageTimer.Start();

               RemoveStateFlyout.Hide();
          }

          private void AddGroup_Click( object sender, RoutedEventArgs e )
          {
               if ( ColorPaletteData.ElementGroups.Contains(comboElementGroups.Text) )
               {
                    TitleBarMessage.Text = "Group already exists";
                    titleBarMessageTimer.Start();
               }
               else
               {
                    ColorPaletteData.ElementGroups.Add(comboElementGroups.Text);
               }
          }

          private void RemoveGroupConfirmation_Click( object sender, RoutedEventArgs e )
          {
               string groupToRemove = comboElementGroups.Text;

               // Set each ColorEntry with the group to remove to an empty string
               foreach ( var colorEntry in ColorPaletteData.ColorEntries )
               {
                    if ( colorEntry.ElementGroup == groupToRemove )
                    {
                         colorEntry.ElementGroup = string.Empty;
                    }
               }

               // Now Remove from the ElementGroups collection
               // Check if it is an empty string, for this cannot be removed
               if ( groupToRemove != defaultComboBoxText )
               {
                    ColorPaletteData.ElementGroups.Remove(groupToRemove);
               }
               else
               {
                    TitleBarMessage.Text = "Group cannot be removed";
                    titleBarMessageTimer.Start();
               }

               comboElementGroups.SelectedItem = comboElementGroups.Items.FirstOrDefault();

               TitleBarMessage.Text = $"Removed Group: {groupToRemove}";
               titleBarMessageTimer.Start();

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
               SortFilteredColorEntries(FontIconSortElementIndex, SortCriteria.Index);
          }

          private void ButtonSortElementName_Click( object sender, RoutedEventArgs e )
          {
               SortFilteredColorEntries(FontIconSortElementName, SortCriteria.Name);
          }

          private void ButtonSortElementState_Click( object sender, RoutedEventArgs e )
          {
               SortFilteredColorEntries(FontIconSortElementState, SortCriteria.State);
          }

          private void ButtonSortElementGroup_Click( object sender, RoutedEventArgs e )
          {
               SortFilteredColorEntries(FontIconSortElementGroup, SortCriteria.Group);
          }

          private void ButtonSortColor_Click( object sender, RoutedEventArgs e )
          {
               SortFilteredColorEntries(FontIconSortColor, SortCriteria.Color);
          }

          private void ButtonSortNote_Click( object sender, RoutedEventArgs e )
          {
               SortFilteredColorEntries(FontIconSortNote, SortCriteria.Note);
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

          private async void BrowseColorSelectorPhoto_Click( object sender, RoutedEventArgs e )
          {
               // Initialize the picker
               FileOpenPicker picker = new FileOpenPicker
               {
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary // More appropriate start location for images
               };

               // Initialize with window handle
               var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
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
                    // Store the path in local settings
                    var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                    localSettings.Values["LastOpenedFilePath"] = file.Path;

                    LoadThumbNailImage(file.Path);

                    ColorPaletteData.ColorSelectorSource = file.Path;
               }
          }

          private void ColorSelectorClearImage_Click( object sender, RoutedEventArgs e )
          {
               ColorSelectorImage.Source = DefaultColorSelectorImage;
               App.ColorSelectorBitmap = DefaultColorSelectorImage;
               ColorPaletteData.ColorSelectorSource = string.Empty;
          }

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
                         if ( IsImageFile(storageFile) )
                         {
                              // Load image into WriteableBitmap
                              LoadThumbNailImage(storageFile.Path);

                              TitleBarMessage.Text = $"Image dropped: {storageFile.Name}";
                              titleBarMessageTimer.Start();
                         }
                         else
                         {
                              TitleBarMessage.Text = "Invalid image / file type";
                              titleBarMessageTimer.Start();
                         }
                    }
               }
          }

          private async void LoadThumbNailImage( string imagePath )
          {
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
                              ColorSelectorImage.Source = App.ColorSelectorBitmap;

                              ColorSelectorImageUpdated();
                         }
                    }
                    else
                    {
                         Debug.WriteLine($"File does not exist: {imagePath}");

                         App.ColorSelectorBitmap = DefaultColorSelectorImage;
                         ColorPaletteData.ColorSelectorSource = string.Empty;
                         ColorSelectorImage.Source = App.ColorSelectorBitmap;
                    }
               }
               catch ( Exception ex )
               {
                    Debug.WriteLine($"Error loading thumbnail image: {ex.Message}");
                    Debug.WriteLine($"StackTrace: {ex.StackTrace}");
               }
          }

          private void ColorSelectorImageUpdated()
          {
               SetColorSelectorImage?.Invoke(this, EventArgs.Empty);
          }

          private async void ColorSelectorSource_Paste( object sender, TextControlPasteEventArgs e )
          {
               e.Handled = true; // Prevent the default paste behavior

               var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
               try
               {
                    if ( dataPackageView.Contains(StandardDataFormats.Bitmap) )
                    {
                         var bitmap = await dataPackageView.GetBitmapAsync();
                         if ( bitmap != null )
                         {
                              using ( var stream = await bitmap.OpenReadAsync() )
                              {
                                   // Ensure the stream is at the beginning
                                   stream.Seek(0);

                                   // Decode the bitmap
                                   var decoder = await BitmapDecoder.CreateAsync(stream);
                                   var writeableBitmap = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                                   await writeableBitmap.SetSourceAsync(stream);

                                   // Assign the WriteableBitmap to the static variable
                                   App.ColorSelectorBitmap = writeableBitmap;

                                   // Update the UI
                                   TitleBarMessage.Text = "Image pasted from clipboard.";
                                   titleBarMessageTimer.Start();
                              }
                         }
                         else
                         {
                              TitleBarMessage.Text = "Failed to retrieve bitmap from clipboard.";
                         }
                    }
                    else
                    {
                         TitleBarMessage.Text = "Clipboard does not contain a bitmap.";
                    }
               }
               catch ( Exception ex )
               {
                    TitleBarMessage.Text = $"An error occurred: {ex.Message}";
               }
          }

          // ComboBox Event Handlers
          private void ElementStateComboBox_Loaded( object sender, RoutedEventArgs e )
          {
               var comboBox = sender as ComboBox;
               if ( comboBox != null )
               {
                    comboBox.ItemsSource = ColorPaletteData.ElementStates;
               }
          }

          private void ElementGroupComboBox_Loaded( object sender, RoutedEventArgs e )
          {
               var comboBox = sender as ComboBox;
               if ( comboBox != null )
               {
                    comboBox.ItemsSource = ColorPaletteData.ElementGroups;
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

               TitleBarMessage.Text = $"Filter: {SelectedState} - {SelectedGroup}";
               titleBarMessageTimer.Start();

          }

          // ScrollViewer Event Handlers
          private void LeftScrollViewerControl_ViewChanged( object sender, ScrollViewerViewChangedEventArgs e )
          {
          }

          // ColorPicker Event Handlers
          private void CustomColorPicker_ColorChanged( ColorPicker sender, ColorChangedEventArgs args )
          {
               if ( currentColorPickerHex != null )
               {
                    currentColorPickerHex = ColorConverter.ToHex(ColorConverter.ConvertColorToSysDrawColor(args.NewColor), includeAlpha: true);
               }
               if ( currentColorPickerHexNoAlpha != null )
               {
                    currentColorPickerHexNoAlpha = ColorConverter.ToHex(ColorConverter.ConvertColorToSysDrawColor(args.NewColor), includeAlpha: false);
               }
               if ( currentColorPickerRGB != null )
               {
                    currentColorPickerRGB = $"{args.NewColor.A}, {args.NewColor.R}, {args.NewColor.G}, {args.NewColor.B}";
               }
               if ( currentColorPickerCodeSnippet != null )
               {
                    //TODO: Settings will allow the desired snippet to be created and then use that to form the snippet with selected color
                    currentColorPickerCodeSnippet = $"new SolidColorBrush(Color.FromArgb({args.NewColor.A}, {args.NewColor.R}, {args.NewColor.G}, {args.NewColor.B}));";
               }

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

                    var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                    localSettings.Values["LastOpenedFilePath"] = file.Path;

                    ColorPaletteData.ColorPaletteName = file.DisplayName;

                    return AppConstants.ReturnCode.Success;
               }
               else
               {
                    return AppConstants.ReturnCode.GeneralFailure;
               }
          }
          private bool IsImageFile( StorageFile file )
          {
               var imageFileTypes = new[] { ".bmp", ".jpg", ".jpeg", ".png" };
               return imageFileTypes.Contains(file.FileType.ToLower());
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

          private void SortFilteredColorEntries( FontIcon sortIcon, SortCriteria criteria )
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
