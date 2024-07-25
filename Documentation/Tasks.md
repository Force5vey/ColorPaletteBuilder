# Task List for Implementation

## Ongoing and Upcoming Tasks

### Main Page Features

#### General Notes

- When no usersettings.json file is present, the application hangs. closing it then opening again ti is good because a usersettings file is there (i'm thinking) looks like it has something to do with the await aspect.
  - its happening in the 'else' bracket of settingsService.DeserializeUserSettings();
- colorselector window: gui zoom in / out buttons.
- Adding a new entry auto sets state and group based on current filter. Not only for entry convenience but when a filter is on and an entry is added it is initially hidden.
- It starts to get pretty laggy after about 20 colors

### Features Notes

- **Undo Tree implementation**: Add undo functionality for each element of a color entry.
- **Separate undo for the color picker**: Implement dedicated undo functionality for the color picker.
- **Recent colors list for the color picker**: Add a list of recent colors, with both auto-added and manually selected options.
- **Paste from clipboard**: Allow users to paste from the clipboard, such as screenshots.
- Camera interface to take photo and assign to colorselector
- add a browser (webview2) to get image for color selector

### User Interface Enhancements

- **Tooltips**: Add tooltips for all interactive elements.
- Theme: For none system colors, need to ensure the light theme is set (example TitleBarMessage Color)
- Theme: Light theme is having some issues with some of the built in features like minimize / maximize buttons, need to look over anywhere I have custom brushes or colors set.

### Additional Features for Usability and Customization

#### Recent Palettes

- **Clear recent palettes history**: Implement functionality to clear the history.

#### Backup and Restore

- **Restore from backups**: Provide a way to restore palettes and settings from backups.