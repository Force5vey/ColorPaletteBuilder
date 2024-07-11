using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Linq.Expressions;
using System.Diagnostics;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;


namespace ColorPaletteBuilder
{
     public static class FileService
     {
          public static async Task SavePalette_Async( string filePath, ColorPalette colorPaletteData )
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

          public static async Task<ColorPalette> LoadPaletteFile_Async( string filePath )
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

          internal static async Task<WriteableBitmap> LoadDefaultColorSelectorImage()
          {
               try
               {
                    StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/ColorSelectorDefaultImage.png"));
                    using ( IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read) )
                    {
                         BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);
                         WriteableBitmap bitmap = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                         await bitmap.SetSourceAsync(fileStream);
                         return bitmap;
                    }
               }
               catch
               {
                    return null;
               }
          }

          internal static string GetLocalSettings(string settingString)
          {
               var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

               if ( localSettings.Values.TryGetValue(settingString, out object setting) )
               {
                    string settingValue = setting as string;
                    if ( !string.IsNullOrEmpty(settingString) && File.Exists(settingValue) )
                    {
                         return settingValue;
                    }
               }
               return null;
          }

          internal static bool IsImageFile(StorageFile file)
          {
               var imageFileTypes = new[] { ".bmp", ".jpg", ".jpeg", ".png" };
               return imageFileTypes.Contains(file.FileType.ToLower());
          }


     }


}
