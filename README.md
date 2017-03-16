MapInfoDataDrivenPages
======================

Simple data driven pages implementation for MapInfo

Similar to Arc Data Driven pages, although limitations of MapInfo prevent adding the flexibility of data driven def queries.  I've tried to make up for this somewhat by adding page driven queries.
Precompiled version Works with mapinfo v11.5.2 and above. 
if you compile it yourself it works with 11.5.0 + (unfortnutly PBBI only has 11.5.2 and above for download on thier website so i can't pre-compile and earlier version)

Latest:
Fixed XML dockinfo error. occured where MI was looking for the appdata folder of the version it was compiled against (11.5) it should have been looking for the appdata folder of the version running (systeminfo(3))

WARNING/UPDATE 3/2017
64Bit Update - Unfortunalty after some investigation this tool will never be able to work on the new 64bit/ribbon interface versions of MapInfo (e.g. 15.2).  While most aspects of the tool can be upgraded to work this is not the case with the new layout designer. 

The old layout designer used a hidden table called LayoutN to store all objects on the layout; this table behaved in an identical fashion to a normal database table.  When a record (object on the layout) was removed from the layout the contents of the corresponding table row was deleted, but most importantly the row itself and its unique ID was not.  For example if a layout had three objects on it 1,2 and 3; they would be stored in rows 1,2 and 3.  If object 2 was deleted then the remaining objects 1 and 3 would still be stored in the same rows (1&3).  This allowed data driven text to be tracked and updated in the layout window + saved and restored with a little extra effort.
The new layout designer window now identifies objects using a "frame_id", this is wholly dynamic. in the example above, deleting object 2 would result in object 3 being identified as object 2 - therefore there is no way to track what text object page driven text is in.  The only way it could work is if the user never deleted anything from the layout window.
