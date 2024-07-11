Major Issues:

​	List View "flashes" when being refreshed. Very annoying on the eyes.

XAML Errors

Custom Button: Border Brush

​	Theme Resource Line for Border Brush is commented out until further resolution.



10 July 2024 Refactor:

I am looking at LoadLastSession()



but it has the code for LoadPalette built in.

This needs to get moved to a LoadPalette i nthe code behind that will send whatever path to the file service to loadpaletteasync.



then whether it is last session or open palette file select I can send the file path

in last session use LastOpenedFilePath from appconstants; if it is open from picker then assign LastOpenedFilePath in appconstants.



this keeps ui / colorpalettedata loding in code behind (for now) and returns the full palette from file.



