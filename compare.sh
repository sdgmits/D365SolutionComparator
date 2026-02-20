#!/bin/bash
# D365 Solution Comparator - Unix/Linux/macOS Shell Script
# This script makes it easier to run the comparator

echo ""
echo "╔═══════════════════════════════════════════════════════════╗"
echo "║      D365 Solution Comparator                            ║"
echo "╚═══════════════════════════════════════════════════════════╝"
echo ""

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ Error: .NET SDK is not installed or not in PATH"
    echo ""
    echo "Please install .NET 8.0 SDK from:"
    echo "https://dotnet.microsoft.com/download/dotnet/8.0"
    echo ""
    exit 1
fi

# Check for arguments
if [ -z "$1" ] || [ -z "$2" ]; then
    echo "Usage: ./compare.sh <source1> <source2> [output]"
    echo ""
    echo "Examples:"
    echo "  ./compare.sh solution1.zip solution2.zip"
    echo "  ./compare.sh solution1.zip solution2.zip report.html"
    echo "  ./compare.sh \"/path/to/Solutions/Dev\" \"/path/to/Solutions/Prod\""
    echo ""
    exit 1
fi

# Set default output if not provided
OUTPUT="${3:-comparison-report.html}"

echo "Running comparison..."
echo "Source 1: $1"
echo "Source 2: $2"
echo "Output:   $OUTPUT"
echo ""

# Get the directory where this script is located
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Run the comparator
dotnet run --project "$SCRIPT_DIR/D365SolutionComparator.csproj" -- --source1 "$1" --source2 "$2" --output "$OUTPUT"

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Comparison completed successfully!"
    echo ""
    
    # Try to open the report in default browser
    if command -v xdg-open &> /dev/null; then
        echo "Opening report in default browser..."
        xdg-open "$OUTPUT" &> /dev/null &
    elif command -v open &> /dev/null; then
        echo "Opening report in default browser..."
        open "$OUTPUT"
    else
        echo "Please open the report manually: $OUTPUT"
    fi
else
    echo ""
    echo "❌ Comparison failed. Check the error messages above."
    echo ""
fi
