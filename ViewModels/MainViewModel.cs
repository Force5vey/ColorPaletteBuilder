using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorPaletteBuilder
{
    internal class MainViewModel
    {

        internal async Task<ContentDialogResult> ShowSaveFileDialogAsync()
        {
            var saveFileDialog = new ContentDialog
            {
                Title = "Unsaved Changes",
                Content = "Do you want to save your changes?",
                PrimaryButtonText = "Save",
                SecondaryButtonText = "No",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary
            };

            return await saveFileDialog.ShowAsync();
        }


    }
}
