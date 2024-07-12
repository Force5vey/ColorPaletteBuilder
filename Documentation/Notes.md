Major Issues:

​	List View "flashes" when being refreshed. Very annoying on the eyes.

XAML Errors

Custom Button: Border Brush

​	Theme Resource Line for Border Brush is commented out until further resolution.



Clicking new doesn't update the title bar title

clicking save didn't save the new blank color paltette nor did it trigger a save as dialog box

auto save after clicking new returns an error in mainView at the await storage file line.mostlikely a null filePath is being sent.

​	walk through the process of:

​		New

​		resetting all of the colorpalette data

​		resetting the titlbar title

​		resetting the save file etc to New Palette

​		button save click event and where it links up first with auto save timer tick

​		where the filepath is being derived

​		button click needs to check for New Pallete to ask to save as (this needs to be updated to a tracker variable. a new palette is HasBeenSaved to false, then first save will be True, this will be checked but also check filepath etc for completeness)

​	aut save needs to check against this and not save, however backup can still be saved to default back up location.

