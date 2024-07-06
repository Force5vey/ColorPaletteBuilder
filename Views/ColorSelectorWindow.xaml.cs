using Microsoft.UI.Input;
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
using System.Drawing;
using Microsoft.UI.Xaml.Media.Imaging;
using System.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ColorPaletteBuilder
{
     /// <summary>
     /// An empty window that can be used on its own or navigated to within a Frame.
     /// </summary>
     public sealed partial class ColorSelectorWindow :Window
     {
          private MainWindow _mainWindow;

          public event EventHandler DataSelected;

          public ColorSelectorWindow( MainWindow mainWindow )
          {
               this.InitializeComponent();

               _mainWindow = mainWindow;

               _mainWindow.SetColorSelectorImage += OnImageUpdated;

               ExtendsContentIntoTitleBar = true;

               ScreenshotImage.Source = App.ColorSelectorBitmap;

               this.Closed += ColorSelectorWindow_Closed;

               LoadWindowSizeAndLocation();
          }

          private void ColorSelectorWindow_Closed( object sender, WindowEventArgs args )
          {
               var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
               localSettings.Values["ColorSelectorWindowWidth"] = this.AppWindow.Size.Width;
               localSettings.Values["ColorSelectorWindowHeight"] = this.AppWindow.Size.Height;
               localSettings.Values["ColorSelectorWindowLeft"] = this.AppWindow.Position.X;
               localSettings.Values["ColorSelectorWindowTop"] = this.AppWindow.Position.Y;

               _mainWindow.SetColorSelectorImage -= OnImageUpdated;
          }

          private void LoadWindowSizeAndLocation()
          {
               var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
               if ( localSettings.Values.TryGetValue("ColorSelectorWindowWidth", out object width) &&
                   localSettings.Values.TryGetValue("ColorSelectorWindowHeight", out object height) &&
                   localSettings.Values.TryGetValue("ColorSelectorWindowLeft", out object left) &&
                   localSettings.Values.TryGetValue("ColorSelectorWindowTop", out object top) )
               {
                    this.AppWindow.Resize(new Windows.Graphics.SizeInt32 { Width = (int)width, Height = (int)height });
                    this.AppWindow.Move(new Windows.Graphics.PointInt32 { X = (int)left, Y = (int)top });
               }
          }

          private void OnImageUpdated( object sender, EventArgs e )
          {
               ScreenshotImage.Source = App.ColorSelectorBitmap;
          }

          private InputCursor? OriginalInputCursor { get; set; }

          private void CustomGrid_PointerEntered( object sender, PointerRoutedEventArgs e )
          {
               OriginalInputCursor = this.CustomGrid.InputCursor ?? InputSystemCursor.Create(InputSystemCursorShape.Arrow);
               this.CustomGrid.InputCursor = InputSystemCursor.Create(InputSystemCursorShape.Cross);

          }

          private void CustomGrid_PointerExited( object sender, PointerRoutedEventArgs e )
          {
               if ( OriginalInputCursor != null )
               {
                    //this.CustomGrid.InputCursor = OriginalInputCursor;
               }
          }

          private void ScreenShotImage_Clicked( object sender, PointerRoutedEventArgs e )
          {

               if ( ScreenshotImage.Source != null )
               {
                    var position = e.GetCurrentPoint(ScreenshotImage).Position;

                    App.colorSelectorColor = GetColorAtPosition((int)position.X, (int)position.Y);

                    DataSelected?.Invoke(this, EventArgs.Empty);
               }


          }

          private void ScreenShotImage_PointerMoved( object sender, PointerRoutedEventArgs e )
          {
               var position = e.GetCurrentPoint(ScreenshotImage).Position;
               XCoord.Content = $"X: {position.X}";
               YCoord.Content = $"Y: {position.Y}";

               HoverColor.Background = ColorConverter.ConvertToSolidColorBrush(GetColorAtPosition((int)position.X, (int)position.Y));
          }


          private Color GetColorAtPosition( int x, int y )
          {
               using ( Stream pixelStream = App.ColorSelectorBitmap.PixelBuffer.AsStream() )
               {
                    byte[] pixels = new byte[4]; //BGRA format
                    int widthInBytes = 4 * App.ColorSelectorBitmap.PixelWidth;

                    pixelStream.Seek(y * widthInBytes + x * 4, SeekOrigin.Begin);
                    pixelStream.Read(pixels, 0, 4);

                    byte b = pixels[0];
                    byte g = pixels[1];
                    byte r = pixels[2];
                    byte a = pixels[3];

                    return Color.FromArgb(a, r, g, b);
               }
          }

          private void ColorSelectorExitButton_Click( object sender, RoutedEventArgs e )
          {
               _mainWindow.SetColorSelectorImage -= OnImageUpdated;
               this.Close();
          }

          private void DisplayImage_ManipulationDelta( object sender, ManipulationDeltaRoutedEventArgs e )
          {
               var scale = e.Delta.Scale;
               var newZoomFactor = ImageScrollViewer.ZoomFactor * scale;
               if ( newZoomFactor >= ImageScrollViewer.MinZoomFactor && newZoomFactor <= ImageScrollViewer.MaxZoomFactor )
               {
                    ImageScrollViewer.ChangeView(null, null, newZoomFactor);
               }
          }

          private void ImageScrollViewer_DoubleTapped( object sender, DoubleTappedRoutedEventArgs e )
          {
               var pointerPosition = e.GetPosition(ScreenshotImage);
               var currentZoomFactor = ImageScrollViewer.ZoomFactor;
               if ( currentZoomFactor < 2.0f )
               {
                    ImageScrollViewer.ChangeView(pointerPosition.X, pointerPosition.Y, 2.0f);
               }
               else
               {
                    ImageScrollViewer.ChangeView(pointerPosition.X, pointerPosition.Y, 1.0f);
               }
          }
     }
}
