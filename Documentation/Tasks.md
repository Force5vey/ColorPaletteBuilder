## Task List for Implementation

### Main Page Features

#### General Notes
- [ ] Auto-copy buttons below color picker
- [ ] update the title bar file name if a palette is saved as and other scenarios.
- [ ] fix the lock toggle, so it stays accurate.
  - [ ] with undo implemented, rethink needing this lock.

- [ ] color picker needs better checking for file loads
- [ ] color picker needs to handle not having a proper image file by loading and presenting buttons to browse or a spot drag / drop a file.
- [ ] thumbnail of image in toolbar for whichever image is currently there.
  - [ ] this might involve just getting rid of the file name and having the thumbnail.

- [ ] scroll functionality for the color picker column as well as over all proper scrolling.
- [ ] title bar message for the filter selections should be Filter: state & group so it would be Filter: Enabled & Any
- [ ] Color Entry needs a note item
- [ ] if the backup is loaded, ensure it asks to save to new file.

#### Settings Notes

- [ ] Save the settings as well as actually load and use them
- [ ] Proper theme selection
- [ ] Color format preference sets what is saved in the color code entry
  - [ ] allow to tick off the hashtag for hex copy

- [ ] 

#### Features Notes

- [ ] Undo Tree implementation
  - [ ] this should be for each of the elements of a color entry.
- [ ] a separate undo for the color picker specifically
- [ ] Recent colors list for the color picker
  - [ ] row of auto added that just filters through by adding to newest and subtracting to oldest
  - [ ] row of manually selected, a 'temp' spot in sorts.
- [ ] Paste from clipboard, from say a screen shot.
- [ ] opposite of the assign current to existing color button, do an assign to the color picker the color from the color entry.
  - [ ] This is so if you want to adjust slightly the saved color it can auto set the color picker, make the slight adjustment, then reassign.






#### Color Picker Integration
- [ ] Implement the color picker and ensure it updates the current color entry.
- [ ] Ensure the color picker supports RGB, Hex, and RGBA formats.
- [ ] Add buttons for copying color values in different formats.

#### Color Palette Management
- [ ] Implement functionality to add, remove, and edit color entries.
- [ ] Enable saving and loading color palettes from files.
- [ ] Implement auto-save functionality with user-defined intervals.

#### Element Groups and States
- [ ] Allow users to create and manage element groups and states.
- [ ] Ensure ComboBoxes for element groups and states are properly populated and editable.

#### ListView Item Interaction
- [ ] Implement functionality to copy hex codes to clipboard.
- [ ] Add the ability to assign the current color to a color entry.
- [ ] Ensure ListView updates appropriately when color entries are modified.

### Settings Window
- [ ] Create a settings window to manage application settings.
- [ ] Ensure the settings window is modal and blocks interaction with the main window.
- [ ] Add functionality to save and load settings.

### Screen Color Picker
- [ ] Implement an eye-dropper tool to pick colors from the screen.
- [ ] Ensure the tool works across different displays and resolutions.

### User Interface Enhancements
- [ ] Implement an adaptive layout for different screen sizes and resolutions.
- [ ] Add tooltips for all interactive elements.
- [ ] Ensure accessibility features such as keyboard navigation and screen reader support.

### Additional Features for Usability and Customization

#### Default Palette Location
- [ ] Allow users to set a default folder for saving and opening palettes.
- [ ] Add a folder picker in the settings window for this purpose.

#### Recent Palettes
- [ ] Display a list of recently opened palettes for quick access.
- [ ] Implement functionality to clear recent palettes history.

#### Auto-Save Location
- [ ] Specify a default location for auto-saving temporary palettes.
- [ ] Add an option in the settings window for this feature.

#### Color Format Preference
- [ ] Allow users to select their preferred color format (Hex, RGBA, HSLA).
- [ ] Add a ComboBox in the settings window for this preference.

#### Palette Sorting Options
- [ ] Enable sorting of colors in the palette by name, color value, or custom order.
- [ ] Add sorting options in the settings window.

#### Color Preview Size
- [ ] Let users choose the size of color previews in the palette (small, medium, large).
- [ ] Implement a slider or ComboBox in the settings window for this setting.

#### Export Options
- [ ] Provide options for default export formats (e.g., JSON, XML, CSV).
- [ ] Add export options in the settings window.

#### Backup and Restore
- [ ] Enable automatic backups of palettes and settings.
- [ ] Provide a way to restore palettes and settings from backups.

#### Notification Settings
- [ ] Manage notifications for different events (e.g., palette saved, export completed).
- [ ] Add checkboxes in the settings window for each notification type.

#### Undo/Redo History Limit
- [ ] Set the maximum number of undo/redo actions stored.
- [ ] Implement a slider in the settings window for this setting.

#### Language Preference
- [ ] Allow users to change the language of the application.
- [ ] Add a ComboBox in the settings window for language selection.

#### Theme and Appearance
- [ ] Allow users to customize the UI theme (light, dark, custom) and font size.
- [ ] Add options in the settings window for theme and font size preferences.

#### Performance Optimization
- [ ] Optimize the application for performance with large color palettes.
- [ ] Ensure smooth UI interactions and fast loading times.

### Code Organization and Refactoring

#### Service Layer
- [ ] Create a service layer for handling file operations, settings management, and color conversions.
- [ ] Move relevant methods from MainWindow.xaml.cs to appropriate services.

#### MVVM Implementation
- [ ] Implement the Model-View-ViewModel (MVVM) pattern for better separation of concerns.
- [ ] Create ViewModels for the main window and settings window.
- [ ] Bind UI elements to ViewModel properties and commands.

#### Dependency Injection
- [ ] Implement dependency injection to manage services and ViewModels.
- [ ] Use a dependency injection container for service registration and resolution.

### Testing and Validation

#### Unit Testing
- [ ] Write unit tests for services and ViewModels.
- [ ] Ensure high code coverage and test for edge cases.

#### UI Testing
- [ ] Implement automated UI tests to validate user interactions.
- [ ] Test for accessibility compliance and usability.

#### Performance Testing
- [ ] Conduct performance tests to ensure the application performs well under load.
- [ ] Optimize code and UI for better performance.