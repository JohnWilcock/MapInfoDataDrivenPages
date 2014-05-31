MapInfoDataDrivenPages
======================

Simple data driven pages implementation for MapInfo

Similar to Arc Data Driven pages, although limitations of MapInfo prevent adding the flexibility of data driven def queries.  I've tried to make up for this somewhat by adding page driven queries.
Precompiled version Works with mapinfo v11.5.2 and above. 
if you compile it yourself it works with 11.5.0 + (unfortnutly PBBI only has 11.5.2 and above for download on thier website so i can't pre-compile and earlier version)


WARNING - NOT TESTED on V12.5 or x64. These versions are having very big changes to them and this extention may need fixes after thier release (feel free to fix yourself on github if your happy to do so) 

Latest:
Fixed XML dockinfo error. occured where MI was looking for the appdata folder of the version it was compiled against (11.5) it should have been looking for the appdata folder of the version running (systeminfo(3))
