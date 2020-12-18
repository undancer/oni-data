@echo off
echo ===============
echo NOTE!!!!!!!!!!!
echo ===============
echo Grab the latest .pot from the build machine, the one with the revision number in the name. Drag that on this script to update the .po files with the latest source strings.
echo .
echo .
echo TO USE: Drag the .pot file on to this batch file. New .po files will be create with the revisions.
echo .
echo Finally: This uses the GNU gettext utilities.. if you don't have those installed, it won't work!
@echo on
python.exe -u update_from_pot.py %1