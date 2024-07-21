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

Settings

- Save and Load User Settings
- **Theme selection**: Add proper theme selection options.
- **Color format preference**: Allow users to set their preferred color format for saved entries, including an option to include/exclude the hashtag for hex codes.
  - Complete, because I decided to keep the standard for the save file to be the hex code. any other format will be through the color selector converters.


