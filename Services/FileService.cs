using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;


namespace ColorPaletteBuilder
{
    public static class FileService
    {
        public static async Task SavePaletteAsync(string filePath, IEnumerable<ColorEntry> colorEntries)
        {
            var json = JsonSerializer.Serialize(colorEntries);
            await File.WriteAllTextAsync(filePath, json);
        }

        public static async Task<IEnumerable<ColorEntry>> LoadPaletteAsync(string filePath)
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<IEnumerable<ColorEntry>>(json);
        }
    }
}
