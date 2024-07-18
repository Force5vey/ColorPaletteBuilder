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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ColorPaletteBuilder
{
     /// <summary>
     /// An empty window that can be used on its own or navigated to within a Frame.
     /// </summary>
     public sealed partial class SettingsWindow :Window
     {
          private const int settingsWindowStartWidth = 650;
          private const int settingsWindowStartHeight = 1000;
          private const int settingsWindowMinWidth = 425;
          private const int settingsWindowMinHeight = 700;


          //check git changes for branch
          public SettingsWindow()
          {
               this.InitializeComponent();

               ExtendsContentIntoTitleBar = true;

               this.AppWindow.Resize(new Windows.Graphics.SizeInt32(settingsWindowStartWidth, settingsWindowStartHeight));

               this.AppWindow.Changed += SettingsWindow_Changed;

               LoadLocalFilePath();

          }

          private void LoadLocalFilePath()
          {
               LocalFolderPathTextbox.Text = ApplicationData.Current.LocalFolder.Path;
          }

          private void SaveSettings_Click( object sender, RoutedEventArgs e )
          {
               App.UserSettings.Theme = "Dark";
               SaveSettings();
          }

          private async Task SaveSettings()
          {
               await SettingsService.SerializeUserSettings_Async();
          }

          private void CloseSettingsWindow_Click( object sender, RoutedEventArgs e )
          {
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


     }
}

