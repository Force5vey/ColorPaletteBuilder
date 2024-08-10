using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;

namespace ColorPaletteBuilder
{
     internal static class SettingsService
     {
          //User Settings
          internal static async Task SerializeUserSettings_Async()
          {
               try
               {
                    var options = new JsonSerializerOptions
                    {
                         WriteIndented = true
                    };

                    var json = JsonSerializer.Serialize(App.UserSettings, options);
                    await File.WriteAllTextAsync(AppConstants.UserSettingsLocation, json);
               }
               catch ( Exception ex )
               {
                    Debug.WriteLine($"Error saving settings: {ex.Message}");
               }
          }

          public static async Task<bool> DeserializeUserSettings_Async()
          {
               if ( File.Exists(AppConstants.UserSettingsLocation) )
               {
                    try
                    {
                         var json = await File.ReadAllTextAsync(AppConstants.UserSettingsLocation);
                         App.UserSettings = JsonSerializer.Deserialize<UserSettings>(json);

                         return true;
                    }
                    catch ( Exception ex )
                    {
                         Debug.WriteLine($"\n\nError loading settings: {ex.Message}\n\n");
                         return false;
                    }
               }
               else
               {
                    await SerializeUserSettings_Async();

                    Debug.WriteLine($"\n\n Used default settings and resaved.\n\n");
                    return true;
               }
          }

     }
}


