# Task List for Implementation

## Completed and logged for posterity.

### Main Page Features

#### General Notes

- Nullable type set to true, review types for nullbility. colorpalette.cs for example shouldn't be null and needs to be properly set up wit hits constructor etc. _ reveiw CPB_Build_11
- Might take a new approach just to ensure proper null handling.
- **Update the title bar file name**: When a palette is saved, ensure the title bar updates.
- **Thumbnail of current image in toolbar**: Consider removing the file name in favor of a thumbnail.
- **Scroll functionality**: Implement proper scrolling for the color picker column and overall application.
- **Note item for Color Entry**: Add a note field to color entries.
- **Review auto-save and save on close**: Address scenarios where saving prompts are inconsistent.
- *complete* Color Selector needs a clear image button
- Auto-save doesn't seem to be triggering. noticed case: app loads and loads last file. doesn't trigger it.
  - Ok, so it is triggered, its just that it is only saving the backupfile. This can be reviewed because it autosaves a backup but not the current file an this could be beneficial to give the option to make a bunch of changes, do a save as, and preserve the original file from when it was last saved. think of MS Word, and it autosaves all the time and then when you realized you wanted to keep the original and save a copy it is messd up from the autosave.
- Clicking new, then adding and then saving doesn't set the title bar to the file name.
- **Title bar message for filter selections**: Format as `Filter: state & group`, e.g., `Filter: Enabled & Any`.
- **Assign color to color picker**: Add functionality to set the color picker to a color entry's color for adjustments.
- **Settings** are saved but need to be implemented in logic to use them.
- **Improve color picker file handling**: Add better checks for file loads and handle improper image files by presenting buttons to browse or drag/drop a file.
- Color hex codes button shortcuts need a user settings option to decide if it includes the hashtag or not --- double clicking to highight and paste over works better without including the hashtag.
- clearing colorselector doesn't update the color selector window
- **Backup file handling**: Ensure the application prompts to save as a new file if a backup is loaded.
  - this works as long as the file was a new file and not prior to saving, then 
- **Wrap Text:** wrap text usersetting is declared but needs to be integrated into the listview dispaly to change the wrap text. typical binding is not working and it seems it needs to be propagated down to the color entry items individually as a TextWrapping type so it can set the textbox for each row.
- 


Settings

- Save and Load User Settings
- **Theme selection**: Add proper theme selection options.
- **Color format preference**: Allow users to set their preferred color format for saved entries, including an option to include/exclude the hashtag for hex codes.
  - Complete, because I decided to keep the standard for the save file to be the hex code. any other format will be through the color selector converters.


#### Default Palette Location

- **Set default folder for palettes**: Allow users to set a default folder for saving and opening palettes.
- **Folder picker in settings**: Add a folder picker for this setting.

#### Palette Sorting Options

- **Sorting of colors in the palette**: Enable sorting by name, color value, or custom order.



## Completed Tasks

### Main Page Features

- **Auto-copy buttons below color picker**: Implemented buttons for copying color values in different formats.
- **List viewer scroll viewer**: Transitioned the built-in scroll viewer to a standalone one to incorporate header buttons for sorting.

### Color Picker Integration

- **Implemented the color picker**: Ensured it updates the current color entry and supports RGB, Hex, and RGBA formats.

### Color Palette Management

- **Added, removed, and edited color entries**: Implemented basic color entry management functionality.
- **Enabled saving and loading color palettes**: Added functionality to save and load palettes from files.
- **Implemented auto-save functionality**: Allows user-defined intervals for auto-saving.

### Element Groups and States

- **Managed element groups and states**: Allowed users to create and manage groups and states.
- **Populated ComboBoxes**: Ensured ComboBoxes for element groups and states are properly populated and editable.

### ListView Item Interaction

- **Copied hex codes to clipboard**: Added functionality to copy hex codes to clipboard.
- **Assigned current color to a color entry**: Enabled assigning the current color from the picker to a color entry.
- **ListView updates**: Ensured ListView updates appropriately when color entries are modified.

### Settings Window

- **Created a settings window**: Developed a window to manage application settings.

### Screen Color Picker

- **Implemented an eye-dropper tool**: Added functionality to pick colors from the screen.
- **Ensured tool functionality across displays**: Ensured the tool works across different displays and resolutions.

