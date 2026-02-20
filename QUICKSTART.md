# 🚀 Quick Start Guide - D365 Solution Comparator

## Prerequisites

Before using the comparator, ensure you have:

- ✅ .NET 8.0 SDK installed ([Download here](https://dotnet.microsoft.com/download/dotnet/8.0))
- ✅ Two D365 solution exports (ZIP files or extracted folders)

## Quick Usage

### Option 1: Using the Convenience Scripts

**Windows:**
```cmd
compare.bat solution1.zip solution2.zip
```

**macOS/Linux:**
```bash
chmod +x compare.sh
./compare.sh solution1.zip solution2.zip
```

### Option 2: Using dotnet CLI

```bash
dotnet run -- --source1 "solution1.zip" --source2 "solution2.zip"
```

## Testing with Sample Solutions

If you have sample solutions in the directory, try:

```bash
# Windows
compare.bat SolutionContoso_1_0_0_1.zip SolutionSDDemo_1_0_0_1.zip

# macOS/Linux
./compare.sh SolutionContoso_1_0_0_1.zip SolutionSDDemo_1_0_0_1.zip
```

## Output

The comparison generates:
- 📄 **comparison-report.html** - Open this in your browser
- 🎨 **styles.css** - Styling file
- ⚡ **script.js** - Interactive features

## Common Commands

### Build the project
```bash
dotnet build
```

### Run with custom output
```bash
dotnet run -- -s1 "dev.zip" -s2 "prod.zip" -o "my-report.html"
```

### Run with verbose output
```bash
dotnet run -- -s1 "solution1.zip" -s2 "solution2.zip" --verbose
```

## Troubleshooting

### ❌ "dotnet: command not found"
**Solution:** Install .NET 8.0 SDK from https://dotnet.microsoft.com/download

### ❌ "customizations.xml not found"
**Solution:** Verify your ZIP file is a valid D365 solution export

### ❌ Build errors
**Solution:** Run `dotnet restore` to restore NuGet packages

## What Gets Compared?

✅ **Entities** - Properties, attributes, forms, views, relationships  
✅ **Attributes** - All type-specific properties and metadata  
✅ **Forms** - Structure, JavaScript libraries, form types  
✅ **Views** - FetchXML queries, layouts, columns  
✅ **Relationships** - Cascade configurations, entity references  
✅ **Business Rules** - Rules and scopes  
✅ **Web Resources** - Scripts, styles, images (content hash)  
✅ **Processes** - Workflows, actions, business process flows  
✅ **Option Sets** - Global option sets and values  
✅ **Security Roles** - Privileges per entity  
✅ **App Modules** - Model-driven apps and components  

## Report Features

The generated HTML report includes:

- 📊 **Summary Tab** - Overview with statistics and critical changes
- 📦 **Entity Tabs** - Detailed view for each entity
- 🌐 **Component Tabs** - Web resources, processes, security roles, etc.
- 🌳 **Tree View** - Collapsible hierarchical structure
- 🎨 **Color Coding**:
  - 🟢 Green (➕) = Added
  - 🔴 Red (➖) = Removed
  - 🟡 Yellow (⚠️) = Modified
  - ⚪ Gray (✅) = Unchanged

## Next Steps

1. Open the generated HTML report in your browser
2. Navigate through tabs to explore changes
3. Click on tree items to expand/collapse details
4. Review the Summary tab for overall statistics
5. Check critical changes section for important modifications

## Need Help?

- Check [README.md](README.md) for detailed documentation
- Review [Outline.md](Outline.md) for technical specifications
- Ensure your solutions are valid D365 exports
- Use `--verbose` flag for detailed error information

---

**Happy Comparing! 🎯**
