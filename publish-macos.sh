#!/bin/bash
echo "Building self-contained macOS executables..."
echo

# Clean previous builds
rm -rf publish/osx-x64
rm -rf publish/osx-arm64

# Publish for macOS x64 (Intel)
echo "Building for macOS x64 (Intel)..."
dotnet publish D365SolutionComparator.csproj -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publish/osx-x64

if [ $? -eq 0 ]; then
    echo "macOS x64 build successful!"
    chmod +x publish/osx-x64/D365SolutionComparator
fi

echo
echo "Building for macOS ARM64 (Apple Silicon)..."
dotnet publish D365SolutionComparator.csproj -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publish/osx-arm64

if [ $? -eq 0 ]; then
    echo "macOS ARM64 build successful!"
    chmod +x publish/osx-arm64/D365SolutionComparator
    echo
    echo "Build successful!"
    echo "Executable locations:"
    echo "  - Intel Macs: publish/osx-x64/D365SolutionComparator"
    echo "  - Apple Silicon Macs: publish/osx-arm64/D365SolutionComparator"
    echo
    echo "You can now distribute the appropriate folder for the target Mac."
else
    echo "Build failed!"
fi
