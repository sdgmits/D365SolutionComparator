# Quick Start Guide - Building and Distributing

## For Developers: Building the Standalone Executable

### Windows Users

Simply run:
```batch
publish-windows.bat
```

The executable will be created in `publish\windows-x64\D365SolutionComparator.exe`

### Cross-Platform Build (All Platforms at Once)

Run from Windows:
```batch
publish-all.bat
```

This creates executables for:
- Windows (x64)
- Linux (x64)
- macOS Intel (x64)
- macOS Apple Silicon (ARM64)

## For End Users: Running the Tool

### No Installation Required!

The standalone executable includes everything needed to run - no .NET installation required.

### Windows
1. Extract the ZIP file
2. Open Command Prompt or PowerShell
3. Navigate to the extracted folder
4. Run:
   ```batch
   D365SolutionComparator.exe -s1 solution1.zip -s2 solution2.zip
   ```

### Linux/macOS
1. Extract the archive
2. Open Terminal
3. Navigate to the extracted folder
4. Make executable (first time only):
   ```bash
   chmod +x D365SolutionComparator
   ```
5. Run:
   ```bash
   ./D365SolutionComparator -s1 solution1.zip -s2 solution2.zip
   ```

## Example Usage

```bash
# Compare two solution ZIP files
D365SolutionComparator.exe -s1 "DevSolution_1_0_0_1.zip" -s2 "ProdSolution_1_0_0_2.zip"

# Compare and specify output file
D365SolutionComparator.exe -s1 "solution1.zip" -s2 "solution2.zip" -o "my-report.html"

# Compare with verbose logging
D365SolutionComparator.exe -s1 "solution1.zip" -s2 "solution2.zip" --verbose

# Show help
D365SolutionComparator.exe --help
```

## Distribution Checklist

✅ Executable size: ~35-40 MB (normal for self-contained apps)  
✅ No dependencies required  
✅ Works on systems without .NET installed  
✅ Single executable file (+ optional PDB for debugging)  
✅ Compatible with Windows 10/11, Linux (various distros), macOS

## Troubleshooting

### "Permission denied" on Linux/macOS
Run: `chmod +x D365SolutionComparator`

### "Cannot be opened because it is from an unidentified developer" on macOS
Right-click → Open → Click "Open" in the security dialog

### Antivirus false positives
Self-contained .NET apps may trigger false positives. Add an exception if needed.

## File Size Considerations

The executable is larger (~35MB) because it includes:
- .NET 8.0 runtime
- All required libraries
- Application code

This is normal and expected for self-contained deployments.

## Need More Info?

See [DISTRIBUTION.md](DISTRIBUTION.md) for detailed documentation.
