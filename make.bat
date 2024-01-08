@echo off
setlocal enabledelayedexpansion

REM Check if the folder name is provided as a parameter
if "%~1"=="" (
  echo Please provide the folder name as a parameter.
  echo Usage: make.bat [FolderName]
  goto :eof
)

REM Folder name from the input parameter
set "folderName=%~1"

REM Path to the source folder containing exe and dlls
set "sourceDir=%folderName%\bin\debug"

REM Path to the output directory
set "outputDir=out"

REM Initialize variables
set "primary="
set "secondary="

REM Change the directory to the source folder
cd /d "%sourceDir%"

REM Loop through files to find the first exe and all dlls
for %%f in (*.exe *.dll) do (
  if not defined primary (
    if "%%~xf"==".exe" set "primary=%%f"
  ) else (
    if "%%~xf"==".dll" set "secondary=!secondary! %%f"
  )
)

REM Run ILRepack
if defined primary (
  ILRepack.exe /out:"%outputDir%\%primary%" %primary% %secondary%
  echo Merged %primary% with %secondary% into %outputDir%\%primary%
) else (
  echo No executable file found in %sourceDir%.
)

endlocal
