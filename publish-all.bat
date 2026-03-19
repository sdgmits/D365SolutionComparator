@echo off
echo Building self-contained executables for all platforms...
echo.

REM Clean all previous builds
if exist "publish" rmdir /s /q "publish"
mkdir publish

echo ========================================
echo Building Windows x64...
echo ========================================
dotnet publish D365SolutionComparator.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publish\windows-x64

echo.
echo ========================================
echo Building Linux x64...
echo ========================================
dotnet publish D365SolutionComparator.csproj -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publish\linux-x64

echo.
echo ========================================
echo Building macOS x64 (Intel)...
echo ========================================
dotnet publish D365SolutionComparator.csproj -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publish\osx-x64

echo.
echo ========================================
echo Building macOS ARM64 (Apple Silicon)...
echo ========================================
dotnet publish D365SolutionComparator.csproj -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publish\osx-arm64

echo.
echo ========================================
echo Build Summary
echo ========================================
if exist "publish\windows-x64\D365SolutionComparator.exe" (
    echo [OK] Windows x64: publish\windows-x64\D365SolutionComparator.exe
) else (
    echo [FAILED] Windows x64
)

if exist "publish\linux-x64\D365SolutionComparator" (
    echo [OK] Linux x64: publish\linux-x64\D365SolutionComparator
) else (
    echo [FAILED] Linux x64
)

if exist "publish\osx-x64\D365SolutionComparator" (
    echo [OK] macOS x64: publish\osx-x64\D365SolutionComparator
) else (
    echo [FAILED] macOS x64
)

if exist "publish\osx-arm64\D365SolutionComparator" (
    echo [OK] macOS ARM64: publish\osx-arm64\D365SolutionComparator
) else (
    echo [FAILED] macOS ARM64
)

echo.
echo All builds completed! Check the publish folder for distributables.
pause
