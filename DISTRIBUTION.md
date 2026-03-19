# Distribution Guide

This guide explains how to build and distribute the D365 Solution Comparator as a standalone executable that doesn't require .NET to be installed on the target machine.

## Building Self-Contained Executables

The project is configured to create self-contained, single-file executables that bundle the .NET runtime with the application.

### Option 1: Build for Specific Platform

#### Windows
Run the following script on a Windows machine with .NET SDK installed:
```batch
publish-windows.bat
```
Output: `publish\windows-x64\D365SolutionComparator.exe`

#### Linux
Run the following script on a machine with .NET SDK installed:
```bash
chmod +x publish-linux.sh
./publish-linux.sh
```
Output: `publish/linux-x64/D365SolutionComparator`

#### macOS
Run the following script on a machine with .NET SDK installed:
```bash
chmod +x publish-macos.sh
./publish-macos.sh
```
Outputs:
- Intel Macs: `publish/osx-x64/D365SolutionComparator`
- Apple Silicon Macs: `publish/osx-arm64/D365SolutionComparator`

### Option 2: Build All Platforms at Once

Run on Windows:
```batch
publish-all.bat
```

Or manually for all platforms:
```bash
# Windows x64
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish/windows-x64

# Linux x64
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o publish/linux-x64

# macOS x64 (Intel)
dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true -o publish/osx-x64

# macOS ARM64 (Apple Silicon)
dotnet publish -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true -o publish/osx-arm64
```

## Distribution

### What to Distribute

After building, distribute the appropriate folder for the target platform:

- **Windows users**: `publish\windows-x64\` folder
- **Linux users**: `publish/linux-x64/` folder
- **Mac users (Intel)**: `publish/osx-x64/` folder
- **Mac users (Apple Silicon)**: `publish/osx-arm64/` folder

### File Size

Self-contained executables are larger (typically 60-100 MB) because they include the .NET runtime. This is normal and expected.

### Usage

Users can run the executable directly without installing .NET:

**Windows:**
```batch
D365SolutionComparator.exe -s1 solution1.zip -s2 solution2.zip
```

**Linux/macOS:**
```bash
./D365SolutionComparator -s1 solution1.zip -s2 solution2.zip
```

## Creating a Release Package

To create a distributable ZIP file:

### Windows
```batch
cd publish
tar -czf D365SolutionComparator-windows-x64.zip windows-x64
```

### Linux/macOS
```bash
cd publish
zip -r D365SolutionComparator-windows-x64.zip windows-x64/
zip -r D365SolutionComparator-linux-x64.zip linux-x64/
zip -r D365SolutionComparator-osx-x64.zip osx-x64/
zip -r D365SolutionComparator-osx-arm64.zip osx-arm64/
```

## Troubleshooting

### Executable Too Large?

If the executable size is a concern, you can enable trimming (reduces size but may cause issues with reflection):

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -o publish/windows-x64
```

⚠️ **Warning**: Trimming may break functionality if the application uses reflection heavily. Test thoroughly!

### Permission Denied (Linux/macOS)

If users get "Permission denied" errors, they need to make the file executable:
```bash
chmod +x D365SolutionComparator
```

### macOS Security Warning

On macOS, users might see "cannot be opened because it is from an unidentified developer". They can bypass this by:
1. Right-click the executable
2. Select "Open"
3. Click "Open" in the security dialog

Or via command line:
```bash
xattr -d com.apple.quarantine D365SolutionComparator
```

## Advanced: Custom Runtime Identifiers

For other platforms, use these runtime identifiers with the `-r` flag:

- `win-x64`: Windows 64-bit
- `win-x86`: Windows 32-bit
- `win-arm64`: Windows ARM64
- `linux-x64`: Linux 64-bit
- `linux-arm`: Linux ARM
- `linux-arm64`: Linux ARM64
- `osx-x64`: macOS Intel
- `osx-arm64`: macOS Apple Silicon

Example:
```bash
dotnet publish -c Release -r linux-arm64 --self-contained true -p:PublishSingleFile=true -o publish/linux-arm64
```

## CI/CD Integration

To automate builds in GitHub Actions or Azure DevOps, see the example workflows in the `.github/workflows/` directory (if available), or use:

```yaml
- name: Publish Windows
  run: dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish/windows-x64
  
- name: Upload Artifacts
  uses: actions/upload-artifact@v3
  with:
    name: windows-x64
    path: publish/windows-x64/
```
