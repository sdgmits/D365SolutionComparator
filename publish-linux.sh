#!/bin/bash
echo "Building self-contained Linux executable..."
echo

# Clean previous builds
rm -rf publish/linux-x64

# Publish for Linux x64
dotnet publish D365SolutionComparator.csproj -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publish/linux-x64

if [ $? -eq 0 ]; then
    echo
    echo "Build successful!"
    echo "Executable location: publish/linux-x64/D365SolutionComparator"
    echo
    echo "Making executable runnable..."
    chmod +x publish/linux-x64/D365SolutionComparator
    echo
    echo "You can now distribute the entire 'publish/linux-x64' folder."
else
    echo "Build failed!"
fi
