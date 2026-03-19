@echo off
echo Building self-contained Windows executable...
echo.

REM Clean previous builds
if exist "publish\windows-x64" rmdir /s /q "publish\windows-x64"

REM Publish for Windows x64
dotnet publish D365SolutionComparator.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publish\windows-x64

echo.
if %ERRORLEVEL% EQU 0 (
    echo Build successful!
    echo Executable location: publish\windows-x64\D365SolutionComparator.exe
    echo.
    echo You can now distribute the entire 'publish\windows-x64' folder.
) else (
    echo Build failed!
)
pause
