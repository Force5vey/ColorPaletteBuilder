Major Issues:

 I MADE A CHANGE TO A FILE, CLICKED SAVE, IT DID NOT SAVE, I SAVED, CLOSED, OPENED, CHANGES WERE LOST. PRIORITY #1
        note: it seems that the issue for this particular case is that I moved the file from one folder to another, then opened it, and the filename is still the old file location
        but it isn't where it is now. I don't htink there is anything for save where if file not found it creaates a new one sicne htere is a file name in there, unlike unsaved file
        that prompts the save as dialog. so there needs to be a check for file exists and move to save as. which i think there is osmething in there for that but it obviously isn't
        working correctly.

​	List View "flashes" when being refreshed. Very annoying on the eyes.

XAML Errors

Custom Button: Border Brush

​	Theme Resource Line for Border Brush is commented out until further resolution.



need to refactor backupService, it doesn't need to do all the storage folder work, just run it like save palette. Look at file service and see if it really needs to be separated out.

also, SavePaletteToFile_Async also creates a file object, gets file from path (colorpalettedata.fullfilepath) and then only uses the file.path. this may be overly redundant and just use the path.

in save as it is probably necessary because the filepicker returns a file object. then get the path.

Whereas backup is not getting it from a picker (maybe in settings but the full path will be saved) as well as settings it is just getting from the default location or creating a new one.



settings window should be modal



opening a file when there is an unsaved file needs to ask to save, or autosave a 'backup'.

the settings should have a list of backups.

the entire backup system should be a list that maxes at like five so that there can be some recover file history.

opening a file doesn't load the colorselector image, it does when first opening the app though. so it isn't calling the correct image load method on manually opening but it is on app open. 





app images:

lockscreenlogo.scale200

splashscreen.scale-200

square 150x150 scale 200

square 44x44

square 44x44 targetsize 24

storelogo

wide 310x150


Random notes from 27 October 2024:

Settings:
    Theme - Doesn't change theme.
    Not all settings are being saved / loaded / used
        Such as default save location.
    Backup file:
        Last Backup File: This needs to show the file name. The location is shown.
        Restore: This currently doesn't do anything.

Main Window
    Tool Tips: Buttons need tool tips added. Most don't have one. Some do, like in the list view but tool bar buttons and filter buttons do not


Focus on these main issues, then conduct a traceability matrix for testing.


Get windows store publishable package.
    I had issues with this with code trimming etc. It needs to be trimmed, packages and imports varified for use so the file size can not be ridiculous.

