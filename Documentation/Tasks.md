# Task List for Implementation

## Ongoing and Upcoming Tasks

### Main Page Features

#### General Notes

- **Update the title bar file name**: When a palette is saved, ensure the title bar updates.
- **Improve color picker file handling**: Add better checks for file loads and handle improper image files by presenting buttons to browse or drag/drop a file.
- **Thumbnail of current image in toolbar**: Consider removing the file name in favor of a thumbnail.
- **Scroll functionality**: Implement proper scrolling for the color picker column and overall application.
- **Title bar message for filter selections**: Format as `Filter: state & group`, e.g., `Filter: Enabled & Any`.
- **Note item for Color Entry**: Add a note field to color entries.
- **Backup file handling**: Ensure the application prompts to save as a new file if a backup is loaded.
- **Hex code validation**: Ensure the hex code in color entries follows the proper pattern, with an option to override if necessary.
- **Review auto-save and save on close**: Address scenarios where saving prompts are inconsistent.
- ColorPicker needs a clear image button

#### Settings Notes

- **Save and load settings**: Implement functionality to save and use settings.
- **Theme selection**: Add proper theme selection options.
- **Color format preference**: Allow users to set their preferred color format for saved entries, including an option to include/exclude the hashtag for hex codes.
- **Word wrap in ListView**: Add a setting to toggle word wrap for name and note fields.

### Features Notes

- **Undo Tree implementation**: Add undo functionality for each element of a color entry.
- **Separate undo for the color picker**: Implement dedicated undo functionality for the color picker.
- **Recent colors list for the color picker**: Add a list of recent colors, with both auto-added and manually selected options.
- **Paste from clipboard**: Allow users to paste from the clipboard, such as screenshots.
- **Assign color to color picker**: Add functionality to set the color picker to a color entry's color for adjustments.
- **Sort by hex**: Add user settings for sorting methods, including light-to-dark and group sorting options.
- **Saved color code format**: Make the saved color code format (e.g., hex with alpha, HSV, RGB) a user setting.

### User Interface Enhancements

- **Adaptive layout**: Implement an adaptive layout for different screen sizes and resolutions.
- **Tooltips**: Add tooltips for all interactive elements.

### Additional Features for Usability and Customization

#### Default Palette Location

- **Set default folder for palettes**: Allow users to set a default folder for saving and opening palettes.
- **Folder picker in settings**: Add a folder picker for this setting.

#### Recent Palettes

- **List of recently opened palettes**: Display recently opened palettes for quick access.
- **Clear recent palettes history**: Implement functionality to clear the history.

#### Auto-Save Location

- **Default location for auto-saving**: Specify a default location for auto-saving temporary palettes.
- **Option in settings**: Add an option for this feature in the settings window.

#### Color Format Preference

- **Preferred color format**: Allow users to select their preferred color format (Hex, RGBA, HSLA).
- **ComboBox in settings**: Add a ComboBox for this preference.

#### Palette Sorting Options

- **Sorting of colors in the palette**: Enable sorting by name, color value, or custom order.
- **Sorting options in settings**: Add sorting options in the settings window.

#### Color Preview Size

- **Size of color previews**: Let users choose the size of color previews (small, medium, large).
- **Slider or ComboBox in settings**: Implement a slider or ComboBox for this setting.

#### Export Options

- **Default export formats**: Provide options for default export formats (e.g., JSON, XML, CSV).
- **Export options in settings**: Add these options in the settings window.

#### Backup and Restore

- **Automatic backups**: Enable automatic backups of palettes and settings.
- **Restore from backups**: Provide a way to restore palettes and settings from backups.

#### Notification Settings

- **Manage notifications**: Allow users to manage notifications for different events (e.g., palette saved, export completed).
- **Checkboxes in settings**: Add checkboxes for each notification type.

#### Undo/Redo History Limit

- **Set maximum number of undo/redo actions**: Implement a limit on the number of actions stored.
- **Slider in settings**: Add a slider for this setting.

#### Language Preference

- **Change application language**: Allow users to change the language of the application.
- **ComboBox in settings**: Add a ComboBox for language selection.

#### Theme and Appearance

- **Customize UI theme and font size**: Allow users to customize the UI theme (light, dark, custom) and font size.
- **Options in settings**: Add these options in the settings window.

#### Performance Optimization

- **Optimize for performance**: Ensure the application performs well with large color palettes.
- **Smooth UI interactions**: Ensure smooth UI interactions and fast loading times.

### Code Organization and Refactoring

#### Service Layer

- **Create a service layer**: Handle file operations, settings management, and color conversions in a dedicated service layer.
- **Move methods to services**: Move relevant methods from `MainWindow.xaml.cs` to appropriate services.

#### MVVM Implementation

- **Implement MVVM pattern**: Create ViewModels for better separation of concerns.
- **Bind UI elements to ViewModel properties**: Ensure UI elements are bound to ViewModel properties and commands.

#### Dependency Injection

- **Implement dependency injection**: Use a dependency injection container for service registration and resolution.

### Testing and Validation

#### Unit Testing

- **Write unit tests**: Ensure high code coverage and test for edge cases.

#### UI Testing

- **Automated UI tests**: Validate user interactions and test for accessibility compliance and usability.

#### Performance Testing

- **Conduct performance tests**: Ensure the application performs well under load and optimize as necessary.

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