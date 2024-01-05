echo "Building ToolChecker"

cd ToolChecker\bin\Debug
..\..\..\tools\ILRepack.exe /out:"..\..\..\out\ToolChecker.exe" "ToolChecker.exe" "Common.dll" "Common.ConfigProvider.dll"

cd ..\..\..\

pause
