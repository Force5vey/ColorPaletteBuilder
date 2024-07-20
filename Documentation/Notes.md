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





---

still working on settings, but doing specific things with binding settingsviewmodel with singleton usersettings in app. right now there is some basic binding but it is with viewmodel prperties taht aren't properly translating to the singleton usersettings so in the gpt CPB_Build_13 go back through what the latest process is. basically, crate the singleton. create a variable / property in settings viewmodel. in the constructor assign this viewmodel property the singleton values. then do properties to get and set to the singleton then update bindings to ensure it is referencing correct properties.



review the questions I had for udnerstanding. I need to do all of last. I am stopping in a state of things workig but not doing anything because it is essentially just UI binding and it isn't translating to the underlying model..
