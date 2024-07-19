Major Issues:

​	List View "flashes" when being refreshed. Very annoying on the eyes.

XAML Errors

Custom Button: Border Brush

​	Theme Resource Line for Border Brush is commented out until further resolution.



need to refactor backupService, it doesn't need to do all the storage folder work, just run it like save palette. Look at file service and see if it really needs to be separated out.

also, SavePaletteToFile_Async also creates a file object, gets file from path (colorpalettedata.fullfilepath) and then only uses the file.path. this may be overly redundant and just use the path.

in save as it is probably necessary because the filepicker returns a file object. then get the path.

Whereas backup is not getting it from a picker (maybe in settings but the full path will be saved) as well as settings it is just getting from the default location or creating a new one.



Settings:

Theme - light dark system default

Enable Auto Save

Auto save interval (seconds)

Default Palette Location:

radio buttons (2) for use system folder or custom

combobox for selecting a default system folder

text box and browse for a custom location



Auto copy preference:

Copy hex code w/ or without hashtag

Code Snippet Language preference (C#, Python, Javascript)

Textblock showing code snippet

