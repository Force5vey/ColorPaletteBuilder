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


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ColorPaletteBuilder
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public ColorPalette ColorPaletteData { get; set; } = new ColorPalette
        {
            ColorEntries = new ObservableCollection<ColorEntry>(),
            ElementGroups = new ObservableCollection<string>(),
            ElementStates = new ObservableCollection<string>()
        };

        private ColorEntry _currentEntry = new ColorEntry();

        public MainWindow()
        {
            this.InitializeComponent();
            this.AppWindow.Resize(new Windows.Graphics.SizeInt32(1000, 800));


            ColorPaletteListView.DataContext = this;

            ColorPaletteListView.ItemsSource = ColorPaletteData.ColorEntries;

        }

        private void ClearColorPaletteData()
        {
            ColorPaletteData.ColorPaletteFile = "New Palette";
            ColorPaletteData.ColorPaletteName = "New Palette";
            ColorPaletteData.ColorEntries.Clear();
            ColorPaletteData.ElementStates.Clear();
            ColorPaletteData.ElementGroups.Clear();
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

        private void CopyHexCode_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.DataContext is ColorEntry colorEntry)
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(colorEntry.HexCode);
                Clipboard.SetContent(dataPackage);
            }
        }

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
                ColorPalette colorPalette = await FileService.LoadPaletteAsync(file.Path);

                ClearColorPaletteData();

                ColorPaletteData.ColorPaletteName = colorPalette.ColorPaletteName;
                ColorPaletteData.ColorPaletteFile = colorPalette.ColorPaletteFile;

                foreach (var entry in colorPalette.ColorEntries)
                {
                    ColorPaletteData.ColorEntries.Add(entry);
                }
                foreach (var group in colorPalette.ElementGroups)
                {
                    ColorPaletteData.ElementGroups.Add(group);
                }
                foreach (var state in colorPalette.ElementStates)
                {
                    ColorPaletteData.ElementStates.Add(state);
                }

                // I am right here to start implementing the auto save / auto open last file that is laid out in last conversation
                // of the chatgpt conversation

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
                ColorPaletteData.ColorPaletteFile = file.Path;
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
                ColorPaletteData.ColorPaletteFile = file.Path;
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


        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {

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

            newEntry.HexCode = _currentEntry.HexCode;

            ColorPaletteData.ColorEntries.Insert(0, newEntry);

        }
        private void RemoveColorEntry_Click(object sender, RoutedEventArgs e)
        {
          var selectedEntry = ColorPaletteListView.SelectedItem as ColorEntry;
            if(selectedEntry != null)
            {
                ColorPaletteData.ColorEntries.Remove(selectedEntry);
            }
        }

        private void CustomColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (_currentEntry != null)
            {
                var color = args.NewColor;
                _currentEntry.HexCode = ColorConverter.ToHex(color, includeAlpha: true);
            }
        }

        private void AssignColor_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.DataContext is ColorEntry colorEntry)
            {
                var color = CustomColorPicker.Color;
                colorEntry.HexCode = ColorConverter.ToHex(color, includeAlpha: true);
            }
        }


    }
}
