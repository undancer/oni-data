@echo off
echo ===============
echo NOTE!!!!!!!!!!!
echo ===============
echo By default the .pot file in this directory is not up-to-date, it's worth grabbing the latest from the build machine or rebuilding it yourself from the editor before running this file.
echo .
echo .
echo TO USE: Drag the .po file that you want split on to this batch file. Recommend adding the revision number to the output files when you're done.
echo .
echo Finally: This uses the GNU gettext utilities.. if you don't have those installed, it won't work!
@echo on
msgattrib.exe -o %1.translated.po --no-wrap --translated %1
msgattrib.exe -o %1.untranslated.po --no-wrap --untranslated %1

