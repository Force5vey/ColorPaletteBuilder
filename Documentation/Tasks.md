# Task List for Implementation

## Ongoing and Upcoming Tasks

### Main Page Features

#### General Notes

- **Wrap Text:** wrap text usersetting is declared but needs to be integrated into the listview dispaly to change the wrap text. typical binding is not working and it seems it needs to be propagated down to the color entry items individually as a TextWrapping type so it can set the textbox for each row.

#### Settings Notes

- **Word wrap in ListView**: Add a setting to toggle word wrap for name and note fields.

### Features Notes

- **Undo Tree implementation**: Add undo functionality for each element of a color entry.
- **Separate undo for the color picker**: Implement dedicated undo functionality for the color picker.
- **Recent colors list for the color picker**: Add a list of recent colors, with both auto-added and manually selected options.
- **Paste from clipboard**: Allow users to paste from the clipboard, such as screenshots.
- Camera interface to take photo and assign to colorselector
- add a browser (webview2) to get image for color selector

### User Interface Enhancements

- **Adaptive layout**: Implement an adaptive layout for different screen sizes and resolutions.
- **Tooltips**: Add tooltips for all interactive elements.
- Theme: For none system colors, need to ensure the light theme is set (example TitleBarMessage Color)
- Theme: Light theme is having some issues with some of the built in features like minimize / maximize buttons, need to look over anywhere I have custom brushes or colors set.

### Additional Features for Usability and Customization

#### Recent Palettes

- **List of recently opened palettes**: Display recently opened palettes for quick access.
- **Clear recent palettes history**: Implement functionality to clear the history.

#### Export Options

- **Default export formats**: Provide options for default export formats (e.g., JSON, XML, CSV).
- **Export options in settings**: Add these options in the settings window.

#### Backup and Restore

- **Restore from backups**: Provide a way to restore palettes and settings from backups.