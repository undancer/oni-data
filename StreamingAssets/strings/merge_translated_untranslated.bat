@echo off
echo ===============
echo NOTE!!!!!!!!!!!
echo ===============
echo By default the .pot file in this directory is not up-to-date, it's worth grabbing the latest from the build machine or rebuilding it yourself from the editor before running this file.
echo .
echo .
echo TO USE: Drag the pair of .po files that you want to merge on to this batch file. Rename them to the repo name when finished.
echo .
echo Finally: This uses the GNU gettext utilities.. if you don't have those installed, it won't work!
@echo on
msgcat.exe -o merged.po --no-wrap %1 %2 %3 %4

