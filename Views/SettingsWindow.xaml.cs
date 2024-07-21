using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
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
using Windows.Storage;
using System.Threading.Tasks;
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
     public sealed partial class SettingsWindow :Window
     {
          private SettingsViewModel settingsViewModel { get; set; }

          private const int settingsWindowStartWidth = 650;
          private const int settingsWindowStartHeight = 1000;
          private const int settingsWindowMinWidth = 425;
          private const int settingsWindowMinHeight = 700;

          //check git changes for branch
          public SettingsWindow()
          {
               this.InitializeComponent();
               settingsViewModel = new SettingsViewModel(App.UserSettings);

               ExtendsContentIntoTitleBar = true;

               this.AppWindow.Resize(new Windows.Graphics.SizeInt32(settingsWindowStartWidth, settingsWindowStartHeight));
               this.AppWindow.Changed += SettingsWindow_Changed;

               LoadLocalFilePath();

          }

          private void BrowsePreferredSaveFolder_Click( object sender, RoutedEventArgs e )
          {
               settingsViewModel.BrowsePreferredPaletteSaveFolder(this);
          }

          private void LoadLocalFilePath()
          {
               LocalFolderPathTextbox.Text = ApplicationData.Current.LocalFolder.Path;
          }

          private void SaveSettings_Click( object sender, RoutedEventArgs e )
          {
               settingsViewModel.SaveSettings();
               this.Close();
          }

          private void SettingsWindow_Changed( Microsoft.UI.Windowing.AppWindow sender, object args )
          {

               if ( sender.Size.Width < settingsWindowMinWidth )
               {
                    this.AppWindow.Resize(new Windows.Graphics.SizeInt32(settingsWindowMinWidth, (int)sender.Size.Height));
               }
               if ( sender.Size.Height < settingsWindowMinHeight )
               {
                    this.AppWindow.Resize(new Windows.Graphics.SizeInt32((int)sender.Size.Width, settingsWindowMinHeight));
               }
          }

          private void MyDocsButton_Click( object sender, RoutedEventArgs e )
          {
               DefaultPaletteLocationTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
          }

          private void DesktopButton_Click( object sender, RoutedEventArgs e )
          {
               DefaultPaletteLocationTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
          }

          private void FavoritesButton_Click( object sender, RoutedEventArgs e )
          {
               DefaultPaletteLocationTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
          }

     }
}

