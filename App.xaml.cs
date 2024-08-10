using Microsoft.UI.Xaml;
using System.Drawing;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.UI.Xaml.Media;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ColorPaletteBuilder
{
     /// <summary>
     /// Provides application-specific behavior to supplement the default Application class.
     /// </summary>
     public partial class App :Application
     {
          private Window m_window;

          internal static WriteableBitmap ColorSelectorBitmap;
          internal static Color ColorSelectorColor;

          internal static UserSettings UserSettings { get; set; } = new UserSettings();

          /// <summary>
          /// Initializes the singleton application object.  This is the first line of authored code
          /// executed, and as such is the logical equivalent of main() or WinMain().
          /// </summary>
          public App()
          {
               this.InitializeComponent();
          
               InitializeApp_Async();
               
               RequestedTheme = UserSettings.Theme;

          }

          /// <summary>
          /// Invoked when the application is launched.
          /// </summary>
          /// <param name="args">Details about the launch request and process.</param>
          protected override void OnLaunched( Microsoft.UI.Xaml.LaunchActivatedEventArgs args )
          {
               m_window = new MainWindow();
               m_window.ExtendsContentIntoTitleBar = true;

               m_window.Activate();
          }

          private async void InitializeApp_Async()
          {
               bool settingsLoaded = await SettingsService.DeserializeUserSettings_Async();
            if ( !settingsLoaded)
            {
                    Debug.WriteLine("Failed to load User Settings, in app.xaml.cs in InitializeApp_Async");
            }
        }

     }
}
