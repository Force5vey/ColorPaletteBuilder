using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ColorPaletteBuilder.Services
{
    internal class BackupService
    {
        private static readonly string backupFileName = "backup.cpb";

        private static async Task<StorageFile> GetBackupFileAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFolder backupFolder = await localFolder.CreateFolderAsync("BackupFiles", CreationCollisionOption.OpenIfExists);
            StorageFile backupFile = await backupFolder.CreateFileAsync(backupFileName, CreationCollisionOption.ReplaceExisting);
            return backupFile;
        }

        internal static async Task<string> SaveBackupAsync(ColorPalette colorPaletteData)
        {
            try
            {
                StorageFile backupFile = await GetBackupFileAsync();
                await FileService.SavePaletteAsync(backupFile.Path, colorPaletteData);
                return "Backup saved successfully";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving backup: {ex.Message}");
                return $"Error saving backup";
            }
        }

        internal static async Task<ColorPalette> LoadBackupAsync()
        {
            try
            {
                StorageFile backupFile = await GetBackupFileAsync();
                return await FileService.LoadPaletteAsync(backupFile.Path);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading backup: {ex.Message}");
                return null;
            }
        }
    }
}
