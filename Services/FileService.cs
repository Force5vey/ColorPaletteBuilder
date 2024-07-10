using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Linq.Expressions;
using System.Diagnostics;


namespace ColorPaletteBuilder
{
     public static class FileService
     {
          public static async Task SavePaletteAsync( string filePath, ColorPalette colorPaletteData )
          {
               try
               {
                    var options = new JsonSerializerOptions
                    {
                         WriteIndented = true
                    };

                    var json = JsonSerializer.Serialize(colorPaletteData, options);
                    await File.WriteAllTextAsync(filePath, json);
               }
               catch ( Exception ex )
               {
                    Debug.WriteLine($"Error saving file: {ex.Message}");
               }

          }

          public static async Task<ColorPalette> LoadPaletteAsync( string filePath )
          {
               try
               {
                    var json = await File.ReadAllTextAsync(filePath);
                    return JsonSerializer.Deserialize<ColorPalette>(json);
               }
               catch ( Exception ex )
               {
                    Debug.WriteLine($"Error loading file: {ex.Message}");
                    return null;
               }
          }
     }

   
}
