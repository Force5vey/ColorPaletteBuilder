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
using Windows.ApplicationModel.Preview.Notes;



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

        public ColorPalette ColorPaletteData { get; set; } = new ColorPalette
        {
            ColorEntries = new ObservableCollection<ColorEntry>(),
            ElementGroups = new ObservableCollection<string>(),
            ElementStates = new ObservableCollection<string>()
        };

        private ColorEntry _currentEntry = new ColorEntry();

        private DispatcherTimer titleMessageTimer = new DispatcherTimer();

        private bool isColorAssignEnabled = false;

        public MainWindow()
        {
            this.InitializeComponent();

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
                // Ignore errors (maybe for now) assign a default window size
                this.AppWindow.Resize(new Windows.Graphics.SizeInt32(800, 600));
            }

            ColorPaletteListView.DataContext = this;
            ColorPaletteListView.ItemsSource = ColorPaletteData.ColorEntries;

            LoadLastOpenedFile();

            this.Closed += MainWindow_Closed;

            titleMessageTimer.Interval = TimeSpan.FromSeconds(2);
            titleMessageTimer.Tick += TitleMessageTimer_Tick;
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

        private void ClearColorPaletteData()
        {
            ColorPaletteData.ColorPaletteFile = "New Palette";
            ColorPaletteData.ColorPaletteName = "New Palette";
            ColorPaletteData.ColorEntries.Clear();
            ColorPaletteData.ElementStates.Clear();
            ColorPaletteData.ElementGroups.Clear();
        }

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

                    // force a rebind of the ListView to ensure it updates
                    ColorPaletteListView.ItemsSource = null;
                    ColorPaletteListView.ItemsSource = ColorPaletteData.ColorEntries;

                }
                return 0;
            }
            catch (Exception ex)
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

            newEntry.HexCode = _currentEntry.HexCode;
            newEntry.IsColorAssignEnabled = isColorAssignEnabled;

            ColorPaletteData.ColorEntries.Insert(0, newEntry);

        }
        private void RemoveColorEntry_Click(object sender, RoutedEventArgs e)
        {
            var selectedEntry = ColorPaletteListView.SelectedItem as ColorEntry;
            if (selectedEntry != null)
            {
                ColorPaletteData.ColorEntries.Remove(selectedEntry);
            }
        }

        private void CustomColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (_currentEntry != null)
            {
                _currentEntry.HexCode = ColorConverter.ToHex(ColorConverter.ConvertColorToSysDrawColor(args.NewColor), includeAlpha: true);
            }
        }

        private void AssignColor_Click(object sender, RoutedEventArgs e)
        {

            //TODO: This needs an enabled / disabled function to avoid accidental assignment

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



        private void UnlockColorAssign_Click(object sender, RoutedEventArgs e)
        {
            isColorAssignEnabled = !isColorAssignEnabled;

            TitleBarMessage.Text = isColorAssignEnabled ? "Editing Enabled" : "Editing Disabled";
            titleMessageTimer.Start();

            foreach (var item in ColorPaletteListView.Items)
            {
                if (item is ColorEntry colorEntry)
                {
                    colorEntry.IsColorAssignEnabled = isColorAssignEnabled;
                }
            }

            ColorPaletteListView.ItemsSource = null;
            ColorPaletteListView.ItemsSource = ColorPaletteData.ColorEntries;


            if (isColorAssignEnabled)
            {
                EditListViewButton.Label = "Lock";

            }
            else
            {
                EditListViewButton.Label = "Edit";
            }

        }


        private void TitleMessageTimer_Tick(object sender, object e)
        {
            TitleBarMessage.Text = "";
            titleMessageTimer.Stop();
        }


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











        #region Color Picker Logic

        /*  ******************   Logic for Color Picker    ******************  */


        private GraphicsCaptureItem _captureItem;
        private Direct3D11CaptureFramePool _framePool;
        private GraphicsCaptureSession _session;
        private bool _isEyeDropperActive = false;


        private async void SelectColorButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO: None of this is working at all, all the way through the other methods.
            _isEyeDropperActive = !_isEyeDropperActive;

            // Change the cursor to eye-dropper
            // (Ensure you have a cursor file for eye-dropper)
            //Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Custom, 0);

            // Listen for the pointer pressed event
            //Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
        }

        private async void CoreWindow_PointerPressed(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            if (_isEyeDropperActive)
            {
                var pointerPosition = args.CurrentPoint.Position;
                var color = await GetColorAt((int)pointerPosition.X, (int)pointerPosition.Y);

                // Set the captured color to your color entry
                _currentEntry.HexCode = ColorConverter.ToHex(color, includeAlpha: true);

                // Revert the cursor back to default
                sender.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);

                // Remove the event handler
                sender.PointerPressed -= CoreWindow_PointerPressed;
                _isEyeDropperActive = false;
            }
        }

        private async Task<Color> GetColorAt(int x, int y)
        {
            var picker = new GraphicsCapturePicker();
            _captureItem = await picker.PickSingleItemAsync();
            if (_captureItem == null)
            {
                return System.Drawing.Color.Transparent; // Handle the case where the user cancels the picker
            }

            //var device = Direct3D11Helper.CreateDevice();
            //var itemSize = _captureItem.Size;
            //_framePool = Direct3D11CaptureFramePool.CreateFreeThreaded(
            //    device,
            //    DirectXPixelFormat.B8G8R8A8UIntNormalized,
            //    1,
            //    itemSize);

            _session = _framePool.CreateCaptureSession(_captureItem);
            _session.StartCapture();

            var frame = _framePool.TryGetNextFrame();
            var bitmap = await SoftwareBitmap.CreateCopyFromSurfaceAsync(frame.Surface);

            using (var stream = new InMemoryRandomAccessStream())
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetSoftwareBitmap(bitmap);
                await encoder.FlushAsync();

                stream.Seek(0);

                var decoder = await BitmapDecoder.CreateAsync(stream);
                var pixelData = await decoder.GetPixelDataAsync();
                var pixels = pixelData.DetachPixelData();

                var index = (y * bitmap.PixelWidth + x) * 4;
                byte b = pixels[index];
                byte g = pixels[index + 1];
                byte r = pixels[index + 2];
                byte a = pixels[index + 3];

                return Color.FromArgb(a, r, g, b);
            }
        }




        #endregion
    }
}
