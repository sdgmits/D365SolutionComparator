🎯 PROJECT SPECIFICATION
Project Name: D365 Solution Comparator (Alternative names: SolutionDiffAnalyzer, CRM Solution Comparer, D365 SolutionSync Analyzer)

Technology Stack: C# .NET 8.0 (Console Application with potential Web UI later)

IDE: Visual Studio Code

📋 REQUIREMENTS
Core Functionality
Build a C# console application that compares two Dynamics 365 CE solution exports and generates a comprehensive difference report.

Input Requirements
The application must accept TWO inputs in either format:

Solution ZIP files (exported from D365 CE)
Extracted folder structure (pre-unzipped solution files)
Recommendation: Support both formats. If ZIP is provided, extract to temp directory first.

Input Parameters:

Code
D365SolutionComparator.exe --source1 "path/to/solution1.zip" --source2 "path/to/solution2.zip" --output "path/to/report.html"
OR

Code
D365SolutionComparator.exe --source1 "path/to/extracted/folder1" --source2 "path/to/extracted/folder2" --output "path/to/report.html"
🔍 COMPARISON REQUIREMENTS
Primary Target: customizations.xml File
Perform a node-by-node XML traversal and deep comparison of the customizations.xml file from both solutions.

Specific Comparison Categories
1. Entity-Level Changes
Detect entities added in Solution 2 but not in Solution 1
Detect entities removed (present in Solution 1 but not in Solution 2)
Detect entities present in both but with modifications
Properties to compare:

DisplayName, PluralName, Description
OwnershipType (User/Team/Organization)
IsActivity, IsCustomEntity, IsAuditEnabled
IsValidForQueue, IsConnectionsEnabled
IsDuplicateDetectionEnabled
IsMailMergeEnabled
PrimaryNameAttribute
PrimaryImageAttribute
EntityColor, EntityHelpUrlEnabled
AutoRouteToOwnerQueue
All other entity metadata properties
2. Attribute-Level Changes (per Entity)
Detect attributes added/removed per entity
Detect modified attribute properties
Properties to compare:

AttributeType (String, Lookup, OptionSet, DateTime, etc.)
DisplayName, Description
RequiredLevel (None, SystemRequired, ApplicationRequired, Recommended)
IsAuditEnabled, IsSecured, IsCustomizable
SchemaName, LogicalName
Type-specific properties:
String: MaxLength, Format, ImeMode
Integer/Decimal/Money: MinValue, MaxValue, Precision
DateTime: Format (DateOnly, DateAndTime), ImeMode
Lookup: Targets (related entities)
OptionSet/Picklist: OptionSet values (label, value pairs)
Two Options (Boolean): TrueOption, FalseOption labels
3. Relationship Changes
One-to-Many (1:N) relationships
Many-to-One (N:1) relationships
Many-to-Many (N:N) relationships
Properties to compare:

SchemaName, RelationshipType
ReferencingEntity, ReferencedEntity
ReferencingAttribute, ReferencedAttribute
CascadeConfiguration (Assign, Delete, Merge, Reparent, Share, Unshare)
IsCustomizable, IsValidForAdvancedFind
AssociatedMenuConfiguration
4. Form Changes
System Forms, Quick Create Forms, Quick View Forms, Card Forms
Form XML structure comparison
Hierarchical comparison:


Form
├── Tabs
│   ├── Tab Properties (Name, Label, Visible, Expanded)
│   └── Sections
│       ├── Section Properties (Name, Label, ShowLabel)
│       └── Fields/Controls
│           ├── Field Properties (DataField, Required, Visible, Disabled)
│           └── Control Type
├── Header Fields
├── Footer Fields
├── Navigation
└── FormXml Events (OnLoad, OnSave, etc.)
└── JavaScript Libraries
Properties to compare:

FormId, Name, FormType
IsDefault, IsDesktop, IsTablet, IsPhone
FormXml structure (deep comparison)
JavaScript library references
Business Rules attached
5. View Changes (SavedQuery)
System Views, Public Views
Properties to compare:

SavedQueryId, Name, QueryType
FetchXml query structure
LayoutXml (column layout)
ColumnSetXml
IsDefault, IsUserDefined
ReturnedTypeCode
6. Business Rules
Business Rule definitions per entity
Properties to compare:

Name, Description
Scope (Entity, All Forms, Specific Forms)
Conditions and Actions (XML structure)
7. Web Resources
JavaScript, CSS, HTML, Images, Data (XML), Stylesheets (XSL)
Properties to compare:

Name, DisplayName, WebResourceType
Content (file content hash comparison)
IsCustomizable, IsEnabledForMobileClient
LanguageCode
8. Processes (Workflows, Actions, Business Process Flows)
Classic Workflows (XAML)
Actions
Business Process Flows
Properties to compare:

Name, Category, ProcessType
IsTransacted, Scope, Mode (Background/Real-time)
XAML definition structure
Trigger conditions
9. Option Sets (Global)
Global OptionSets
Properties to compare:

Name, DisplayName, Description
IsGlobal, IsCustomizable
Options (Value, Label, Color)
DefaultValue
10. Security Roles
Role definitions
Properties to compare:

Name, RoleId
Privileges per entity (Create, Read, Write, Delete, Append, AppendTo, Assign, Share)
Privilege Depth (None, User, Business Unit, Parent-Child Business Units, Organization)
11. Field Security Profiles
Field-level security configurations
12. Site Map
Navigation structure
Properties to compare:

Areas, Groups, SubAreas
Titles, URLs, Icons
13. App Modules (Model-Driven Apps)
App configurations
Properties to compare:

UniqueName, DisplayName, Description
AppModuleComponents (Entities, Dashboards, Business Process Flows included)
FormFactor (Desktop, Tablet, Phone)
14. Dashboards
System Dashboards, Personal Dashboards
Properties to compare:

Name, Description
FormXml (dashboard layout)
15. Charts
System Charts, Personal Charts
Properties to compare:

Name, Description, ChartType
DataDescription (FetchXml)
PresentationDescription (Chart definition)
16. Email Templates
Email template definitions
17. Connection Roles
Connection role definitions
18. SDK Message Processing Steps (Plugin Steps)
Plugin registration details
Properties to compare:

Name, Stage, Mode, Rank
FilteringAttributes
SecureConfigId, UnsecureConfig
19. Other Customizations
Any other XML nodes present in customizations.xml
📤 OUTPUT REQUIREMENTS
Format: HTML Report
Generate an interactive HTML file with the following characteristics:

Layout Structure:
Multi-Tab Interface:

Tab 1: Summary (Overview of all changes)
Tab 2+: Dynamic Tabs
One tab per Entity (e.g., "account", "contact", "custom_entity")
One tab per Model-Driven App (if present)
Additional tabs for:
Global Option Sets
Web Resources
Security Roles
Processes
Site Map
Dashboards
Charts
Tree View with Drill-Down Capability:
Each tab should display a collapsible/expandable tree structure showing:


📦 Entity: account
├─ 🔧 Entity Properties
│  ├─ DisplayName: ⚠️ Different
│  ├─ IsAuditEnabled: ⚠️ Different
│  └─ OwnershipType: ✅ Same
├─ 📝 Attributes (15 total, 2 added, 1 removed, 3 modified)
│  ├─ ➕ new_customfield (Added in Solution 2)
│  ├─ ➖ old_deprecatedfield (Removed from Solution 2)
│  ├─ 📊 accountname
│  │  ├─ DisplayName: ✅ Same
│  │  ├─ MaxLength: ⚠️ Different (100 → 200)
│  │  └─ RequiredLevel: ⚠️ Different (None → ApplicationRequired)
│  └─ 📊 telephone1
│     └─ All properties: ✅ Same
├─ 📄 Forms (3 total, 1 modified)
│  ├─ Main Form - Information
│  │  ├─ 🔵 Tab: General
│  │  │  ├─ Tab Properties: ✅ Same
│  │  │  ├─ 🔲 Section: Account Information
│  │  │  │  ├─ Section Properties: ✅ Same
│  │  │  │  ├─ Field: accountname: ✅ Same
│  │  │  │  └─ Field: new_customfield: ➕ Added
│  │  │  └─ 🔲 Section: Contact Details
│  │  │     └─ All Fields: ✅ Same
│  │  ├─ 🔵 Tab: Details
│  │  │  └─ ⚠️ Modified
│  │  └─ JavaScript Libraries: ⚠️ Different (1 added)
│  └─ Quick Create Form: ✅ Unchanged
├─ 👁️ Views (5 total, 1 modified)
│  ├─ Active Accounts: ⚠️ FetchXml changed
│  └─ My Active Accounts: ✅ Unchanged
├─ ⚙️ Business Rules (2 total, 1 added)
│  ├─ ➕ New Validation Rule (Added in Solution 2)
│  └─ Set Required Fields: ✅ Unchanged
└─ 🔗 Relationships (3 total)
   ├─ account_contact (1:N): ✅ Unchanged
   └─ account_opportunity (1:N): ⚠️ CascadeDelete changed
Three-Column Comparison Display:
When a property is selected/expanded, show detailed comparison:

Property	Solution 1 (Source)	Solution 2 (Target)	Status
DisplayName	"Account Name"	"Business Account Name"	⚠️ Different
MaxLength	100	200	⚠️ Different
RequiredLevel	None	ApplicationRequired	⚠️ Different
IsAuditEnabled	true	true	✅ Same
AttributeType	String	String	✅ Same
Status Icons and Color Coding:
Use consistent visual indicators:

Status	Icon	Color	Meaning
✅ Same / Unchanged	✅	Green	Identical in both solutions
⚠️ Different / Modified	⚠️	Orange/Yellow	Property value changed
➕ Added / New	➕	Blue	Present in Solution 2 only
➖ Removed / Deleted	➖	Red	Present in Solution 1 only
⚪ Not Applicable	⚪	Gray	Doesn't apply or not compared
Summary Tab Structure:
Code
📊 COMPARISON SUMMARY
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

📁 Solutions Compared:
   Source 1: DevEnvironment_Solution_1_0_0_1.zip
   Source 2: ProductionEnvironment_Solution_1_0_0_2.zip
   Comparison Date: 2026-02-20 10:30:45

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

📈 CHANGE STATISTICS

┌─────────────────────────┬───────┬────────┬──────────┬─────────┐
│ Component Type          │ Added │ Removed│ Modified │ Total   │
├─────────────────────────┼───────┼────────┼──────────┼─────────┤
│ Entities                │   2   │   1    │    5     │    8    │
│ Attributes              │  12   │   3    │   18     │   33    │
│ Relationships           │   1   │   0    │    2     │    3    │
│ Forms                   │   0   │   0    │    4     │    4    │
│ Views                   │   2   │   1    │    3     │    6    │
│ Business Rules          │   3   │   0    │    1     │    4    │
│ Web Resources           │   5   │   2    │    8     │   15    │
│ Processes               │   1   │   0    │    2     │    3    │
│ Security Roles          │   0   │   0    │    1     │    1    │
│ Global Option Sets      │   1   │   0    │    0     │    1    │
│ App Modules             │   1   │   0    │    1     │    2    │
└─────────────────────────┴───────┴────────┴──────────┴─────────┘

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

🔍 CRITICAL CHANGES (High Impact)

⚠️ Entity 'account':
   - Attribute 'name' MaxLength changed: 100 → 200
   - Form 'Main Form' has 3 new fields
   - Business Rule 'Validate Account' modified

⚠️ Entity 'new_customentity':
   - ➕ Entire entity added in Solution 2
   - Contains 15 attributes, 2 forms, 3 views

⚠️ Security Role 'Sales Manager':
   - Privilege level changed for 'account' entity (User → Business Unit)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

📋 QUICK NAVIGATION
[Jump to Entities] [Jump to Forms] [Jump to Security] [Export to Excel]
🛠️ TECHNICAL IMPLEMENTATION REQUIREMENTS
C# Project Structure
Code
D365SolutionComparator/
├── D365SolutionComparator.csproj
├── Program.cs (Entry point)
├── Models/
│   ├── SolutionInfo.cs
│   ├── ComparisonResult.cs
│   ├── EntityDefinition.cs
│   ├── AttributeDefinition.cs
│   ├── FormDefinition.cs
│   ├── ViewDefinition.cs
│   ├── ChangeType.cs (enum)
│   └── ... (other model classes)
├── Parsers/
│   ├── SolutionExtractor.cs (ZIP handling)
│   ├── CustomizationsXmlParser.cs (Main XML parser)
│   ├── EntityParser.cs
│   ├── AttributeParser.cs
│   ├── FormParser.cs
│   ├── ViewParser.cs
│   └── ... (other parsers)
├── Comparers/
│   ├── IComparer.cs (interface)
│   ├── EntityComparer.cs
│   ├── AttributeComparer.cs
│   ├── FormComparer.cs
│   ├── ViewComparer.cs
│   ├── XmlNodeComparer.cs (Generic XML comparison)
│   └── ... (other comparers)
├── ReportGenerators/
│   ├── IReportGenerator.cs (interface)
│   ├── HtmlReportGenerator.cs
│   ├── ExcelReportGenerator.cs (optional)
│   └── Templates/
│       ├── report-template.html
│       ├── styles.css
│       └── script.js
└── Utilities/
    ├── Logger.cs
    ├── FileHelper.cs
    └── XmlHelper.cs
Key C# Libraries to Use
XML
<ItemGroup>
  <!-- Core Framework -->
  <PackageReference Include="Microsoft.Extensions.Configuration" Version="8.0.0" />
  <PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.0" />
  
  <!-- XML Processing -->
  <PackageReference Include="System.Xml.Linq" Version="8.0.0" />
  
  <!-- ZIP File Handling -->
  <PackageReference Include="System.IO.Compression" Version="8.0.0" />
  <PackageReference Include="System.IO.Compression.ZipFile" Version="8.0.0" />
  
  <!-- HTML Generation -->
  <PackageReference Include="RazorEngine.NetCore" Version="3.1.0" />
  <!-- OR hand-craft HTML with StringBuilder -->
  
  <!-- Command-line Argument Parsing -->
  <PackageReference Include="CommandLineParser" Version="2.9.1" />
  
  <!-- JSON Serialization (for intermediate storage) -->
  <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  
  <!-- Optional: Excel Generation -->
  <PackageReference Include="EPPlus" Version="7.0.0" />
  <!-- OR -->
  <PackageReference Include="ClosedXML" Version="0.102.0" />
  
  <!-- Optional: Logging -->
  <PackageReference Include="Serilog" Version="3.1.1" />
  <PackageReference Include="Serilog.Sinks.Console" Version="5.0.1" />
  <PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
</ItemGroup>
Core Implementation Approach
1. Solution Extraction
C#
public class SolutionExtractor
{
    public string ExtractSolution(string inputPath)
    {
        // Check if input is ZIP file or directory
        if (Path.GetExtension(inputPath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
        {
            // Extract to temp directory
            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            ZipFile.ExtractToDirectory(inputPath, tempDir);
            return tempDir;
        }
        else if (Directory.Exists(inputPath))
        {
            // Already extracted
            return inputPath;
        }
        else
        {
            throw new ArgumentException("Input must be a ZIP file or extracted directory.");
        }
    }
    
    public string GetCustomizationsXmlPath(string solutionDirectory)
    {
        // Typically located at: solutionDirectory/customizations.xml
        string customizationsPath = Path.Combine(solutionDirectory, "customizations.xml");
        
        if (!File.Exists(customizationsPath))
        {
            throw new FileNotFoundException("customizations.xml not found in solution.");
        }
        
        return customizationsPath;
    }
}
2. XML Parsing with LINQ to XML
C#
public class CustomizationsXmlParser
{
    public XDocument LoadCustomizationsXml(string xmlPath)
    {
        return XDocument.Load(xmlPath);
    }
    
    public List<EntityDefinition> ParseEntities(XDocument customizationsXml)
    {
        var entities = new List<EntityDefinition>();
        
        var entityNodes = customizationsXml.Descendants("Entity");
        
        foreach (var entityNode in entityNodes)
        {
            var entity = new EntityDefinition
            {
                LogicalName = entityNode.Attribute("Name")?.Value,
                // Parse EntityInfo
                DisplayName = entityNode.Element("EntityInfo")
                    ?.Element("entity")
                    ?.Element("LocalizedNames")
                    ?.Element("LocalizedName")
                    ?.Attribute("description")?.Value,
                // ... parse all other properties
                
                // Parse attributes
                Attributes = ParseAttributes(entityNode),
                
                // Parse forms
                Forms = ParseForms(entityNode),
                
                // Parse views
                Views = ParseViews(entityNode),
                
                // Store original XElement for deep comparison
                OriginalXml = entityNode
            };
            
            entities.Add(entity);
        }
        
        return entities;
    }
    
    private List<AttributeDefinition> ParseAttributes(XElement entityNode)
    {
        var attributes = new List<AttributeDefinition>();
        
        var attributeNodes = entityNode.Element("EntityInfo")
            ?.Element("entity")
            ?.Element("attributes")
            ?.Elements("attribute");
        
        if (attributeNodes == null) return attributes;
        
        foreach (var attrNode in attributeNodes)
        {
            var attribute = new AttributeDefinition
            {
                LogicalName = attrNode.Element("LogicalName")?.Value,
                SchemaName = attrNode.Element("SchemaName")?.Value,
                AttributeType = attrNode.Element("Type")?.Value,
                DisplayName = attrNode.Element("displaynames")
                    ?.Element("displayname")
                    ?.Attribute("description")?.Value,
                RequiredLevel = attrNode.Element("RequiredLevel")?.Value,
                MaxLength = int.TryParse(attrNode.Element("MaxLength")?.Value, out int maxLen) 
                    ? maxLen : (int?)null,
                // ... parse all other type-specific properties
                
                OriginalXml = attrNode
            };
            
            attributes.Add(attribute);
        }
        
        return attributes;
    }
    
    // Similar methods for ParseForms, ParseViews, ParseRelationships, etc.
}
3. Comparison Engine
C#
public class EntityComparer : IComparer<EntityDefinition>
{
    public ComparisonResult Compare(EntityDefinition source, EntityDefinition target)
    {
        var result = new ComparisonResult
        {
            ComponentType = "Entity",
            ComponentName = source?.LogicalName ?? target?.LogicalName
        };
        
        // Determine change type
        if (source == null && target != null)
        {
            result.ChangeType = ChangeType.Added;
            return result;
        }
        else if (source != null && target == null)
        {
            result.ChangeType = ChangeType.Removed;
            return result;
        }
        
        // Compare properties
        result.PropertyChanges = new Dictionary<string, PropertyChange>();
        
        CompareProperty(result, "DisplayName", source.DisplayName, target.DisplayName);
        CompareProperty(result, "IsAuditEnabled", source.IsAuditEnabled, target.IsAuditEnabled);
        CompareProperty(result, "OwnershipType", source.OwnershipType, target.OwnershipType);
        // ... compare all properties
        
        // Compare child components
        result.ChildComparisons = new List<ComparisonResult>();
        
        // Compare attributes
        var attributeComparer = new AttributeComparer();
        result.ChildComparisons.AddRange(
            CompareCollections(source.Attributes, target.Attributes, 
                a => a.LogicalName, attributeComparer)
        );
        
        // Compare forms
        var formComparer = new FormComparer();
        result.ChildComparisons.AddRange(
            CompareCollections(source.Forms, target.Forms, 
                f => f.FormId, formComparer)
        );
        
        // Determine overall change type
        result.ChangeType = result.PropertyChanges.Any(p => p.Value.IsDifferent) 
            || result.ChildComparisons.Any(c => c.ChangeType != ChangeType.Unchanged)
            ? ChangeType.Modified
            : ChangeType.Unchanged;
        
        return result;
    }
    
    private void CompareProperty<T>(ComparisonResult result, string propertyName, 
        T sourceValue, T targetValue)
    {
        bool isDifferent = !EqualityComparer<T>.Default.Equals(sourceValue, targetValue);
        
        result.PropertyChanges[propertyName] = new PropertyChange
        {
            PropertyName = propertyName,
            SourceValue = sourceValue?.ToString(),
            TargetValue = targetValue?.ToString(),
            IsDifferent = isDifferent
        };
    }
    
    private List<ComparisonResult> CompareCollections<T>(
        List<T> sourceList, 
        List<T> targetList,
        Func<T, string> keySelector,
        IComparer<T> comparer)
    {
        var results = new List<ComparisonResult>();
        
        var sourceDict = sourceList?.ToDictionary(keySelector) 
            ?? new Dictionary<string, T>();
        var targetDict = targetList?.ToDictionary(keySelector) 
            ?? new Dictionary<string, T>();
        
        var allKeys = sourceDict.Keys.Union(targetDict.Keys).Distinct();
        
        foreach (var key in allKeys)
        {
            sourceDict.TryGetValue(key, out T sourceItem);
            targetDict.TryGetValue(key, out T targetItem);
            
            var comparison = comparer.Compare(sourceItem, targetItem);
            results.Add(comparison);
        }
        
        return results;
    }
}
4. HTML Report Generation
C#
public class HtmlReportGenerator : IReportGenerator
{
    public void GenerateReport(ComparisonResult rootResult, string outputPath)
    {
        var html = new StringBuilder();
        
        // HTML Header
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang='en'>");
        html.AppendLine("<head>");
        html.AppendLine("    <meta charset='UTF-8'>");
        html.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        html.AppendLine("    <title>D365 Solution Comparison Report</title>");
        html.AppendLine("    <link rel='stylesheet' href='styles.css'>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");
        
        // Tab Navigation
        html.AppendLine("<div class='tab-container'>");
        html.AppendLine("    <div class='tab-nav'>");
        html.AppendLine("        <button class='tab-button active' onclick='openTab(event, \"summary\")'>Summary</button>");
        
        // Generate tabs for entities
        foreach (var entityComparison in rootResult.EntityComparisons)
        {
            html.AppendLine($"        <button class='tab-button' onclick='openTab(event, \"{entityComparison.ComponentName}\")'>{entityComparison.ComponentName}</button>");
        }
        
        html.AppendLine("    </div>");
        
        // Summary Tab Content
        html.AppendLine("    <div id='summary' class='tab-content' style='display:block;'>");
        html.AppendLine(GenerateSummaryContent(rootResult));
        html.AppendLine("    </div>");
        
        // Entity Tab Contents
        foreach (var entityComparison in rootResult.EntityComparisons)
        {
            html.AppendLine($"    <div id='{entityComparison.ComponentName}' class='tab-content'>");
            html.AppendLine(GenerateEntityContent(entityComparison));
            html.AppendLine("    </div>");
        }
        
        html.AppendLine("</div>");
        
        // JavaScript
        html.AppendLine("<script src='script.js'></script>");
        html.AppendLine("</body>");
        html.AppendLine("</html>");
        
        // Write to file
        File.WriteAllText(outputPath, html.ToString());
        
        // Also generate CSS and JS files
        GenerateStylesheet(Path.GetDirectoryName(outputPath));
        GenerateJavaScript(Path.GetDirectoryName(outputPath));
    }
    
    private string GenerateSummaryContent(ComparisonResult result)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<h1>📊 COMPARISON SUMMARY</h1>");
        sb.AppendLine($"<p>Comparison Date: {DateTime.Now}</p>");
        
        sb.AppendLine("<h2>📈 CHANGE STATISTICS</h2>");
        sb.AppendLine("<table class='summary-table'>");
        sb.AppendLine("<thead>");
        sb.AppendLine("    <tr>");
        sb.AppendLine("        <th>Component Type</th>");
        sb.AppendLine("        <th>Added</th>");
        sb.AppendLine("        <th>Removed</th>");
        sb.AppendLine("        <th>Modified</th>");
        sb.AppendLine("        <th>Total</th>");
        sb.AppendLine("    </tr>");
        sb.AppendLine("</thead>");
        sb.AppendLine("<tbody>");
        
        // Generate statistics rows
        var stats = CalculateStatistics(result);
        foreach (var stat in stats)
        {
            sb.AppendLine("    <tr>");
            sb.AppendLine($"        <td>{stat.ComponentType}</td>");
            sb.AppendLine($"        <td class='added'>{stat.Added}</td>");
            sb.AppendLine($"        <td class='removed'>{stat.Removed}</td>");
            sb.AppendLine($"        <td class='modified'>{stat.Modified}</td>");
            sb.AppendLine($"        <td>{stat.Total}</td>");
            sb.AppendLine("    </tr>");
        }
        
        sb.AppendLine("</tbody>");
        sb.AppendLine("</table>");
        
        return sb.ToString();
    }
    
    private string GenerateEntityContent(ComparisonResult entityComparison)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"<h1>Entity: {entityComparison.ComponentName}</h1>");
        
        // Tree view structure
        sb.AppendLine("<div class='tree-view'>");
        sb.AppendLine("    <ul class='tree'>");
        
        // Entity Properties
        sb.AppendLine("        <li>");
        sb.AppendLine($"            <span class='tree-toggle'>{GetStatusIcon(entityComparison.ChangeType)} Entity Properties</span>");
        sb.AppendLine("            <ul class='nested'>");
        
        foreach (var prop in entityComparison.PropertyChanges)
        {
            string icon = prop.Value.IsDifferent ? "⚠️" : "✅";
            sb.AppendLine($"                <li>{icon} {prop.Key}: {RenderPropertyValue(prop.Value)}</li>");
        }
        
        sb.AppendLine("            </ul>");
        sb.AppendLine("        </li>");
        
        // Attributes
        var attributes = entityComparison.ChildComparisons
            .Where(c => c.ComponentType == "Attribute").ToList();
        
        if (attributes.Any())
        {
            sb.AppendLine("        <li>");
            sb.AppendLine($"            <span class='tree-toggle'>📝 Attributes ({attributes.Count} total)</span>");
            sb.AppendLine("            <ul class='nested'>");
            
            foreach (var attr in attributes)
            {
                sb.AppendLine($"                <li>");
                sb.AppendLine($"                    <span class='tree-toggle'>{GetStatusIcon(attr.ChangeType)} {attr.ComponentName}</span>");
                
                if (attr.PropertyChanges.Any())
                {
                    sb.AppendLine("                    <ul class='nested'>");
                    foreach (var prop in attr.PropertyChanges)
                    {
                        string icon = prop.Value.IsDifferent ? "⚠️" : "✅";
                        sb.AppendLine($"                        <li>{icon} {prop.Key}: {RenderPropertyValue(prop.Value)}</li>");
                    }
                    sb.AppendLine("                    </ul>");
                }
                
                sb.AppendLine($"                </li>");
            }
            
            sb.AppendLine("            </ul>");
            sb.AppendLine("        </li>");
        }
        
        // Forms, Views, etc. (similar pattern)
        
        sb.AppendLine("    </ul>");
        sb.AppendLine("</div>");
        
        return sb.ToString();
    }
    
    private string GetStatusIcon(ChangeType changeType)
    {
        return changeType switch
        {
            ChangeType.Added => "➕",
            ChangeType.Removed => "➖",
            ChangeType.Modified => "⚠️",
            ChangeType.Unchanged => "✅",
            _ => "⚪"
        };
    }
    
    private string RenderPropertyValue(PropertyChange change)
    {
        if (!change.IsDifferent)
        {
            return $"<span class='same'>{change.SourceValue}</span>";
        }
        else
        {
            return $"<span class='different'>{change.SourceValue} → {change.TargetValue}</span>";
        }
    }
}
5. JavaScript for Interactivity
JavaScript
// script.js

// Tab switching
function openTab(evt, tabName) {
    var i, tabcontent, tabbuttons;
    
    // Hide all tab contents
    tabcontent = document.getElementsByClassName("tab-content");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    
    // Remove active class from all buttons
    tabbuttons = document.getElementsByClassName("tab-button");
    for (i = 0; i < tabbuttons.length; i++) {
        tabbuttons[i].className = tabbuttons[i].className.replace(" active", "");
    }
    
    // Show current tab and mark button as active
    document.getElementById(tabName).style.display = "block";
    evt.currentTarget.className += " active";
}

// Tree view toggle
document.addEventListener('DOMContentLoaded', function() {
    var togglers = document.getElementsByClassName("tree-toggle");
    
    for (var i = 0; i < togglers.length; i++) {
        togglers[i].addEventListener("click", function() {
            this.parentElement.querySelector(".nested").classList.toggle("active");
            this.classList.toggle("expanded");
        });
    }
});

// Search/Filter functionality
function filterTree(searchText) {
    // Implement search logic
}

// Export to Excel (if implemented)
function exportToExcel() {
    // Trigger Excel export
}
6. CSS Styling
CSS
/* styles.css */

:root {
    --color-added: #28a745;
    --color-removed: #dc3545;
    --color-modified: #ffc107;
    --color-unchanged: #6c757d;
    --color-same: #28a745;
}

body {
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    margin: 0;
    padding: 20px;
    background-color: #f5f5f5;
}

.tab-container {
    background: white;
    border-radius: 8px;
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    overflow: hidden;
}

.tab-nav {
    background-color: #2c3e50;
    padding: 10px;
    display: flex;
    overflow-x: auto;
}

.tab-button {
    background-color: transparent;
    color: white;
    border: none;
    padding: 10px 20px;
    cursor: pointer;
    transition: background-color 0.3s;
    white-space: nowrap;
}

.tab-button:hover {
    background-color: #34495e;
}

.tab-button.active {
    background-color: #3498db;
}

.tab-content {
    display: none;
    padding: 20px;
}

/* Tree View */
.tree-view ul {
    list-style-type: none;
    padding-left: 20px;
}

.tree-view .tree-toggle {
    cursor: pointer;
    user-select: none;
    padding: 5px;
    display: inline-block;
}

.tree-view .tree-toggle:hover {
    background-color: #f0f0f0;
    border-radius: 4px;
}

.tree-view .nested {
    display: none;
}

.tree-view .nested.active {
    display: block;
}

.tree-view .tree-toggle::before {
    content: "▶ ";
    display: inline-block;
    margin-right: 6px;
    transition: transform 0.3s;
}

.tree-view .tree-toggle.expanded::before {
    transform: rotate(90deg);
}

/* Status Colors */
.added {
    color: var(--color-added);
    font-weight: bold;
}

.removed {
    color: var(--color-removed);
    font-weight: bold;
}

.modified {
    color: var(--color-modified);
    font-weight: bold;
}

.same {
    color: var(--color-same);
}

.different {
    color: var(--color-modified);
    font-weight: bold;
}

/* Summary Table */
.summary-table {
    width: 100%;
    border-collapse: collapse;
    margin: 20px 0;
}

.summary-table th,
.summary-table td {
    padding: 12px;
    text-align: left;
    border-bottom: 1px solid #ddd;
}

.summary-table th {
    background-color: #2c3e50;
    color: white;
}

.summary-table tr:hover {
    background-color: #f5f5f5;
}

/* Comparison Table */
.comparison-table {
    width: 100%;
    border-collapse: collapse;
    margin: 20px 0;
}

.comparison-table th,
.comparison-table td {
    padding: 10px;
    border: 1px solid #ddd;
    text-align: left;
}

.comparison-table th {
    background-color: #34495e;
    color: white;
}

.comparison-table td {
    vertical-align: top;
}
📝 PROGRAM ENTRY POINT
C#
// Program.cs

using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace D365SolutionComparator
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("D365 Solution Comparator - Compare Dynamics 365 CE solution exports");
            
            var source1Option = new Option<string>(
                aliases: new[] { "--source1", "-s1" },
                description: "Path to first solution (ZIP file or extracted folder)");
            source1Option.IsRequired = true;
            
            var source2Option = new Option<string>(
                aliases: new[] { "--source2", "-s2" },
                description: "Path to second solution (ZIP file or extracted folder)");
            source2Option.IsRequired = true;
            
            var outputOption = new Option<string>(
                aliases: new[] { "--output", "-o" },
                description: "Output path for the HTML report",
                getDefaultValue: () => "comparison-report.html");
            
            var verboseOption = new Option<bool>(
                aliases: new[] { "--verbose", "-v" },
                description: "Enable verbose logging");
            
            rootCommand.AddOption(source1Option);
            rootCommand.AddOption(source2Option);
            rootCommand.AddOption(outputOption);
            rootCommand.AddOption(verboseOption);
            
            rootCommand.SetHandler(async (string source1, string source2, string output, bool verbose) =>
            {
                await CompareSolutions(source1, source2, output, verbose);
            }, source1Option, source2Option, outputOption, verboseOption);
            
            return await rootCommand.InvokeAsync(args);
        }
        
        static async Task CompareSolutions(string source1Path, string source2Path, string outputPath, bool verbose)
        {
            try
            {
                Console.WriteLine("🚀 D365 Solution Comparator");
                Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                Console.WriteLine();
                
                // 1. Extract solutions
                Console.WriteLine("📦 Extracting solutions...");
                var extractor = new SolutionExtractor();
                string solution1Dir = extractor.ExtractSolution(source1Path);
                string solution2Dir = extractor.ExtractSolution(source2Path);
                Console.WriteLine("✅ Solutions extracted");
                Console.WriteLine();
                
                // 2. Parse customizations.xml
                Console.WriteLine("📄 Parsing customizations.xml files...");
                var parser = new CustomizationsXmlParser();
                
                string customizations1 = extractor.GetCustomizationsXmlPath(solution1Dir);
                string customizations2 = extractor.GetCustomizationsXmlPath(solution2Dir);
                
                var xml1 = parser.LoadCustomizationsXml(customizations1);
                var xml2 = parser.LoadCustomizationsXml(customizations2);
                
                var solution1Data = parser.ParseSolution(xml1, source1Path);
                var solution2Data = parser.ParseSolution(xml2, source2Path);
                Console.WriteLine("✅ Parsing complete");
                Console.WriteLine();
                
                // 3. Compare solutions
                Console.WriteLine("🔍 Comparing solutions...");
                var comparator = new SolutionComparator();
                var comparisonResult = comparator.Compare(solution1Data, solution2Data);
                Console.WriteLine("✅ Comparison complete");
                Console.WriteLine();
                
                // 4. Generate report
                Console.WriteLine("📊 Generating HTML report...");
                var reportGenerator = new HtmlReportGenerator();
                reportGenerator.GenerateReport(comparisonResult, outputPath);
                Console.WriteLine($"✅ Report generated: {Path.GetFullPath(outputPath)}");
                Console.WriteLine();
                
                // 5. Display summary
                DisplaySummary(comparisonResult);
                
                Console.WriteLine();
                Console.WriteLine("✅ Comparison completed successfully!");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Error: {ex.Message}");
                if (verbose)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                Console.ResetColor();
            }
        }
        
        static void DisplaySummary(ComparisonResult result)
        {
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("📈 SUMMARY");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            
            var stats = result.GetStatistics();
            
            foreach (var stat in stats)
            {
                Console.WriteLine($"{stat.ComponentType,-20} | " +
                    $"Added: {stat.Added,3} | " +
                    $"Removed: {stat.Removed,3} | " +
                    $"Modified: {stat.Modified,3}");
            }
        }
    }
}
✅ ACCEPTANCE CRITERIA
The solution MUST:

✅ Accept both ZIP files and extracted folders as input
✅ Parse and compare customizations.xml node-by-node
✅ Detect all entity-level changes (added, removed, modified)
✅ Detect all attribute-level changes per entity
✅ Detect all property changes for entities and attributes
✅ Compare forms, views, business rules, and other components
✅ Generate an HTML report with:
Multi-tab interface (Summary + Dynamic tabs per entity/app)
Collapsible tree view structure
Three-column comparison display
Status icons (✅ Same, ⚠️ Different, ➕ Added, ➖ Removed)
Color coding for different change types
✅ Support drill-down from parent to child (Entity → Attributes → Properties)
✅ Include interactive JavaScript for expanding/collapsing tree nodes
✅ Handle large solution files efficiently
🎯 DEVELOPMENT INSTRUCTIONS
Create new C# console project in VS Code:

bash
dotnet new console -n D365SolutionComparator
cd D365SolutionComparator
Add required NuGet packages:

bash
dotnet add package System.CommandLine
dotnet add package Newtonsoft.Json
dotnet add package Serilog
dotnet add package Serilog.Sinks.Console
Implement in this order:

✅ Phase 1: File extraction and XML loading
✅ Phase 2: Entity and attribute parsing
✅ Phase 3: Comparison logic
✅ Phase 4: HTML report generation
✅ Phase 5: Tree view and interactivity
✅ Phase 6: Forms, views, and other components
✅ Phase 7: Testing and refinement
Test with real D365 solution exports

Optimize for performance with large files

🔍 ADDITIONAL REQUIREMENTS
Use async/await for file I/O operations
Implement comprehensive error handling
Add logging (console and file)
Support progress reporting for large files
Make comparison logic extensible for future component types
Consider memory efficiency when parsing large XML files (use streaming if needed)
Add configuration file support for:
Components to include/exclude from comparison
Properties to ignore
Custom comparison rules
📚 REFERENCE: Customizations.xml Structure
Key XML paths to parse:

XML
<ImportExportXml>
  <Entities>
    <Entity Name="account">
      <EntityInfo>
        <entity Name="account">
          <LocalizedNames>...</LocalizedNames>
          <LocalizedCollectionNames>...</LocalizedCollectionNames>
          <Descriptions>...</Descriptions>
          <attributes>
            <attribute PhysicalName="accountid">
              <Type>uniqueidentifier</Type>
              <Name>accountid</Name>
              <LogicalName>accountid</LogicalName>
              <RequiredLevel>SystemRequired</RequiredLevel>
              ...
            </attribute>
          </attributes>
          <EntitySetName>accounts</EntitySetName>
          <IsDuplicateCheckSupported>1</IsDuplicateCheckSupported>
          <IsAuditEnabled>1</IsAuditEnabled>
          ...
        </entity>
      </EntityInfo>
      <FormXml>
        <forms>
          <systemform>
            <formid>{guid}</formid>
            <FormXml>...</FormXml>
            <type>2</type>
            ...
          </systemform>
        </forms>
      </FormXml>
      <SavedQueries>
        <savedqueries>
          <savedquery>
            <savedqueryid>{guid}</savedqueryid>
            <name>Active Accounts</name>
            <fetchxml>...</fetchxml>
            <layoutxml>...</layoutxml>
            ...
          </savedquery>
        </savedqueries>
      </SavedQueries>
      <Relationships>
        <EntityRelationships>
          <EntityRelationship Name="account_contact">
            <EntityRelationshipType>OneToMany</EntityRelationshipType>
            ...
          </EntityRelationship>
        </EntityRelationships>
      </Relationships>
    </Entity>
  </Entities>
  <roles>...</roles>
  <workflows>...</workflows>
  <EntityMaps>...</EntityMaps>
  <EntityRelationships>...</EntityRelationships>
  <optionsets>...</optionsets>
  <WebResources>...</WebResources>
  <CustomControls>...</CustomControls>
  <EntityDataProviders>...</EntityDataProviders>
  <Connectors>...</Connectors>
  <AppModules>...</AppModules>
  <Languages>...</Languages>
</ImportExportXml>
🚀 START PROMPT FOR VS CODE COPILOT
Use this exact prompt in VS Code:

Code
Create a C# .NET 8 console application called "D365SolutionComparator" that:

1. Accepts two input paths (either ZIP files or extracted folders) containing Dynamics 365 CE solution exports
2. Extracts the solutions if they are ZIP files
3. Parses the customizations.xml file from each solution
4. Performs a deep, node-by-node XML comparison to detect:
   - Entities added/removed/modified
   - Attributes added/removed/modified per entity
   - All entity and attribute property changes
   - Forms, views, business rules, and other component changes
5. Generates an interactive HTML report with:
   - Multi-tab interface (Summary tab + dynamic tabs per entity)
   - Collapsible tree view showing hierarchical differences
   - Three-column comparison (Property | Source 1 | Source 2 | Status)
   - Status icons: ✅ Same, ⚠️ Different, ➕ Added, ➖ Removed
   - Color-coded changes

Use System.CommandLine for CLI arguments, LINQ to XML for parsing, and generate standalone HTML/CSS/JS files for the report.

Structure the code with:
- Models/ (ComparisonResult, EntityDefinition, AttributeDefinition, etc.)
- Parsers/ (SolutionExtractor, CustomizationsXmlParser, EntityParser, etc.)
- Comparers/ (EntityComparer, AttributeComparer, FormComparer, etc.)
- ReportGenerators/ (HtmlReportGenerator with embedded CSS/JS)

Start with the basic structure, entity/attribute comparison, and HTML generation.