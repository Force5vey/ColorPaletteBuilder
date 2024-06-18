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


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ColorPaletteBuilder
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public ColorPalette ColorPaletteData { get; set; } = new ColorPalette();



        public MainWindow()
        {
            this.InitializeComponent();
            this.AppWindow.Resize(new Windows.Graphics.SizeInt32(1000, 800));

            //LoadSampleData();

            ColorPaletteListView.DataContext = this;

            ColorPaletteListView.ItemsSource = ColorPaletteData.ColorEntries;

        }

        private void LoadSampleData()
        {
            ColorPaletteData = new ColorPalette
            {
                ColorPaletteName = "Default",
                ElementGroups = new List<string> { "UI", "Game Play", "Level", "Designer", "Background", "Text" },
                ElementStates = new List<string> { "Enabled", "Disabled", "Selected", "No Focus" },
                ColorEntries = new ObservableCollection<ColorEntry>
                {
                    new ColorEntry {ElementName = "Button", ElementGroup = "UI", ElementState = "Enabled", HexCode = "#FF0000", Alpha = 1.0, DisplayColor = "PlaceHolder"},
                    new ColorEntry { ElementName = "Main Background Page Color Accent", ElementGroup= "Game Play", ElementState = "Enabled", HexCode = "#00FF00", Alpha = 1.0, DisplayColor = "PlaceHolder" },
                    new ColorEntry { ElementName = "Header Text", ElementGroup = "UI", ElementState = "Selected", HexCode = "#0000FF", Alpha = 1.0 },
                    new ColorEntry { ElementName = "Footer Background", ElementGroup = "UI", ElementState = "Enabled", HexCode = "#FFFF00", Alpha = 1.0 },
                    new ColorEntry { ElementName = "Sidebar", ElementGroup = "UI", ElementState = "No Focus", HexCode = "#FF00FF", Alpha = 1.0 },
                    new ColorEntry { ElementName = "Link Text", ElementGroup = "Text", ElementState = "Selected", HexCode = "#00FFFF", Alpha = 1.0 },
                    new ColorEntry { ElementName = "Warning Text", ElementGroup = "Text", ElementState = "Enabled", HexCode = "#FFA500", Alpha = 1.0 },
                    new ColorEntry { ElementName = "Success Text", ElementGroup = "Text", ElementState = "Enabled", HexCode = "#008000", Alpha = 1.0 },
                    new ColorEntry { ElementName = "Info Background", ElementGroup = "Background", ElementState = "Enabled", HexCode = "#ADD8E6", Alpha = 1.0 },
                    new ColorEntry { ElementName = "Alert Background", ElementGroup = "Background", ElementState = "Enabled", HexCode = "#FF4500", Alpha = 1.0 },
                    new ColorEntry { ElementName = "Primary Button Text", ElementGroup = "UI", ElementState = "Enabled", HexCode = "#FFFFFF", Alpha = 1.0 },
                    new ColorEntry { ElementName = "Secondary Button Text", ElementGroup = "UI", ElementState = "Enabled", HexCode = "#000000", Alpha = 1.0 },
                    new ColorEntry { ElementName = "Card Background", ElementGroup = "Background", ElementState = "Enabled", HexCode = "#F0E68C", Alpha = 1.0 },
                    new ColorEntry { ElementName = "Primary Button Background", ElementGroup = "UI", ElementState = "Enabled", HexCode = "#1E90FF", Alpha = 1.0 },
                    new ColorEntry { ElementName = "Secondary Button Background", ElementGroup = "UI", ElementState = "Enabled", HexCode = "#D3D3D3", Alpha = 1.0 },
                    new ColorEntry { ElementName = "Modal Background", ElementGroup = "Background", ElementState = "Enabled", HexCode = "#2F4F4F", Alpha = 1.0 },
                    new ColorEntry { ElementName = "Modal Text", ElementGroup = "Text", ElementState = "Enabled", HexCode = "#F5F5F5", Alpha = 1.0 }

                }
            };

        }


        private void CopyToClipboard(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dataPackage.SetText(text);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            }
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
                ColorPalette colorPalette = await FileService.LoadPaletteAsync(file.Path);
                ColorPaletteData.ColorEntries.Clear();
                ColorPaletteData = colorPalette;
            }
        }

        private async void SavePalette_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ColorPaletteData.ColorPaletteFile) || ColorPaletteData.ColorPaletteFile == "New Palette")
            {
                SavePaletteAs_Click(sender, e);
                return;
            }

            StorageFile file = await StorageFile.GetFileFromPathAsync(ColorPaletteData.ColorPaletteFile);
            if (file != null)
            {
                await FileService.SavePaletteAsync(file.Path, ColorPaletteData);
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
                await FileService.SavePaletteAsync(file.Path, ColorPaletteData);
            }
        }

        private void ThemeRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

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

        private void NewPalette_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
