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
        public ObservableCollection<ColorEntry> ColorEntries { get; set; } = new ObservableCollection<ColorEntry>();

        public MainWindow()
        {
            this.InitializeComponent();
            this.AppWindow.Resize(new Windows.Graphics.SizeInt32(1200, 800));

            LoadSampleData();

            ColorPaletteListView.ItemsSource = ColorEntries;
        }

        private void LoadSampleData()
        {
            ColorEntries = new ObservableCollection<ColorEntry>
            {
            new ColorEntry { Name = "Red", State = "Enabled", ElementGroup = "UI", HexCode = "#FF0000", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Red) },
                new ColorEntry { Name = "Green", State = "Enabled", ElementGroup = "UI", HexCode = "#00FF00", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Green) },
                new ColorEntry { Name = "Blue", State = "Enabled", ElementGroup = "UI", HexCode = "#0000FF", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Blue) },
                new ColorEntry { Name = "Yellow", State = "Disabled", ElementGroup = "Background", HexCode = "#FFFF00", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Yellow) },
                new ColorEntry { Name = "Cyan", State = "Enabled", ElementGroup = "Text", HexCode = "#00FFFF", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Cyan) },
                new ColorEntry { Name = "Magenta", State = "Disabled", ElementGroup = "Text", HexCode = "#FF00FF", Alpha = 1.0, SelectedColor = new SolidColorBrush(Microsoft.UI.Colors.Magenta) }
            };

        }

        /// <summary>
        /// Color Wheel Selector
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectColor_Click(object sender, RoutedEventArgs e)
        {
        }


        /// <summary>
        /// Activate Eye Dropper
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ActivateEyeDropper_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Copy HexCode Code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CopyHexCode_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(tbHexCode.Text);
        }

        /// <summary>
        /// Copy RGB_A Code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CopyRGBA_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(tbRGBA.Text);
        }

        /// <summary>
        /// Copy CSharp Code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CopyCSharpCode_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(tbCSharp.Text);
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
    }
}
