Major Issues:

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
