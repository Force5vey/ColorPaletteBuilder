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
using System.ComponentModel;
using Windows.Devices.Bluetooth.Background;


namespace ColorPaletteBuilder
{
     public sealed partial class MainWindow :Window
     {
          // View Model
          private MainViewModel MainViewModel { get; set; }

          // Window and UI Elements
          private SettingsWindow settingsWindow;
          private ColorSelectorWindow colorSelectorWindow;

          // Constants
          private const int mainWindowMinWidth = 800;
          private const int mainWindowMinHeight = 625;

          private ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;


          // Events
          public event EventHandler SetColorSelectorImage;

          public delegate void ColorSelectorImageCleared_EventHandler( object sender, EventArgs e );
          public event ColorSelectorImageCleared_EventHandler ColorSelectorImageCleared;

          // Private Fields
          private string currentColorPickerHex = string.Empty;
          private string currentColorPickerHexNoAlpha = string.Empty;
          private string currentColorPickerRGB = string.Empty;
          private Windows.UI.Color currentColorPickerCodeSnippetColor = Windows.UI.Color.FromArgb(255, 1, 1, 1);

          // Sort and Filter Fields
          private AppConstants.SortCriteria activeSortCriteria = AppConstants.SortCriteria.Index;
          private bool isAscending = true;


          // Timers
          private readonly DispatcherTimer titleBarMessageTimer = new DispatcherTimer();
          private int messageTimerInterval = 3; // seconds

          private readonly DispatcherTimer autoSaveIndicatorTimer = new DispatcherTimer();
          private int autoSaveIndicatorInterval = 2;

          private readonly DispatcherTimer autoSaveTimer = new DispatcherTimer();

          private readonly DispatcherTimer backupSaveTimer = new DispatcherTimer();

          public MainWindow()
          {
               // Initialization
               this.InitializeComponent();
               MainViewModel = new MainViewModel(App.UserSettings);
               InitializeAsync();

               // Setting Default Values
               CustomColorPicker.Color = ColorConverter.ConvertColorToWinUIColor(Color.FromArgb(255, Color.White));

               // Event Handlers Setup
               comboElementStates.SelectionChanged += OnFilterSelectionChanged;
               comboElementGroups.SelectionChanged += OnFilterSelectionChanged;
               this.AppWindow.Changed += MainWindow_Changed;
               this.Closed += MainWindow_Closed;

               // Window Size Configuration
               ConfigureWindowSize();

               // Timers
               InitializeTimers();
               ConfigureTimers();

               //Recent List
               PopulateRecentFilesMenu();
               App.UserSettings.RecentFiles.CollectionChanged += RecentFiles_CollectionChanged;
          }

          private void PopulateRecentFilesMenu()
          {
               MenuFlyoutRecent.Items.Clear();

               foreach ( var filePath in App.UserSettings.RecentFiles )
               {
                    var menuItem = new MenuFlyoutItem
                    {
                         Text = filePath
                    };

                    menuItem.Click += ( sender, e ) => OpenRecentPalette(filePath);

                    MenuFlyoutRecent.Items.Add(menuItem);
               }

          }

          private void RecentFiles_CollectionChanged( object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e )
          {
               PopulateRecentFilesMenu();
          }

          private async void OpenRecentPalette( string filePath )
          {
               await MainViewModel.SavePaletteToFile_Async();
               await MainViewModel.LoadPalette_Async(filePath);
          }

          // Initialization Methods
          private async void InitializeAsync()
          {
               await MainViewModel.LoadDefaultColorSelectorImage();
               App.ColorSelectorBitmap = MainViewModel.DefaultColorSelectorImage;
               ColorSelectorImage.Source = App.ColorSelectorBitmap;

               // Data Binding Setup
               ColorPaletteListView.ItemsSource = MainViewModel.ColorPaletteData.FilteredColorEntries;
               ColorPaletteListView.DataContext = MainViewModel.ColorPaletteData.FilteredColorEntries;


               // Last Color Picker Color Used
               if ( localSettings.Values.TryGetValue(AppConstants.LastColorPickerHex, out object lastColorPickerHex) )
               {
                    currentColorPickerHex = lastColorPickerHex as string;
                    CustomColorPicker.Color = ColorConverter.ConvertColorToWinUIColor(ColorConverter.FromHex(currentColorPickerHex));
               }
               else
               {
                    currentColorPickerHex = "#FFFFFFFF";
                    CustomColorPicker.Color = ColorConverter.ConvertColorToWinUIColor(ColorConverter.FromHex(currentColorPickerHex));
               }

               if ( localSettings.Values.TryGetValue(AppConstants.LastOpenedFilePath, out var lastOpenedFilePath) )
               {
                    string path = lastOpenedFilePath as string;
                    if ( !string.IsNullOrEmpty(path) )
                    {
                         await MainViewModel.LoadPalette_Async(path);
                    }
               }

               SortListView(FontIconSortElementIndex, activeSortCriteria);

               LoadColorSelectorImage(MainViewModel.ColorPaletteData.ColorSelectorSource);

               comboElementStates.SelectedItem = MainViewModel.SelectedState;
               comboElementGroups.SelectedItem = MainViewModel.SelectedGroup;

               MainViewModel.ApplyFilter();
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

          private void InitializeTimers()
          {
               autoSaveTimer.Interval = TimeSpan.FromSeconds(App.UserSettings.AutoSaveInterval);
               autoSaveTimer.Tick += AutoSaveTimer_Tick;
               if ( App.UserSettings.AutoSave )
               {
                    autoSaveTimer.Start();
               }

               backupSaveTimer.Interval = TimeSpan.FromSeconds(App.UserSettings.BackupSaveInterval);
               backupSaveTimer.Tick += AutoSaveBackupTimer_Tick;
               if ( App.UserSettings.BackupSave )
               {
                    backupSaveTimer.Start();
               }

               titleBarMessageTimer.Interval = TimeSpan.FromSeconds(messageTimerInterval);
               titleBarMessageTimer.Tick += TitleMessageTimer_Tick;

               autoSaveIndicatorTimer.Interval = TimeSpan.FromSeconds(autoSaveIndicatorInterval);
               autoSaveIndicatorTimer.Tick += AutoSaveProgressRingTimer_Tick;
          }

          private void ConfigureTimers()
          {
               if ( App.UserSettings.AutoSave )
               {
                    autoSaveTimer.Interval = TimeSpan.FromSeconds(App.UserSettings.AutoSaveInterval);
                    autoSaveTimer.Start();
               }
               else
               {
                    autoSaveTimer.Stop();
               }

               if ( App.UserSettings.BackupSave )
               {
                    backupSaveTimer.Interval = TimeSpan.FromSeconds(App.UserSettings.BackupSaveInterval);
                    backupSaveTimer.Start();
               }
               else
               {
                    backupSaveTimer.Stop();
               }
          }

          private void AutoSaveProgressRingTimer_Tick( object sender, object e )
          {
               AutoSaveIndicator.IsActive = false;
               autoSaveIndicatorTimer.Stop();
          }

          private void TitleMessageTimer_Tick( object sender, object e )
          {
               TitleBarMessage.Text = "";
               titleBarMessageTimer.Stop();
          }

          private async void AutoSaveTimer_Tick( object sender, object e )
          {
               AppConstants.ReturnCode returnCode;
               returnCode = await MainViewModel.SavePaletteToFile_Async();

               if ( TitleBarMessage != null || titleBarMessageTimer != null )
               {
                    switch ( returnCode )
                    {
                         case AppConstants.ReturnCode.Success:
                         AutoSaveIndicator.IsActive = true;
                         autoSaveIndicatorTimer.Start();
                         break;
                         case AppConstants.ReturnCode.GeneralFailure:
                         TitleBarMessage.Text = "Auto-Save Failed";
                         titleBarMessageTimer.Start();
                         break;

                         case AppConstants.ReturnCode.FileNotFound:
                         TitleBarMessage.Text = "Save File to Enable Auto-Save";
                         titleBarMessageTimer.Start();
                         break;
                    }
               }
          }

          private async void AutoSaveBackupTimer_Tick( object sender, object e )
          {
               AppConstants.ReturnCode returnCode;
               returnCode = await MainViewModel.AutoSaveBackup_Async();

               if ( TitleBarMessage != null || titleBarMessageTimer != null )
               {
                    switch ( returnCode )
                    {
                         case AppConstants.ReturnCode.Success:
                         AutoSaveIndicator.IsActive = true;
                         autoSaveIndicatorTimer.Start();
                         break;
                         case AppConstants.ReturnCode.GeneralFailure:
                         TitleBarMessage.Text = "Backup-Save Failed";
                         titleBarMessageTimer.Start();
                         break;
                    }
               }
          }

          #endregion //End Timer Event Handlers

          #region // Button Click Event Handlers - Color Palette Actions

          private void NewPaletteButton_Click( object sender, RoutedEventArgs e )
          {
               MainViewModel.ClearColorPaletteData(false);

               comboElementStates.SelectedItem = MainViewModel.SelectedState;
               comboElementGroups.SelectedItem = MainViewModel.SelectedGroup;
          }

          private async void OpenPalette_Click( object sender, RoutedEventArgs e )
          {
               AppConstants.ReturnCode returnCode = await MainViewModel.OpenPalette_Async(this);

               if ( returnCode == AppConstants.ReturnCode.Success )
               {
                    TitleBarMessage.Text = $"Opened Color Palette: {MainViewModel.ColorPaletteData.FileName}";
                    titleBarMessageTimer.Start();
               }
          }

          private async void SavePalette_Click( object sender, RoutedEventArgs e )
          {
               if ( MainViewModel.ColorPaletteData.IsSaved )
               {
                    await MainViewModel.SavePaletteToFile_Async();
               }
               else
               {
                    SavePaletteAs_Click(sender, e);
               }
          }

          private async void SavePaletteAs_Click( object sender, RoutedEventArgs e )
          {
               var returnCode = await MainViewModel.SavePaletteAs_Async(this);
               if ( returnCode == AppConstants.ReturnCode.Success )
               {
                    TitleBarMessage.Text = "File Saved Succesfully";
                    titleBarMessageTimer.Start();
               }
          }


          // Button Click Event Handlers - Color Entries Management
          private void AddColorEntry_Click( object sender, RoutedEventArgs e )
          {
               ColorEntry newEntry = new ColorEntry
               {
                    ElementIndex = MainViewModel.ColorPaletteData.HighestEntryIndex++,
                    ElementName = "Name",
                    ElementGroup = MainViewModel.ColorPaletteData.ElementGroups.FirstOrDefault(),
                    ElementState = MainViewModel.ColorPaletteData.ElementStates.FirstOrDefault(),
                    HexCode = currentColorPickerHex
               };

               // TODO: Make user setting to add to top (0) or end (list Count)
               // add settingsValue to appConstants. viewModel method for loading all user settings. pull a value from mainViewModel UserSettings
               // Make a model UserSettings. this can get populated in MainViewModel on initialization us that value below to set insert location.
               // The setting will just be on top or bottom, this will need logic to get the last index then plus 1
               MainViewModel.ColorPaletteData.ColorEntries.Insert(0, newEntry);

               MainViewModel.ApplyFilter();
               ColorPaletteListView.ItemsSource = null;
               ColorPaletteListView.ItemsSource = MainViewModel.ColorPaletteData.FilteredColorEntries;

          }

          private void RemoveColorEntry_Click( object sender, RoutedEventArgs e )
          {
               var selectedEntry = ColorPaletteListView.SelectedItem as ColorEntry;
               MainViewModel.RemoveColorEntry(selectedEntry);

               MainViewModel.ApplyFilter();
               ColorPaletteListView.ItemsSource = null;
               ColorPaletteListView.ItemsSource = MainViewModel.ColorPaletteData.FilteredColorEntries;
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
                    MainViewModel.CopyToClipbard(colorEntry.HexCode);

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
               if ( MainViewModel.ColorPaletteData.ElementStates.Contains(comboElementStates.Text) )
               {
                    TitleBarMessage.Text = "State already exists";
                    titleBarMessageTimer.Start();
               }
               else
               {
                    MainViewModel.ColorPaletteData.ElementStates.Add(comboElementStates.Text);
               }
          }

          private void RemoveStateConfirmation_Click( object sender, RoutedEventArgs e )
          {
               string stateToRemove = comboElementStates.Text;
               AppConstants.ReturnCode returnCode = MainViewModel.RemoveState(stateToRemove);

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
               if ( MainViewModel.ColorPaletteData.ElementGroups.Contains(comboElementGroups.Text) )
               {
                    TitleBarMessage.Text = "Group already exists";
                    titleBarMessageTimer.Start();
               }
               else
               {
                    MainViewModel.ColorPaletteData.ElementGroups.Add(comboElementGroups.Text);
               }
          }

          private void RemoveGroupConfirmation_Click( object sender, RoutedEventArgs e )
          {
               string groupToRemove = comboElementGroups.Text;
               AppConstants.ReturnCode returnCode = MainViewModel.RemoveGroup(groupToRemove);

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
               comboElementStates.SelectedItem = AppConstants.DefaultComboBoxText;
               comboElementGroups.SelectedItem = AppConstants.DefaultComboBoxText;

               MainViewModel.ApplyFilter();
               ColorPaletteListView.ItemsSource = null;
               ColorPaletteListView.ItemsSource = MainViewModel.ColorPaletteData.FilteredColorEntries;
          }

          private void RefreshFilterButton_Click( object sender, RoutedEventArgs e )
          {
               MainViewModel.ApplyFilter();
               ColorPaletteListView.ItemsSource = null;
               ColorPaletteListView.ItemsSource = MainViewModel.ColorPaletteData.FilteredColorEntries;
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
               SortListView(FontIconSortElementIndex, AppConstants.SortCriteria.Index);
          }

          private void ButtonSortElementName_Click( object sender, RoutedEventArgs e )
          {
               SortListView(FontIconSortElementName, AppConstants.SortCriteria.Name);
          }

          private void ButtonSortElementState_Click( object sender, RoutedEventArgs e )
          {
               SortListView(FontIconSortElementState, AppConstants.SortCriteria.State);
          }

          private void ButtonSortElementGroup_Click( object sender, RoutedEventArgs e )
          {
               SortListView(FontIconSortElementGroup, AppConstants.SortCriteria.Group);
          }

          private void ButtonSortColor_Click( object sender, RoutedEventArgs e )
          {
               SortListView(FontIconSortColor, AppConstants.SortCriteria.Color);
          }

          private void ButtonSortNote_Click( object sender, RoutedEventArgs e )
          {
               SortListView(FontIconSortNote, AppConstants.SortCriteria.Note);
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
               AppConstants.ReturnCode returnCode = await MainViewModel.SelectColorSelectorPhoto(this);

               LoadColorSelectorImage(MainViewModel.ColorPaletteData.ColorSelectorSource);
          }



          private void ColorSelectorClearImage_Click( object sender, RoutedEventArgs e )
          {
               ColorSelectorImage.Source = MainViewModel.DefaultColorSelectorImage;
               App.ColorSelectorBitmap = MainViewModel.DefaultColorSelectorImage;
               MainViewModel.ColorPaletteData.ColorSelectorSource = string.Empty;
               OnColorSelectorImageCleared(EventArgs.Empty);
          }

          private void OnColorSelectorImageCleared( EventArgs e )
          {
               ColorSelectorImageCleared?.Invoke(this, e);
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
                              LoadColorSelectorImage(storageFile.Path);

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
               AppConstants.ReturnCode returnCode = await MainViewModel.ProcessColorSelectorImage_Async(imagePath);
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
                    comboBox.ItemsSource = MainViewModel.ColorPaletteData.ElementStates;
               }
          }

          private void ElementGroupComboBox_Loaded( object sender, RoutedEventArgs e )
          {
               var comboBox = sender as ComboBox;
               if ( comboBox != null )
               {
                    comboBox.ItemsSource = MainViewModel.ColorPaletteData.ElementGroups;
               }
          }

          private void OnFilterSelectionChanged( object sender, SelectionChangedEventArgs e )
          {
               if ( (ComboBox)sender == comboElementStates )
               {
                    MainViewModel.SelectedState = comboElementStates.SelectedItem as string;
               }
               else if ( (ComboBox)sender == comboElementGroups )
               {
                    MainViewModel.SelectedGroup = comboElementGroups.SelectedItem as string;
               }

               TitleBarMessage.Text = $"Filter Settings: {MainViewModel.SelectedState} AND {MainViewModel.SelectedGroup}";
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
               currentColorPickerCodeSnippetColor = args.NewColor;

               // Set Controls
               TextBoxColorPickerHex.Text = currentColorPickerHex;
               TextBoxColorPickerHexNoAlpha.Text = currentColorPickerHexNoAlpha;
               TextBoxColorPickerRGB.Text = currentColorPickerRGB;

               string snippetTemplate = GetSnippetForCurrentLanguage(App.UserSettings.SnippetLanguage);
               string updatedSnippet = InterpolateUserSnippet(snippetTemplate, currentColorPickerCodeSnippetColor);

               TextBoxColorPickerCodeSnippet.Text = updatedSnippet;
          }

          private string InterpolateUserSnippet( string template, Windows.UI.Color color )
          {
               return template
                    .Replace("$a", color.A.ToString())
                    .Replace("$r", color.R.ToString())
                    .Replace("$g", color.G.ToString())
                    .Replace("$b", color.B.ToString());
          }

          private string GetSnippetForCurrentLanguage( AppConstants.SnippetLanguage language )
          {
               return language switch
               {
                    AppConstants.SnippetLanguage.CSharp => App.UserSettings.SnippetCSharp,
                    AppConstants.SnippetLanguage.Javascript => App.UserSettings.SnippetJavascript,
                    AppConstants.SnippetLanguage.Python => App.UserSettings.SnippetPython,
                    _ => App.UserSettings.SnippetCustom,
               };
          }

          private void CopyColorPickerHex_Click( object sender, RoutedEventArgs e )
          {
               MainViewModel.CopyToClipbard(currentColorPickerHex);

               TitleBarMessage.Text = "Copied Hex to Clipboard";
               titleBarMessageTimer.Start();
          }

          private void CopyColorPickerHexNoAlpha_Click( object sender, RoutedEventArgs e )
          {
               MainViewModel.CopyToClipbard(currentColorPickerHexNoAlpha);

               TitleBarMessage.Text = "Copied Hex w/o Alpha to Clipboard";
               titleBarMessageTimer.Start();
          }

          private void CopyColorPickerRGB_Click( object sender, RoutedEventArgs e )
          {
               MainViewModel.CopyToClipbard(currentColorPickerRGB);

               TitleBarMessage.Text = "Copied RGB to Clipboard";
               titleBarMessageTimer.Start();
          }

          private void CopyColorPickerCodeSnippet_Click( object sender, RoutedEventArgs e )
          {
               MainViewModel.CopyToClipbard(TextBoxColorPickerCodeSnippet.Text);

               TitleBarMessage.Text = "Copied Code Snippet to Clipboard";
               titleBarMessageTimer.Start();
          }

          #endregion // Color Picker Event Handlers

          // SettingsWindow Event Handlers
          private void SettingsWindow_Closed( object sender, WindowEventArgs e )
          {
               settingsWindow = null;
               ConfigureTimers();
               MainViewModel.ApplyColorEntrySettings();
          }

          // ColorSelectorWindow Event Handlers
          private void ColorSelectorWindow_DataSelected( object sender, EventArgs e )
          {
               CustomColorPicker.Color = ColorConverter.ConvertColorToWinUIColor(App.ColorSelectorColor);

          }

          private void ColorSelectorWindow_Closed( object sender, WindowEventArgs e )
          {
               colorSelectorWindow = null;
          }

          // Helper Methods
          private void SortListView( FontIcon sortIcon, AppConstants.SortCriteria criteria )
          {
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

               MainViewModel.SortFilteredColorEntries(criteria, isAscending);

               ColorPaletteListView.ItemsSource = null;
               ColorPaletteListView.ItemsSource = MainViewModel.ColorPaletteData.FilteredColorEntries;
          }

          private void StartColorSelector()
          {
               if ( App.ColorSelectorBitmap == null )
               {
                    App.ColorSelectorBitmap = MainViewModel.DefaultColorSelectorImage;
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

          private void AutoSaveIndicator_PointerPressed( object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e )
          {

          }
     }
}
