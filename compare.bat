@echo off
REM D365 Solution Comparator - Windows Batch Script
REM This script makes it easier to run the comparator

echo.
echo ╔═══════════════════════════════════════════════════════════╗
echo ║      D365 Solution Comparator                            ║
echo ╚═══════════════════════════════════════════════════════════╝
echo.

REM Check if .NET is installed
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ❌ Error: .NET SDK is not installed or not in PATH
    echo.
    echo Please install .NET 8.0 SDK from:
    echo https://dotnet.microsoft.com/download/dotnet/8.0
    echo.
    pause
    exit /b 1
)

REM Check for arguments
if "%~1"=="" (
    echo Usage: compare.bat ^<source1^> ^<source2^> [output]
    echo.
    echo Examples:
    echo   compare.bat solution1.zip solution2.zip
    echo   compare.bat solution1.zip solution2.zip report.html
    echo   compare.bat "C:\Solutions\Dev" "C:\Solutions\Prod"
    echo.
    pause
    exit /b 1
)

if "%~2"=="" (
    echo ❌ Error: Two solution paths are required
    echo.
    echo Usage: compare.bat ^<source1^> ^<source2^> [output]
    echo.
    pause
    exit /b 1
)

REM Set default output if not provided
set OUTPUT=%~3
if "%OUTPUT%"=="" set OUTPUT=comparison-report.html

echo Running comparison...
echo Source 1: %~1
echo Source 2: %~2
echo Output:   %OUTPUT%
echo.

REM Run the comparator
dotnet run --project "%~dp0D365SolutionComparator.csproj" -- --source1 "%~1" --source2 "%~2" --output "%OUTPUT%"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✅ Comparison completed successfully!
    echo.
    echo Opening report in default browser...
    start "" "%OUTPUT%"
) else (
    echo.
    echo ❌ Comparison failed. Check the error messages above.
    echo.
)

pause
