using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Collections.ObjectModel;
using System.Drawing;
using Microsoft.UI;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.ApplicationModel.DataTransfer;
using System.Threading.Tasks;
using Windows.Graphics.Capture;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Microsoft.UI.Windowing;
using System.ComponentModel;
using Microsoft.UI.Input;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ColorPaletteBuilder
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        int settingsWindowWidth = 400;
        int settingsWindowHeight = 600;


        public ColorPalette ColorPaletteData = new ColorPalette();


        private string currentColorPickerHex = "#FFFFFFFF";
        private string defaultComboBoxText = "Any";

        private DispatcherTimer titleMessageTimer = new DispatcherTimer();

        public string SelectedState { get; set; }
        public string SelectedGroup { get; set; }

        public MainWindow()
        {
            this.InitializeComponent();

            ColorPaletteData = new ColorPalette();

            SelectedState = defaultComboBoxText;
            SelectedGroup = defaultComboBoxText;




            // Set up event handlers
            comboElementStates.SelectionChanged += OnFilterSelectionChanged;
            comboElementGroups.SelectionChanged += OnFilterSelectionChanged;


            // Set the window size according to last window size
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
                this.AppWindow.Resize(new Windows.Graphics.SizeInt32(800, 600));
            }

            ColorPaletteListView.ItemsSource = ColorPaletteData.FilteredColorEntries;

            LoadLastOpenedFile();

            this.Closed += MainWindow_Closed;

            titleMessageTimer.Interval = TimeSpan.FromSeconds(2);
            titleMessageTimer.Tick += TitleMessageTimer_Tick;

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

                    // force a rebind of the ListView to ensure it updates
                    ColorPaletteListView.ItemsSource = null;
                    ApplyFilter();
                    ColorPaletteListView.ItemsSource = ColorPaletteData.FilteredColorEntries;

                    IsAssignButtonEnabled.IsOn = ColorPaletteData.IsColorAssignEnabled;

                    foreach (var item in ColorPaletteListView.Items)
                    {
                        if (item is ColorEntry colorEntry)
                        {
                            colorEntry.IsColorAssignEnabled = ColorPaletteData.IsColorAssignEnabled;
                        }
                    }


                    TitleBarFileName.Text = ColorPaletteData.ColorPaletteName;

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

            settingsWindow.AppWindow.Resize(new Windows.Graphics.SizeInt32(settingsWindowWidth, settingsWindowHeight));


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




        //=============================    Color Selector    =============================

        private ColorSelectorWindow colorSelectorWindow;
        private void ColorSelectorButton_Click(object sender, RoutedEventArgs e)
        {
            StartColorSelector();
        }

        private void StartColorSelector()
        {

            //Get screen shot
            App.screenShot = ScreenCapture.CaptureScreen();

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

      

    }
}
