# D365 Solution Comparator

A comprehensive C# .NET 8 console application for comparing two Dynamics 365 CE solution exports and generating detailed HTML reports.

## 🎯 Features

- **Multiple Input Formats**: Accepts both ZIP files and extracted solution folders
- **Deep XML Analysis**: Node-by-node comparison of customizations.xml
- **Comprehensive Comparison**:
  - ✅ Entities (properties, attributes, forms, views, relationships, business rules)
  - ✅ Attributes (all type-specific properties)
  - ✅ Forms (structure, JavaScript libraries)
  - ✅ Views (FetchXML, layout)
  - ✅ Relationships (cascade configurations)
  - ✅ Business Rules
  - ✅ Web Resources (content hash comparison)
  - ✅ Processes/Workflows
  - ✅ Global Option Sets
  - ✅ Security Roles
  - ✅ App Modules
- **Interactive HTML Report**: 
  - Multi-tab interface with entity-specific tabs
  - Collapsible tree view for hierarchical navigation
  - Color-coded change indicators
  - Detailed property-level comparisons
- **Command-Line Interface**: Easy-to-use CLI with multiple options

## 📋 Prerequisites

- .NET 8.0 SDK or later
- Windows, macOS, or Linux

## 🚀 Installation

1. **Clone or download** this repository
2. **Navigate** to the project directory:
   ```bash
   cd SolutionComparison
   ```
3. **Restore dependencies** (if .NET SDK is installed):
   ```bash
   dotnet restore
   ```
4. **Build the project**:
   ```bash
   dotnet build
   ```

## 📖 Usage

### Basic Command

```bash
dotnet run -- --source1 "path/to/solution1.zip" --source2 "path/to/solution2.zip" --output "report.html"
```

### Command-Line Options

| Option | Alias | Description | Required |
|--------|-------|-------------|----------|
| `--source1` | `-s1` | Path to first solution (ZIP or folder) | Yes |
| `--source2` | `-s2` | Path to second solution (ZIP or folder) | Yes |
| `--output` | `-o` | Output path for HTML report (default: comparison-report.html) | No |
| `--verbose` | `-v` | Enable verbose logging | No |

### Examples

**Compare two ZIP files:**
```bash
dotnet run -- -s1 "DevSolution_1_0_0_1.zip" -s2 "ProdSolution_1_0_0_2.zip" -o "comparison.html"
```

**Compare extracted folders:**
```bash
dotnet run -- -s1 "C:\Solutions\Dev\extracted" -s2 "C:\Solutions\Prod\extracted"
```

**With verbose output:**
```bash
dotnet run -- -s1 "solution1.zip" -s2 "solution2.zip" --verbose
```

## 📊 Output

The application generates three files:

1. **comparison-report.html** - Main HTML report with interactive interface
2. **styles.css** - Styling for the report
3. **script.js** - JavaScript for interactive features

### Report Features

- **Summary Tab**: Overview statistics and critical changes
- **Entity Tabs**: Detailed comparison for each entity
- **Component Tabs**: Web Resources, Processes, Option Sets, Security Roles, App Modules
- **Tree View**: Expandable/collapsible hierarchical structure
- **Change Indicators**:
  - ➕ **Added** (Green) - Present in Solution 2 only
  - ➖ **Removed** (Red) - Present in Solution 1 only
  - ⚠️ **Modified** (Yellow) - Changed between solutions
  - ✅ **Unchanged** (Gray) - Identical in both solutions

## 🏗️ Project Structure

```
SolutionComparison/
├── D365SolutionComparator.csproj
├── Program.cs                          # Main entry point
├── Models/                             # Data models
│   ├── ChangeType.cs
│   ├── PropertyChange.cs
│   ├── ComparisonResult.cs
│   ├── SolutionInfo.cs
│   ├── EntityDefinition.cs
│   ├── AttributeDefinition.cs
│   ├── FormDefinition.cs
│   ├── ViewDefinition.cs
│   ├── RelationshipDefinition.cs
│   ├── BusinessRuleDefinition.cs
│   ├── WebResourceDefinition.cs
│   ├── ProcessDefinition.cs
│   ├── OptionSetDefinition.cs
│   ├── SecurityRoleDefinition.cs
│   └── AppModuleDefinition.cs
├── Parsers/                            # XML parsing
│   ├── SolutionExtractor.cs
│   ├── CustomizationsXmlParser.cs
│   ├── EntityParser.cs
│   ├── AttributeParser.cs
│   ├── FormParser.cs
│   ├── ViewParser.cs
│   ├── RelationshipParser.cs
│   ├── WebResourceParser.cs
│   ├── ProcessParser.cs
│   ├── OptionSetParser.cs
│   ├── SecurityRoleParser.cs
│   └── AppModuleParser.cs
├── Comparers/                          # Comparison logic
│   ├── IComparer.cs
│   ├── SolutionComparator.cs
│   ├── EntityComparer.cs
│   ├── AttributeComparer.cs
│   ├── FormComparer.cs
│   ├── ViewComparer.cs
│   ├── RelationshipComparer.cs
│   ├── BusinessRuleComparer.cs
│   ├── WebResourceComparer.cs
│   ├── ProcessComparer.cs
│   ├── OptionSetComparer.cs
│   ├── SecurityRoleComparer.cs
│   └── AppModuleComparer.cs
├── ReportGenerators/                   # HTML generation
│   ├── IReportGenerator.cs
│   └── HtmlReportGenerator.cs
└── Utilities/                          # Helper classes
    └── Logger.cs
```

## 🔧 Development

### Building for Release

```bash
dotnet build -c Release
```

### Publishing as Executable

**Windows (Self-contained):**
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

**macOS (Self-contained):**
```bash
dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true
```

**Linux (Self-contained):**
```bash
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
```

The executable will be in `bin/Release/net8.0/{runtime}/publish/`

## 🧪 Testing

To test the application, you'll need two D365 solution exports:

1. Export solutions from Dynamics 365 CE (Settings > Solutions > Export)
2. Run the comparator:
   ```bash
   dotnet run -- -s1 "Solution_v1.zip" -s2 "Solution_v2.zip"
   ```
3. Open the generated `comparison-report.html` in a web browser

## 📝 Notes

- **Temp Files**: The application automatically extracts ZIP files to temporary directories and cleans them up after execution
- **Memory Usage**: For very large solutions, the application loads the entire XML into memory. Monitor memory usage if working with extremely large solutions.
- **XML Structure**: The parser is designed for standard D365 CE solution exports. Custom or modified solution structures may not be fully supported.

## 🤝 Contributing

Contributions are welcome! Areas for enhancement:

- [ ] Excel export functionality
- [ ] JSON export for programmatic consumption
- [ ] Deeper form XML comparison (tab/section/field level)
- [ ] Plugin step comparison enhancements
- [ ] Support for solution patches
- [ ] Filtering options for specific components
- [ ] Diff view for XML content

## 📄 License

This project is provided as-is for comparison purposes.

## 🆘 Troubleshooting

### Error: "customizations.xml not found"
**Solution**: Ensure the ZIP file or folder is a valid D365 solution export containing customizations.xml

### Error: ".NET SDK not installed"
**Solution**: Install .NET 8.0 SDK from https://dotnet.microsoft.com/download

### Report shows "0 entities"
**Solution**: Verify the solution export contains entity customizations

### Large solutions are slow
**Solution**: This is expected for solutions with many components. Consider filtering or splitting the solution.

## 📧 Support

For issues or questions, please check:
1. Ensure both solution paths are correct
2. Verify the solutions are valid D365 exports
3. Check the console output for specific error messages
4. Use `--verbose` flag for detailed logging

---

**Built with ❤️ for D365 Developers**
