using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Security.Cryptography.X509Certificates;
using Microsoft.UI.Xaml;

namespace ColorPaletteBuilder
{
     internal class SettingsViewModel
     {
          public int ThemeSelection {  get; set; }

          public SettingsViewModel()
          {
               if ( App.UserSettings.Theme == ApplicationTheme.Light )
               {
                    ThemeSelection = 0;
               }
               else if(App.UserSettings.Theme == ApplicationTheme.Dark ) 
               {
                    ThemeSelection = 1;
               }
          }

          public async Task SaveSettings()
          {
               switch ( ThemeSelection )
               {
                    case 0: //Light
                    {
                         App.UserSettings.Theme = ApplicationTheme.Light;

                         break;
                    }
                    case 1: //Dark
                    {
                         App.UserSettings.Theme = ApplicationTheme.Dark;
                         break;
                    }
               }

               await SettingsService.SerializeUserSettings_Async();
          }
     }
}
