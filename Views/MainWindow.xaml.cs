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

            LoadSampleData();

            ColorPaletteListView.ItemsSource = ColorPaletteData.ColorEntries;

        }

        private void LoadSampleData()
        {
            ColorPaletteData = new ColorPalette
            {
                ColorPaletteName = "Default",
                ColorEntries = new ObservableCollection<ColorEntry>
            {
                new ColorEntry { ElementName = "Main UI Screen Accent", ElementState = "Enabled", ElementGroup = "UI", HexCode = "#FF0000", Alpha = .5, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Red) },
                new ColorEntry { ElementName = "Secondary UI Screen Item Selection", ElementState = "Enabled", ElementGroup = "Game Play", HexCode = "#00FF00", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Green) },
                new ColorEntry { ElementName = "Blue", ElementState = "Enabled", ElementGroup = "UI", HexCode = "#0000FF", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Blue) },
                new ColorEntry { ElementName = "Yellow", ElementState = "Disabled", ElementGroup = "Background", HexCode = "#FFFF00", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Yellow) },
                new ColorEntry { ElementName = "Cyan", ElementState = "Enabled", ElementGroup = "Text", HexCode = "#00FFFF", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Cyan) },
                new ColorEntry { ElementName = "Magenta", ElementState = "Disabled", ElementGroup = "Text", HexCode = "#FF00FF", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Magenta) },
                new ColorEntry { ElementName = "Red", ElementState = "Enabled", ElementGroup = "UI", HexCode = "#FF0000", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Red) },
                new ColorEntry { ElementName = "Green", ElementState = "Enabled", ElementGroup = "UI", HexCode = "#00FF00", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Green) },
                new ColorEntry { ElementName = "Blue", ElementState = "Enabled", ElementGroup = "UI", HexCode = "#0000FF", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Blue) },
                new ColorEntry { ElementName = "Yellow", ElementState = "Disabled", ElementGroup = "Background", HexCode = "#FFFF00", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Yellow) },
                new ColorEntry { ElementName = "Cyan", ElementState = "Enabled", ElementGroup = "Text", HexCode = "#00FFFF", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Cyan) },
                new ColorEntry { ElementName = "Magenta", ElementState = "Disabled", ElementGroup = "Text", HexCode = "#FF00FF", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Magenta) },
                new ColorEntry { ElementName = "Red", ElementState = "Enabled", ElementGroup = "UI", HexCode = "#FF0000", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Red) },
                new ColorEntry { ElementName = "Green", ElementState = "Enabled", ElementGroup = "UI", HexCode = "#00FF00", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Green) },
                new ColorEntry { ElementName = "Blue", ElementState = "Enabled", ElementGroup = "UI", HexCode = "#0000FF", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Blue) },
                new ColorEntry { ElementName = "Yellow", ElementState = "Disabled", ElementGroup = "Background", HexCode = "#FFFF00", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Yellow) },
                new ColorEntry { ElementName = "Cyan", ElementState = "Enabled", ElementGroup = "Text", HexCode = "#00FFFF", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Cyan) },
                new ColorEntry { ElementName = "Magenta", ElementState = "Disabled", ElementGroup = "Text", HexCode = "#FF00FF", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Magenta) },
                new ColorEntry { ElementName = "Red", ElementState = "Enabled", ElementGroup = "UI", HexCode = "#FF0000", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Red) },
                new ColorEntry { ElementName = "Green", ElementState = "Enabled", ElementGroup = "UI", HexCode = "#00FF00", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Green) },
                new ColorEntry { ElementName = "Blue", ElementState = "Enabled", ElementGroup = "UI", HexCode = "#0000FF", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Blue) },
                new ColorEntry { ElementName = "Yellow", ElementState = "Disabled", ElementGroup = "Background", HexCode = "#FFFF00", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Yellow) },
                new ColorEntry { ElementName = "Cyan", ElementState = "Enabled", ElementGroup = "Text", HexCode = "#00FFFF", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Cyan) },
                new ColorEntry { ElementName = "Magenta", ElementState = "Disabled", ElementGroup = "Text", HexCode = "#FF00FF", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Magenta) }
            },
                ElementGroups = new List<string> { "UI", "Game Play", "Level", "Designer" },
                ElementStates = new List<string> { "Enabled", "Disabled", "Selected", "No Focus" }
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

        private void OpenPalette_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SavePalette_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SavePaletteAs_Click(object sender, RoutedEventArgs e)
        {
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
            if(comboBox != null)
            {
                comboBox.ItemsSource = ColorPaletteData.ElementGroups;
            }   
        }
    }
}
