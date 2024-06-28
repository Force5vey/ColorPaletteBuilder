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



// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ColorPaletteBuilder
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        // Constants
        private const int mainWindowMinWidth = 800;
        private const int mainWindowMinHeight = 625;

        // Public Properties
        public ColorPalette ColorPaletteData { get; set; } = new ColorPalette();
        public string SelectedState { get; set; }
        public string SelectedGroup { get; set; }

        // Private Fields
        private string currentColorPickerHex = "#FFFFFFFF";
        private string defaultComboBoxText = "Any";

        // Timers and Miscellaneous
        private DispatcherTimer titleMessageTimer = new DispatcherTimer();

        public MainWindow()
        {
            // Initialization
            this.InitializeComponent();
            ColorPaletteData = new ColorPalette();

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

            // Data Binding Setup
            ColorPaletteListView.ItemsSource = ColorPaletteData.FilteredColorEntries;

            // Loading Last Session State
            LoadLastOpenedFile();

            // Miscellaneous
            titleMessageTimer.Interval = TimeSpan.FromSeconds(2);
            titleMessageTimer.Tick += TitleMessageTimer_Tick;
        }


        private void ConfigureWindowSize()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            try
            {
                if (localSettings.Values.TryGetValue("WindowWidth", out object width) && localSettings.Values.TryGetValue("WindowHeight", out object height))
                {
                    this.AppWindow.Resize(new Windows.Graphics.SizeInt32((int)width, (int)height));
                }
            }
            catch
            {
                // Just assign a default size if there is an error.
                this.AppWindow.Resize(new Windows.Graphics.SizeInt32(mainWindowMinWidth, mainWindowMinHeight));
            }
        }

        private void MainWindow_Changed(Microsoft.UI.Windowing.AppWindow sender, object args)
        {

            if (sender.Size.Width < mainWindowMinWidth)
            {
                this.AppWindow.Resize(new Windows.Graphics.SizeInt32(mainWindowMinWidth, (int)sender.Size.Height));
            }
            if (sender.Size.Height < mainWindowMinHeight)
            {
                this.AppWindow.Resize(new Windows.Graphics.SizeInt32((int)sender.Size.Width, mainWindowMinHeight));
            }

            ColorSelectorSource.Text = $"Width: {sender.Size.Width} Height: {sender.Size.Height}";
        }


        #region Helper Methods


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


        private void TitleMessageTimer_Tick(object sender, object e)
        {
            TitleBarMessage.Text = "";
            titleMessageTimer.Stop();
        }

        private void LoadLastOpenedFile()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.TryGetValue("LastOpenedFilePath", out object filePath))
            {
                string lastOpenedFilePath = filePath as string;
                if (!string.IsNullOrEmpty(lastOpenedFilePath))
                {
                    LoadPaletteFromFile(lastOpenedFilePath);
                }
            }
        }


        #endregion



        private void CopyHexCode_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.DataContext is ColorEntry colorEntry)
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(colorEntry.HexCode);
                Clipboard.SetContent(dataPackage);

                TitleBarMessage.Text = "Copied to Clipboard";
                titleMessageTimer.Start();
            }
        }


        #region File Operations


        private void NewPalette_Click(object sender, RoutedEventArgs e)
        {
            ClearColorPaletteData();
        }

        private async void OpenPalette_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".cpb");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                await LoadPaletteFromFile(file.Path);

                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["LastOpenedFilePath"] = file.Path;

            }
        }


        //TODO: toggle isn't loading properly with the proper assign color button state
        // need to just bring back all of the toggle of the enable assignments and rebuild.

        private async Task<int> LoadPaletteFromFile(string filePath)
        {
            try
            {
                ColorPalette colorPalette = await FileService.LoadPaletteAsync(filePath);
                if (colorPalette != null)
                {
                    ClearColorPaletteData();

                    ColorPaletteData.ColorPaletteName = colorPalette.ColorPaletteName;
                    ColorPaletteData.ColorPaletteFile = colorPalette.ColorPaletteFile;
                    ColorPaletteData.IsColorAssignEnabled = colorPalette.IsColorAssignEnabled;

                    IsAssignButtonEnabled.IsOn = ColorPaletteData.IsColorAssignEnabled;
                    TitleBarFileName.Text = ColorPaletteData.ColorPaletteName;

                    foreach (var entry in colorPalette.ColorEntries)
                    {
                        ColorPaletteData.ColorEntries.Add(entry);
                    }
                    foreach (var group in colorPalette.ElementGroups)
                    {
                        if (group != defaultComboBoxText)
                        {
                            ColorPaletteData.ElementGroups.Add(group);
                        }
                    }
                    foreach (var state in colorPalette.ElementStates)
                    {
                        if (state != defaultComboBoxText)
                        {
                            ColorPaletteData.ElementStates.Add(state);
                        }
                    }

                    foreach (var item in ColorPaletteListView.Items)
                    {
                        if (item is ColorEntry colorEntry)
                        {
                            colorEntry.IsColorAssignEnabled = ColorPaletteData.IsColorAssignEnabled;
                        }
                    }

                    // force a rebind of the ListView to ensure it updates
                    ColorPaletteListView.ItemsSource = null;
                    ApplyFilter();
                    ColorPaletteListView.ItemsSource = ColorPaletteData.FilteredColorEntries;


                }
                return 0;
            }
            catch
            {
                //TODO: Handle errors
                return -1;
            }
        }


        private async void SavePalette_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ColorPaletteData.ColorPaletteFile) || ColorPaletteData.ColorPaletteFile == "New Palette")
            {
                SavePaletteAs_Click(sender, e);
                return;
            }

            await SavePaletteToFile(ColorPaletteData.ColorPaletteFile);
        }

        private async Task SavePaletteToFile(string filePath)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
            if (file != null)
            {
                ColorPaletteData.ColorPaletteFile = file.Path;
                await FileService.SavePaletteAsync(file.Path, ColorPaletteData);

                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["LastOpenedFilePath"] = file.Path;

                ColorPaletteData.ColorPaletteName = file.DisplayName;
            }
        }

        private async void SavePaletteAs_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileSavePicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add("Color Palette Builder", new List<string> { ".cpb" });
            picker.DefaultFileExtension = ".cpb";

            StorageFile file = picker.PickSaveFileAsync().AsTask().Result;
            if (file != null)
            {
                await SavePaletteToFile(file.Path);
            }
        }





        #endregion



        private void LeftScrollViewerControl_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
        }

        private void ElementStateComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                comboBox.ItemsSource = ColorPaletteData.ElementStates;
            }
        }


        private void ElementGroupComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                comboBox.ItemsSource = ColorPaletteData.ElementGroups;
            }
        }


        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void AddColorEntry_Click(object sender, RoutedEventArgs e)
        {
            ColorEntry newEntry = new ColorEntry
            {
                ElementName = "Name",
                ElementGroup = ColorPaletteData.ElementGroups.FirstOrDefault(),
                ElementState = ColorPaletteData.ElementStates.FirstOrDefault(),
                HexCode = "#FF000000"

            };

            newEntry.HexCode = currentColorPickerHex;
            newEntry.IsColorAssignEnabled = ColorPaletteData.IsColorAssignEnabled;

            ColorPaletteData.ColorEntries.Insert(0, newEntry);

            ApplyFilter();

        }
        private void RemoveColorEntry_Click(object sender, RoutedEventArgs e)
        {
            var selectedEntry = ColorPaletteListView.SelectedItem as ColorEntry;
            if (selectedEntry != null)
            {
                ColorPaletteData.ColorEntries.Remove(selectedEntry);
            }

            ApplyFilter();
        }

        private void CustomColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (currentColorPickerHex != null)
            {
                currentColorPickerHex = ColorConverter.ToHex(ColorConverter.ConvertColorToSysDrawColor(args.NewColor), includeAlpha: true);
            }
        }

        private void AssignColor_Click(object sender, RoutedEventArgs e)
        {

            var button = sender as Button;
            if (button != null && button.DataContext is ColorEntry colorEntry)
            {
                System.Drawing.Color color = ColorConverter.ConvertColorToSysDrawColor(CustomColorPicker.Color);
                colorEntry.HexCode = ColorConverter.ToHex(color, includeAlpha: true);
            }
        }

        private void MainWindow_Closed(object sender, WindowEventArgs e)
        {
            if (string.IsNullOrEmpty(ColorPaletteData.ColorPaletteFile) || ColorPaletteData.ColorPaletteFile == "New Palette")
            {
                //TODO: Modal to ask if save file

            }


            SavePaletteToFile(ColorPaletteData.ColorPaletteFile);

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["WindowWidth"] = this.AppWindow.Size.Width;
            localSettings.Values["WindowHeight"] = this.AppWindow.Size.Height;

            if (settingsWindow != null)
            {
                settingsWindow.Close();
            }
        }






        #region Settings Window

        private SettingsWindow settingsWindow;

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            if (settingsWindow == null)
            {
                settingsWindow = new SettingsWindow();
                settingsWindow.Closed += SettingsWindow_Closed;
                settingsWindow.Activate();
            }




        }


        private void SettingsWindow_Closed(object sender, WindowEventArgs e)
        {
            settingsWindow = null;
        }

        #endregion



        #region State And Group Filters
        private void AddState_Click(object sender, RoutedEventArgs e)
        {
            if (ColorPaletteData.ElementStates.Contains(comboElementStates.Text))
            {
                TitleBarMessage.Text = "State already exists";
                titleMessageTimer.Start();
            }
            else
            {
                ColorPaletteData.ElementStates.Add(comboElementStates.Text);
            }

        }

        private void RemoveStateConfirmation_Click(object sender, RoutedEventArgs e)
        {
            string stateToRemove = comboElementStates.Text;

            // Set each ColorEntry with the state to remove to an empty string
            foreach (var colorEntry in ColorPaletteData.ColorEntries)
            {
                if (colorEntry.ElementState == stateToRemove)
                {
                    colorEntry.ElementState = string.Empty;
                }
            }

            // Now Remove from the ElementStates collection
            // Check if it is an empty string, for this cannot be removed
            if (stateToRemove != defaultComboBoxText)
            {
                ColorPaletteData.ElementStates.Remove(stateToRemove);
            }
            else // Give a message to know it worked but just can't be removed
            {
                TitleBarMessage.Text = "State cannot be removed";
                titleMessageTimer.Start();
            }

            comboElementStates.SelectedItem = comboElementStates.Items.FirstOrDefault();

            TitleBarMessage.Text = $"Removed State: {stateToRemove}";
            titleMessageTimer.Start();

            RemoveStateFlyout.Hide();
        }

        private void AddGroup_Click(object sender, RoutedEventArgs e)
        {
            if (ColorPaletteData.ElementGroups.Contains(comboElementGroups.Text))
            {
                TitleBarMessage.Text = "Group already exists";
                titleMessageTimer.Start();
            }
            else
            {
                ColorPaletteData.ElementGroups.Add(comboElementGroups.Text);
            }
        }

        private void RemoveGroupConfirmation_Click(object sender, RoutedEventArgs e)
        {
            string groupToRemove = comboElementGroups.Text;

            // Set each ColorEntry with the group to remove to an empty string
            foreach (var colorEntry in ColorPaletteData.ColorEntries)
            {
                if (colorEntry.ElementGroup == groupToRemove)
                {
                    colorEntry.ElementGroup = string.Empty;
                }
            }

            // Now Remove from the ElementGroups collection
            // Check if it is an empty string, for this cannot be removed
            if (groupToRemove != defaultComboBoxText)
            {
                ColorPaletteData.ElementGroups.Remove(groupToRemove);
            }
            else
            {
                TitleBarMessage.Text = "Group cannot be removed";
                titleMessageTimer.Start();
            }

            comboElementGroups.SelectedItem = comboElementGroups.Items.FirstOrDefault();

            TitleBarMessage.Text = $"Removed Group: {groupToRemove}";
            titleMessageTimer.Start();

            RemoveGroupFlyout.Hide();
        }


        #endregion



        private void IsColorAssign_Toggle(object sender, RoutedEventArgs e)
        {
            ColorPaletteData.IsColorAssignEnabled = !ColorPaletteData.IsColorAssignEnabled;

            TitleBarMessage.Text = ColorPaletteData.IsColorAssignEnabled ? "Quick Assign Unlocked" : "Quick Assign Lock-Out";
            titleMessageTimer.Start();

            foreach (var item in ColorPaletteListView.Items)
            {
                if (item is ColorEntry colorEntry)
                {
                    colorEntry.IsColorAssignEnabled = ColorPaletteData.IsColorAssignEnabled;
                }
            }

            ColorPaletteListView.ItemsSource = null;
            ColorPaletteListView.ItemsSource = ColorPaletteData.ColorEntries;

        }

        private void IsAssignButtonEnabled_Loaded(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                toggleSwitch.IsOn = ColorPaletteData.IsColorAssignEnabled;
            }
        }



        private void RefreshFilterButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void OnFilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((ComboBox)sender == comboElementStates)
            {
                SelectedState = comboElementStates.SelectedItem as string;
            }
            else if ((ComboBox)sender == comboElementGroups)
            {
                SelectedGroup = comboElementGroups.SelectedItem as string;
            }

            TitleBarMessage.Text = $"Filter: {SelectedState} - {SelectedGroup}";
            titleMessageTimer.Start();

        }

        private void ApplyFilter()
        {

            ClearFilteredColorEntries();

            foreach (var colorEntry in ColorPaletteData.ColorEntries)
            {
                if (SelectedState == defaultComboBoxText && SelectedGroup == defaultComboBoxText)
                {
                    ColorPaletteData.FilteredColorEntries.Add(colorEntry);
                }
                else if (SelectedState == defaultComboBoxText && colorEntry.ElementGroup == SelectedGroup)
                {
                    ColorPaletteData.FilteredColorEntries.Add(colorEntry);
                }
                else if (SelectedGroup == defaultComboBoxText && colorEntry.ElementState == SelectedState)
                {
                    ColorPaletteData.FilteredColorEntries.Add(colorEntry);
                }
                else if (colorEntry.ElementState == SelectedState && colorEntry.ElementGroup == SelectedGroup)
                {
                    ColorPaletteData.FilteredColorEntries.Add(colorEntry);
                }
            }

            ColorPaletteListView.ItemsSource = null;
            ColorPaletteListView.ItemsSource = ColorPaletteData.FilteredColorEntries;

        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            comboElementStates.SelectedItem = defaultComboBoxText;
            comboElementGroups.SelectedItem = defaultComboBoxText;

            ApplyFilter();
        }


        /*===================================================================================================/
         *                                          Color Selector                                           /
         *==================================================================================================*/


        private ColorSelectorWindow colorSelectorWindow;
        private void ColorSelectorButton_Click(object sender, RoutedEventArgs e)
        {
            StartColorSelector();
        }

        private void StartColorSelector()
        {

            //Get screen shot
            if (App.colorSelectorBitmap == null)
            {
                App.colorSelectorBitmap = ColorSelectorProcessor.GetBitmap();
            }

            if (colorSelectorWindow == null)
            {
                colorSelectorWindow = new ColorSelectorWindow();
                colorSelectorWindow.AppWindow.SetPresenter(AppWindowPresenterKind.Overlapped);


                colorSelectorWindow.Closed += ColorSelectorWindow_Closed;
                colorSelectorWindow.DataSelected += ColorSelectorWindow_DataSelected;
                colorSelectorWindow.Activate();
            }
            else
            {
                TitleBarMessage.Text = "Color Selector already open";
                titleMessageTimer.Start();
            }





        }

        private void ColorSelectorWindow_DataSelected(object sender, EventArgs e)
        {
            CustomColorPicker.Color = ColorConverter.ConvertColorToWinUIColor(App.colorSelectorColor);

        }

        private void ColorSelectorWindow_Closed(object sender, WindowEventArgs e)
        {
            colorSelectorWindow = null;

            CustomColorPicker.Color = ColorConverter.ConvertColorToWinUIColor(App.colorSelectorColor);

        }


        private async void BrowseColorSelectorPhoto_Click(object sender, RoutedEventArgs e)
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
            if (file != null)
            {
                // Store the path in local settings
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["LastOpenedFilePath"] = file.Path;

                // Load the image into a WriteableBitmap
                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);
                    WriteableBitmap bitmap = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                    await bitmap.SetSourceAsync(fileStream);

                    // Save the WriteableBitmap to the static colorSelectorBitmap variable
                    App.colorSelectorBitmap = bitmap;
                }

                ColorSelectorSource.Text = file.Name;
            }
        }

        private void ColorSelectorSource_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        private async void ColorSelectorSource_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Any())
                {
                    var storageFile = items[0] as StorageFile;
                    if (IsImageFile(storageFile))
                    {
                        // Load image into WriteableBitmap
                        using (IRandomAccessStream fileStream = await storageFile.OpenAsync(FileAccessMode.Read))
                        {
                            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);
                            WriteableBitmap bitmap = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                            await bitmap.SetSourceAsync(fileStream);

                            // Save the WriteableBitmap to the static colorSelectorBitmap variable
                            App.colorSelectorBitmap = bitmap;

                            ColorSelectorSource.Text = storageFile.Name;

                            TitleBarMessage.Text = $"Image dropped: {storageFile.Name}";
                            titleMessageTimer.Start();
                        }

                    }
                    else
                    {
                        TitleBarMessage.Text = "Invalid image / file type";
                        titleMessageTimer.Start();
                    }
                }
            }
        }

        private bool IsImageFile(StorageFile file)
        {
            var imageFileTypes = new[] { ".bmp", ".jpg", ".jpeg", ".png" };
            return imageFileTypes.Contains(file.FileType.ToLower());
        }

        private async void ColorSelectorSource_Paste(object sender, TextControlPasteEventArgs e)
        {
            e.Handled = true; // Prevent the default paste behavior

            var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
            try
            {
                if (dataPackageView.Contains(StandardDataFormats.Bitmap))
                {
                    var bitmap = await dataPackageView.GetBitmapAsync();
                    if (bitmap != null)
                    {
                        using (var stream = await bitmap.OpenReadAsync())
                        {
                            // Ensure the stream is at the beginning
                            stream.Seek(0);

                            // Decode the bitmap
                            var decoder = await BitmapDecoder.CreateAsync(stream);
                            var writeableBitmap = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                            await writeableBitmap.SetSourceAsync(stream);

                            // Assign the WriteableBitmap to the static variable
                            App.colorSelectorBitmap = writeableBitmap;

                            // Update the UI
                            ColorSelectorSource.Text = "Pasted Image";
                            TitleBarMessage.Text = "Image pasted from clipboard.";
                            titleMessageTimer.Start();
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
            catch (Exception ex)
            {
                TitleBarMessage.Text = $"An error occurred: {ex.Message}";
            }
        }


    }
}
