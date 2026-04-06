dotnet build ..\

REM Prep releaseFiles directory by making sure its empty
del /s /q .\releaseFiles\*
rmdir /s /q .\releaseFiles\
mkdir .\releaseFiles\

REM Copy metadata files to the releaseFiles directory
copy /y ..\Art\icon.png .\releaseFiles\
copy /y ..\Art\manifest.json .\releaseFiles\
copy /y ..\Art\README.md .\releaseFiles\
copy /y ..\CHANGELOG.md .\releaseFiles\

REM Copy the built dll to the releaseFiles directory
xcopy /s /y /q ..\CounterHeads\bin\Debug\netstandard2.1\games.enchanted.CounterHeads.dll .\releaseFiles\

REM Create a zip file named CounterHeads.zip containing all files (except build.bat) in the current directory
"C:\Program Files\7-Zip\7z.exe" a CounterHeads.zip .\releaseFiles\* -x!build.bat -x!CounterHeads.zip -x!\releaseFiles\
