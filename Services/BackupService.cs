using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ColorPaletteBuilder
{
    internal class BackupService
    {
        
        private static async Task<StorageFile> GetBackupFileAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFolder backupFolder = await localFolder.CreateFolderAsync("BackupFiles", CreationCollisionOption.OpenIfExists);
            StorageFile backupFile = await backupFolder.CreateFileAsync(AppConstants.BackupFileName, CreationCollisionOption.ReplaceExisting);
            return backupFile;
        }

        internal static async Task<AppConstants.ReturnCode> SaveBackupAsync(ColorPalette colorPaletteData)
        {
            try
            {
                StorageFile backupFile = await GetBackupFileAsync();
                await FileService.SavePalette_Async(backupFile.Path, colorPaletteData);
                return AppConstants.ReturnCode.Success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving backup: {ex.Message}");
                return AppConstants.ReturnCode.FileWriteError;
            }
        }

        internal static async Task<ColorPalette> LoadBackupAsync()
        {
            try
            {
                StorageFile backupFile = await GetBackupFileAsync();
                return await FileService.LoadPaletteFile_Async(backupFile.Path);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading backup: {ex.Message}");
                return null;
            }
        }
    }
}
