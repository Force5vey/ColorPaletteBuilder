using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using System.Drawing;
using System.IO;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Capture;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using System.Numerics;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Graphics.DirectX;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;


namespace ColorPaletteBuilder
{
     public class ColorSelectorProcessor
     {

          private static WriteableBitmap ConvertBitmapToWriteableBitmap( Bitmap bitmap )
          {
               WriteableBitmap writeableBitmap = new WriteableBitmap(bitmap.Width, bitmap.Height);

               using ( MemoryStream memoryStream = new MemoryStream() )
               {
                    // Save the bitmap to a memory stream
                    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Load the memory stream into the WriteableBitmap
                    writeableBitmap.SetSource(memoryStream.AsRandomAccessStream());
               }

               return writeableBitmap;
          }

     }
}
