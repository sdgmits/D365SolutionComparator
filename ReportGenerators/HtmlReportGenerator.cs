using System.Text;
using D365SolutionComparator.Models;

namespace D365SolutionComparator.ReportGenerators;

public class HtmlReportGenerator : IReportGenerator
{
    public void GenerateReport(ComparisonResult result, string outputPath)
    {
        var html = new StringBuilder();
        
        // Generate HTML structure
        GenerateHtmlHeader(html);
        GenerateTabNavigation(html, result);
        GenerateSummaryTab(html, result);
        GenerateEntityTabs(html, result);
        GenerateOtherComponentTabs(html, result);
        GenerateHtmlFooter(html);
        
        // Write to file
        File.WriteAllText(outputPath, html.ToString());
        
        // Generate accompanying CSS and JS files in the same directory
        var directory = Path.GetDirectoryName(outputPath) ?? Directory.GetCurrentDirectory();
        GenerateStylesheet(directory);
        GenerateJavaScript(directory);
        
        Console.WriteLine($"  HTML report: {Path.GetFullPath(outputPath)}");
        Console.WriteLine($"  CSS file: {Path.Combine(directory, "styles.css")}");
        Console.WriteLine($"  JS file: {Path.Combine(directory, "script.js")}");
    }
    
    private void GenerateHtmlHeader(StringBuilder html)
    {
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang='en'>");
        html.AppendLine("<head>");
        html.AppendLine("    <meta charset='UTF-8'>");
        html.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        html.AppendLine("    <title>D365 Solution Comparison Report</title>");
        html.AppendLine("    <link rel='stylesheet' href='styles.css'>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");
        html.AppendLine("    <div class='header'>");
        html.AppendLine("        <h1>🎯 D365 Solution Comparison Report</h1>");
        html.AppendLine("    </div>");
    }
    
    private void GenerateTabNavigation(StringBuilder html, ComparisonResult result)
    {
        html.AppendLine("    <div class='tab-container'>");
        html.AppendLine("        <div class='tab-nav'>");
        html.AppendLine("            <button class='tab-button active' onclick='openTab(event, \"summary\")'>📊 Summary</button>");
        
        // Entity tabs
        var entities = result.EntityComparisons.OrderBy(e => e.ComponentName).ToList();
        foreach (var entity in entities)
        {
            var icon = GetChangeIcon(entity.ChangeType);
            html.AppendLine($"            <button class='tab-button' onclick='openTab(event, \"entity_{EscapeForId(entity.ComponentName)}\")'>{icon} {entity.ComponentName}</button>");
        }
        
        // Other component type tabs
        if (result.WebResourceComparisons.Any())
        {
            html.AppendLine($"            <button class='tab-button' onclick='openTab(event, \"webresources\")'>🌐 Web Resources ({result.WebResourceComparisons.Count})</button>");
        }
        
        if (result.ProcessComparisons.Any())
        {
            html.AppendLine($"            <button class='tab-button' onclick='openTab(event, \"processes\")'>⚙️ Processes ({result.ProcessComparisons.Count})</button>");
        }
        
        var optionSets = result.ChildComparisons.Where(c => c.ComponentType == "OptionSet").ToList();
        if (optionSets.Any())
        {
            html.AppendLine($"            <button class='tab-button' onclick='openTab(event, \"optionsets\")'>📋 Option Sets ({optionSets.Count})</button>");
        }
        
        var securityRoles = result.ChildComparisons.Where(c => c.ComponentType == "SecurityRole").ToList();
        if (securityRoles.Any())
        {
            html.AppendLine($"            <button class='tab-button' onclick='openTab(event, \"securityroles\")'>🔒 Security Roles ({securityRoles.Count})</button>");
        }
        
        var appModules = result.ChildComparisons.Where(c => c.ComponentType == "AppModule").ToList();
        if (appModules.Any())
        {
            html.AppendLine($"            <button class='tab-button' onclick='openTab(event, \"appmodules\")'>📱 App Modules ({appModules.Count})</button>");
        }
        
        html.AppendLine("        </div>");
    }
    
    private void GenerateSummaryTab(StringBuilder html, ComparisonResult result)
    {
        html.AppendLine("        <div id='summary' class='tab-content' style='display:block;'>");
        html.AppendLine("            <h2>📊 COMPARISON SUMMARY</h2>");
        html.AppendLine("            <div class='info-box'>");
        html.AppendLine($"                <p><strong>Source 1:</strong> {result.Source1Path}</p>");
        html.AppendLine($"                <p><strong>Source 2:</strong> {result.Source2Path}</p>");
        html.AppendLine($"                <p><strong>Comparison Date:</strong> {result.ComparisonDate:yyyy-MM-dd HH:mm:ss}</p>");
        html.AppendLine("            </div>");
        
        // Hierarchical statistics table
        html.AppendLine("            <h3>📈 CHANGE STATISTICS</h3>");
        html.AppendLine("            <p class='info-text'>Click on component types to expand/collapse detailed changes</p>");
        html.AppendLine("            <table class='summary-table hierarchical-table'>");
        html.AppendLine("                <thead>");
        html.AppendLine("                    <tr>");
        html.AppendLine("                        <th>Component Type</th>");
        html.AppendLine("                        <th>Added</th>");
        html.AppendLine("                        <th>Removed</th>");
        html.AppendLine("                        <th>Modified</th>");
        html.AppendLine("                        <th>Unchanged</th>");
        html.AppendLine("                        <th>Total</th>");
        html.AppendLine("                    </tr>");
        html.AppendLine("                </thead>");
        html.AppendLine("                <tbody>");
        
        var hierarchicalStats = result.GetHierarchicalStatistics();
        foreach (var parentStat in hierarchicalStats)
        {
            var unchanged = parentStat.Total - parentStat.Added - parentStat.Removed - parentStat.Modified;
            var hasInstances = parentStat.ComponentInstances.Any();
            var expandIcon = hasInstances ? "▶" : "";
            var rowClass = hasInstances ? "expandable-row" : "";
            
            html.AppendLine($"                    <tr class='{rowClass}' data-parent='{EscapeForId(parentStat.ComponentType)}'>");
            html.AppendLine($"                        <td class='component-name'><span class='expand-icon'>{expandIcon}</span> <strong>{GetComponentDisplayName(parentStat.ComponentType)}</strong></td>");
            html.AppendLine($"                        <td class='added'>{(parentStat.Added > 0 ? parentStat.Added.ToString() : "")}</td>");
            html.AppendLine($"                        <td class='removed'>{(parentStat.Removed > 0 ? parentStat.Removed.ToString() : "")}</td>");
            html.AppendLine($"                        <td class='modified'>{(parentStat.Modified > 0 ? parentStat.Modified.ToString() : "")}</td>");
            html.AppendLine($"                        <td class='unchanged'>{(unchanged > 0 ? unchanged.ToString() : "")}</td>");
            html.AppendLine($"                        <td><strong>{parentStat.Total}</strong></td>");
            html.AppendLine("                    </tr>");
            
            // Show component instances (like individual entities) when there are changes
            if (hasInstances)
            {
                foreach (var instance in parentStat.ComponentInstances)
                {
                    var instanceIcon = GetChangeIcon(instance.ChangeType);
                    var instanceRowClass = instance.HasChildChanges ? "instance-row expandable-instance-row" : "instance-row";
                    var instanceExpandIcon = instance.HasChildChanges ? "▶" : "";
                    var tabId = $"{parentStat.ComponentType.ToLower()}_{EscapeForId(instance.Name)}";
                    
                    html.AppendLine($"                    <tr class='{instanceRowClass}' data-child-of='{EscapeForId(parentStat.ComponentType)}' data-instance='{EscapeForId(instance.Name)}' style='display:none;'>");
                    html.AppendLine($"                        <td class='component-name instance-component'>");
                    html.AppendLine($"                            <span class='expand-icon'>{instanceExpandIcon}</span>");
                    html.AppendLine($"                            {instanceIcon} <a href='#{tabId}' onclick='navigateToTab(event, \"{tabId}\")'>{instance.Name}</a>");
                    html.AppendLine($"                        </td>");
                    html.AppendLine($"                        <td class='added'>{(instance.ChangeType == ChangeType.Added ? "1" : "")}</td>");
                    html.AppendLine($"                        <td class='removed'>{(instance.ChangeType == ChangeType.Removed ? "1" : "")}</td>");
                    html.AppendLine($"                        <td class='modified'>{(instance.ChangeType == ChangeType.Modified ? "1" : "")}</td>");
                    html.AppendLine($"                        <td class='unchanged'></td>");
                    html.AppendLine($"                        <td>1</td>");
                    html.AppendLine("                    </tr>");
                    
                    // Show child changes for this instance (like Attributes, Forms, etc.)
                    if (instance.HasChildChanges)
                    {
                        var instanceChildStats = instance.ChildStatistics.Values
                            .Where(c => c.Added > 0 || c.Removed > 0 || c.Modified > 0)
                            .OrderBy(c => c.ComponentType);
                        
                        foreach (var childStat in instanceChildStats)
                        {
                            html.AppendLine($"                    <tr class='grandchild-row' data-grandchild-of='{EscapeForId(instance.Name)}' style='display:none;'>");
                            html.AppendLine($"                        <td class='component-name grandchild-component'>↳ {childStat.ComponentType}</td>");
                            html.AppendLine($"                        <td class='added'>{(childStat.Added > 0 ? childStat.Added.ToString() : "")}</td>");
                            html.AppendLine($"                        <td class='removed'>{(childStat.Removed > 0 ? childStat.Removed.ToString() : "")}</td>");
                            html.AppendLine($"                        <td class='modified'>{(childStat.Modified > 0 ? childStat.Modified.ToString() : "")}</td>");
                            html.AppendLine($"                        <td class='unchanged'></td>");
                            html.AppendLine($"                        <td>{childStat.Total}</td>");
                            html.AppendLine("                    </tr>");
                        }
                    }
                }
            }
        }
        
        html.AppendLine("                </tbody>");
        html.AppendLine("            </table>");
        
        // Critical changes section
        GenerateCriticalChanges(html, result);
        
        html.AppendLine("        </div>");
    }
    
    private string GetComponentDisplayName(string componentType)
    {
        return componentType switch
        {
            "Entity" => "📦 Entities",
            "AppModule" => "📱 Model-Driven Apps (MDAs)",
            "WebResource" => "🌐 Web Resources",
            "Process" => "⚙️ Processes",
            "OptionSet" => "📋 Option Sets",
            "SecurityRole" => "🔒 Security Roles",
            _ => componentType
        };
    }
    
    private void GenerateCriticalChanges(StringBuilder html, ComparisonResult result)
    {
        html.AppendLine("            <h3>🔍 CRITICAL CHANGES</h3>");
        html.AppendLine("            <div class='critical-changes'>");
        
        var criticalChanges = new List<string>();
        
        // Find entities that were added
        foreach (var entity in result.EntityComparisons.Where(e => e.ChangeType == ChangeType.Added))
        {
            criticalChanges.Add($"➕ Entity '{entity.ComponentName}' was added in Solution 2");
        }
        
        // Find entities that were removed
        foreach (var entity in result.EntityComparisons.Where(e => e.ChangeType == ChangeType.Removed))
        {
            criticalChanges.Add($"➖ Entity '{entity.ComponentName}' was removed from Solution 2");
        }
        
        // Find entities with significant modifications
        foreach (var entity in result.EntityComparisons.Where(e => e.ChangeType == ChangeType.Modified).Take(10))
        {
            var attributeChanges = entity.ChildComparisons.Count(c => c.ComponentType == "Attribute" && c.ChangeType != ChangeType.Unchanged);
            if (attributeChanges > 0)
            {
                criticalChanges.Add($"⚠️ Entity '{entity.ComponentName}' has {attributeChanges} attribute change(s)");
            }
        }
        
        if (criticalChanges.Any())
        {
            html.AppendLine("                <ul>");
            foreach (var change in criticalChanges.Take(20))
            {
                html.AppendLine($"                    <li>{change}</li>");
            }
            html.AppendLine("                </ul>");
        }
        else
        {
            html.AppendLine("                <p>No critical changes detected.</p>");
        }
        
        html.AppendLine("            </div>");
    }
    
    private void GenerateEntityTabs(StringBuilder html, ComparisonResult result)
    {
        var entities = result.EntityComparisons.OrderBy(e => e.ComponentName).ToList();
        
        foreach (var entity in entities)
        {
            html.AppendLine($"        <div id='entity_{EscapeForId(entity.ComponentName)}' class='tab-content'>");
            html.AppendLine($"            <h2>{GetChangeIcon(entity.ChangeType)} Entity: {entity.ComponentName}</h2>");
            
            if (entity.ChangeType == ChangeType.Added)
            {
                html.AppendLine("            <div class='info-box added-box'>");
                html.AppendLine("                <p><strong>This entity was added in Solution 2</strong></p>");
                html.AppendLine("            </div>");
            }
            else if (entity.ChangeType == ChangeType.Removed)
            {
                html.AppendLine("            <div class='info-box removed-box'>");
                html.AppendLine("                <p><strong>This entity was removed from Solution 2</strong></p>");
                html.AppendLine("            </div>");
            }
            else
            {
                // Generate tree view
                GenerateEntityTreeView(html, entity, result);
            }
            
            html.AppendLine("        </div>");
        }
    }
    
    private void GenerateEntityTreeView(StringBuilder html, ComparisonResult entity, ComparisonResult parentResult)
    {
        var solution1Name = GetSolutionName(parentResult.Source1Path);
        var solution2Name = GetSolutionName(parentResult.Source2Path);
        
        html.AppendLine("            <div class='tree-view'>");
        html.AppendLine("                <ul class='tree'>");
        
        // Entity Properties - Table Format
        if (entity.PropertyChanges.Any())
        {
            html.AppendLine("                    <li>");
            html.AppendLine("                        <span class='tree-toggle'>🔧 Entity Properties</span>");
            html.AppendLine("                        <div class='nested'>");
            html.AppendLine("                            <div class='table-filter-container'>");
            html.AppendLine("                                <input type='text' class='property-filter' placeholder='🔍 Filter properties...' onkeyup='filterPropertyTable(this)' />");
            html.AppendLine("                                <label class='filter-checkbox'>");
            html.AppendLine("                                    <input type='checkbox' class='changed-only-filter' onchange='filterChangedOnly(this)' /> Show changed only");
            html.AppendLine("                                </label>");
            html.AppendLine("                            </div>");
            html.AppendLine("                            <table class='property-table'>");
            html.AppendLine("                                <thead>");
            html.AppendLine("                                    <tr>");
            html.AppendLine("                                        <th>Property Name</th>");
            html.AppendLine($"                                        <th>{EscapeHtml(solution1Name)}</th>");
            html.AppendLine($"                                        <th>{EscapeHtml(solution2Name)}</th>");
            html.AppendLine("                                        <th>Is Changed?</th>");
            html.AppendLine("                                    </tr>");
            html.AppendLine("                                </thead>");
            html.AppendLine("                                <tbody>");
            
            foreach (var prop in entity.PropertyChanges.OrderBy(p => p.Key))
            {
                var rowClass = prop.Value.IsDifferent ? "changed-row" : "unchanged-row";
                var changeIcon = prop.Value.IsDifferent ? "⚠️ Yes" : "✅ No";
                var changeClass = prop.Value.IsDifferent ? "status-changed" : "status-unchanged";
                
                html.AppendLine($"                                    <tr class='{rowClass}'>");
                html.AppendLine($"                                        <td class='property-name'><strong>{EscapeHtml(prop.Key)}</strong></td>");
                html.AppendLine($"                                        <td class='value-cell'>{EscapeHtml(prop.Value.SourceValue)}</td>");
                html.AppendLine($"                                        <td class='value-cell'>{EscapeHtml(prop.Value.TargetValue)}</td>");
                html.AppendLine($"                                        <td class='{changeClass}'>{changeIcon}</td>");
                html.AppendLine("                                    </tr>");
            }
            
            html.AppendLine("                                </tbody>");
            html.AppendLine("                            </table>");
            html.AppendLine("                        </div>");
            html.AppendLine("                    </li>");
        }
        
        // Attributes
        var attributes = entity.ChildComparisons.Where(c => c.ComponentType == "Attribute").OrderBy(a => a.ComponentName).ToList();
        if (attributes.Any())
        {
            var addedCount = attributes.Count(a => a.ChangeType == ChangeType.Added);
            var removedCount = attributes.Count(a => a.ChangeType == ChangeType.Removed);
            var modifiedCount = attributes.Count(a => a.ChangeType == ChangeType.Modified);
            
            html.AppendLine("                    <li>");
            html.AppendLine($"                        <span class='tree-toggle'>📝 Attributes ({attributes.Count} total");
            if (addedCount > 0) html.Append($", {addedCount} added");
            if (removedCount > 0) html.Append($", {removedCount} removed");
            if (modifiedCount > 0) html.Append($", {modifiedCount} modified");
            html.AppendLine(")</span>");
            html.AppendLine("                        <ul class='nested'>");
            
            foreach (var attr in attributes)
            {
                GenerateComponentTableItem(html, attr, solution1Name, solution2Name);
            }
            
            html.AppendLine("                        </ul>");
            html.AppendLine("                    </li>");
        }
        
        // Forms
        GenerateComponentSection(html, entity, "Form", "📄 Forms", solution1Name, solution2Name);
        
        // Views
        GenerateComponentSection(html, entity, "View", "👁️ Views", solution1Name, solution2Name);
        
        // Relationships
        GenerateComponentSection(html, entity, "Relationship", "🔗 Relationships", solution1Name, solution2Name);
        
        // Business Rules
        GenerateComponentSection(html, entity, "BusinessRule", "⚙️ Business Rules", solution1Name, solution2Name);
        
        html.AppendLine("                </ul>");
        html.AppendLine("            </div>");
    }
    
    private void GenerateComponentSection(StringBuilder html, ComparisonResult parent, string componentType, string displayName, string solution1Name, string solution2Name)
    {
        var components = parent.ChildComparisons.Where(c => c.ComponentType == componentType).OrderBy(c => c.ComponentName).ToList();
        if (!components.Any()) return;
        
        var addedCount = components.Count(c => c.ChangeType == ChangeType.Added);
        var removedCount = components.Count(c => c.ChangeType == ChangeType.Removed);
        var modifiedCount = components.Count(c => c.ChangeType == ChangeType.Modified);
        
        html.AppendLine("                    <li>");
        html.AppendLine($"                        <span class='tree-toggle'>{displayName} ({components.Count} total");
        if (addedCount > 0) html.Append($", {addedCount} added");
        if (removedCount > 0) html.Append($", {removedCount} removed");
        if (modifiedCount > 0) html.Append($", {modifiedCount} modified");
        html.AppendLine(")</span>");
        html.AppendLine("                        <ul class='nested'>");
        
        foreach (var component in components)
        {
            GenerateComponentTableItem(html, component, solution1Name, solution2Name);
        }
        
        html.AppendLine("                        </ul>");
        html.AppendLine("                    </li>");
    }
    
    private void GenerateComponentTableItem(StringBuilder html, ComparisonResult component, string solution1Name, string solution2Name)
    {
        var icon = GetChangeIcon(component.ChangeType);
        
        // Determine if component has changes
        var hasChanges = component.ChangeType != ChangeType.Unchanged || 
                        component.PropertyChanges.Any(p => p.Value.IsDifferent);
        var changeStatusBadge = hasChanges ? " <span style='color: #ffc107; font-weight: bold;'>[Is Changed: ⚠️ Yes]</span>" : 
                                            " <span style='color: #6c757d;'>[Is Changed: ✅ No]</span>";
        
        // Always show components in table format
        html.AppendLine("                            <li>");
        html.AppendLine($"                                <span class='tree-toggle'>{icon} {component.ComponentName}{changeStatusBadge}</span>");
        html.AppendLine("                                <div class='nested'>");
        
        // Only show filters if there are properties to filter
        if (component.PropertyChanges.Any())
        {
            html.AppendLine("                                    <div class='table-filter-container'>");
            html.AppendLine("                                        <input type='text' class='property-filter' placeholder='🔍 Filter properties...' onkeyup='filterPropertyTable(this)' />");
            html.AppendLine("                                        <label class='filter-checkbox'>");
            html.AppendLine("                                            <input type='checkbox' class='changed-only-filter' onchange='filterChangedOnly(this)' /> Show changed only");
            html.AppendLine("                                        </label>");
            html.AppendLine("                                    </div>");
        }
        
        html.AppendLine("                                    <table class='property-table'>");
        html.AppendLine("                                        <thead>");
        html.AppendLine("                                            <tr>");
        html.AppendLine("                                                <th>Property Name</th>");
        html.AppendLine($"                                                <th>{EscapeHtml(solution1Name)}</th>");
        html.AppendLine($"                                                <th>{EscapeHtml(solution2Name)}</th>");
        html.AppendLine("                                                <th>Is Changed?</th>");
        html.AppendLine("                                            </tr>");
        html.AppendLine("                                        </thead>");
        html.AppendLine("                                        <tbody>");
        
        if (component.PropertyChanges.Any())
        {
            // Show all properties for items with property changes
            foreach (var prop in component.PropertyChanges.OrderBy(p => p.Key))
            {
                var rowClass = prop.Value.IsDifferent ? "changed-row" : "unchanged-row";
                var changeIcon = prop.Value.IsDifferent ? "⚠️ Yes" : "✅ No";
                var changeClass = prop.Value.IsDifferent ? "status-changed" : "status-unchanged";
                
                html.AppendLine($"                                            <tr class='{rowClass}'>");
                html.AppendLine($"                                                <td class='property-name'><strong>{EscapeHtml(prop.Key)}</strong></td>");
                html.AppendLine($"                                                <td class='value-cell'>{EscapeHtml(prop.Value.SourceValue)}</td>");
                html.AppendLine($"                                                <td class='value-cell'>{EscapeHtml(prop.Value.TargetValue)}</td>");
                html.AppendLine($"                                                <td class='{changeClass}'>{changeIcon}</td>");
                html.AppendLine("                                            </tr>");
            }
        }
        else
        {
            // For Added/Removed items without property details, show a summary row
            var statusText = component.ChangeType == ChangeType.Added ? "Added in " + solution2Name : 
                           component.ChangeType == ChangeType.Removed ? "Removed from " + solution2Name : 
                           "No changes";
            var value1 = component.ChangeType == ChangeType.Removed ? "Exists" : "-";
            var value2 = component.ChangeType == ChangeType.Added ? "Exists" : "-";
            
            html.AppendLine($"                                            <tr class='changed-row'>");
            html.AppendLine($"                                                <td class='property-name'><strong>Status</strong></td>");
            html.AppendLine($"                                                <td class='value-cell'>{value1}</td>");
            html.AppendLine($"                                                <td class='value-cell'>{value2}</td>");
            html.AppendLine($"                                                <td class='status-changed'>⚠️ Yes</td>");
            html.AppendLine("                                            </tr>");
        }
        
        html.AppendLine("                                        </tbody>");
        html.AppendLine("                                    </table>");
        html.AppendLine("                                </div>");
        html.AppendLine("                            </li>");
    }
    
    private void GenerateOtherComponentTabs(StringBuilder html, ComparisonResult result)
    {
        // Web Resources tab
        if (result.WebResourceComparisons.Any())
        {
            GenerateSimpleComponentTab(html, "webresources", "🌐 Web Resources", 
                result.WebResourceComparisons.OrderBy(w => w.ComponentName).ToList());
        }
        
        // Processes tab
        if (result.ProcessComparisons.Any())
        {
            GenerateSimpleComponentTab(html, "processes", "⚙️ Processes",
                result.ProcessComparisons.OrderBy(p => p.ComponentName).ToList());
        }
        
        // Option Sets tab
        var optionSets = result.ChildComparisons.Where(c => c.ComponentType == "OptionSet").OrderBy(o => o.ComponentName).ToList();
        if (optionSets.Any())
        {
            GenerateSimpleComponentTab(html, "optionsets", "📋 Global Option Sets", optionSets);
        }
        
        // Security Roles tab
        var securityRoles = result.ChildComparisons.Where(c => c.ComponentType == "SecurityRole").OrderBy(s => s.ComponentName).ToList();
        if (securityRoles.Any())
        {
            GenerateSimpleComponentTab(html, "securityroles", "🔒 Security Roles", securityRoles);
        }
        
        // App Modules tab
        var appModules = result.ChildComparisons.Where(c => c.ComponentType == "AppModule").OrderBy(a => a.ComponentName).ToList();
        if (appModules.Any())
        {
            GenerateSimpleComponentTab(html, "appmodules", "📱 App Modules", appModules);
        }
    }
    
    private void GenerateSimpleComponentTab(StringBuilder html, string tabId, string title, List<ComparisonResult> components)
    {
        html.AppendLine($"        <div id='{tabId}' class='tab-content'>");
        html.AppendLine($"            <h2>{title}</h2>");
        html.AppendLine("            <ul class='component-list'>");
        
        foreach (var component in components)
        {
            var icon = GetChangeIcon(component.ChangeType);
            var cssClass = component.ChangeType.ToString().ToLower();
            
            html.AppendLine($"                <li class='{cssClass}'>");
            html.AppendLine($"                    <strong>{icon} {component.ComponentName}</strong>");
            
            if (component.PropertyChanges.Any(p => p.Value.IsDifferent))
            {
                html.AppendLine("                    <ul class='property-list'>");
                foreach (var prop in component.PropertyChanges.Where(p => p.Value.IsDifferent))
                {
                    html.AppendLine("                        <li>");
                    html.AppendLine($"                            <strong>{prop.Key}:</strong> ");
                    html.AppendLine($"                            <span class='source-value'>{EscapeHtml(prop.Value.SourceValue)}</span>");
                    html.AppendLine($"                            <span class='arrow'>→</span>");
                    html.AppendLine($"                            <span class='target-value'>{EscapeHtml(prop.Value.TargetValue)}</span>");
                    html.AppendLine("                        </li>");
                }
                html.AppendLine("                    </ul>");
            }
            
            html.AppendLine("                </li>");
        }
        
        html.AppendLine("            </ul>");
        html.AppendLine("        </div>");
    }
    
    private void GenerateHtmlFooter(StringBuilder html)
    {
        html.AppendLine("    </div>"); // Close tab-container
        html.AppendLine("    <script src='script.js'></script>");
        html.AppendLine("</body>");
        html.AppendLine("</html>");
    }
    
    private string GetChangeIcon(ChangeType changeType)
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
    
    private string EscapeForId(string? input)
    {
        if (string.IsNullOrEmpty(input)) return "unknown";
        return input.Replace("_", "").Replace(" ", "").Replace(".", "").ToLower();
    }
    
    private string GetSolutionName(string? path)
    {
        if (string.IsNullOrEmpty(path)) return "Solution";
        
        var fileName = Path.GetFileNameWithoutExtension(path);
        if (string.IsNullOrEmpty(fileName)) return "Solution";
        
        // Try to extract solution name from filename pattern like "SolutionName_1_0_0_1"
        var parts = fileName.Split('_');
        if (parts.Length > 0)
        {
            return parts[0];
        }
        
        return fileName;
    }
    
    private string EscapeHtml(string? input)
    {
        if (string.IsNullOrEmpty(input)) return "null";
        return input
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }
    
    private void GenerateStylesheet(string directory)
    {
        var css = @":root {
    --color-added: #28a745;
    --color-removed: #dc3545;
    --color-modified: #ffc107;
    --color-unchanged: #6c757d;
    --color-primary: #007bff;
    --color-bg: #f5f7fa;
    --color-white: #ffffff;
    --color-border: #dee2e6;
    --color-text: #212529;
}

* {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
}

body {
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    background-color: var(--color-bg);
    color: var(--color-text);
    line-height: 1.6;
}

.header {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    padding: 20px;
    text-align: center;
    box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.header h1 {
    margin: 0;
    font-size: 2em;
}

.tab-container {
    max-width: 1400px;
    margin: 20px auto;
    background: var(--color-white);
    border-radius: 8px;
    box-shadow: 0 2px 10px rgba(0,0,0,0.1);
    overflow: hidden;
}

.tab-nav {
    background-color: #2c3e50;
    padding: 10px;
    display: flex;
    overflow-x: auto;
    gap: 5px;
}

.tab-nav::-webkit-scrollbar {
    height: 8px;
}

.tab-nav::-webkit-scrollbar-track {
    background: #34495e;
}

.tab-nav::-webkit-scrollbar-thumb {
    background: #3498db;
    border-radius: 4px;
}

.tab-button {
    background-color: transparent;
    color: white;
    border: none;
    padding: 10px 20px;
    cursor: pointer;
    transition: background-color 0.3s;
    white-space: nowrap;
    border-radius: 4px;
    font-size: 14px;
}

.tab-button:hover {
    background-color: #34495e;
}

.tab-button.active {
    background-color: #3498db;
    font-weight: bold;
}

.tab-content {
    display: none;
    padding: 30px;
    animation: fadeIn 0.3s;
}

@keyframes fadeIn {
    from { opacity: 0; }
    to { opacity: 1; }
}

.info-box {
    background-color: #e7f3ff;
    border-left: 4px solid var(--color-primary);
    padding: 15px;
    margin: 20px 0;
    border-radius: 4px;
}

.added-box {
    background-color: #d4edda;
    border-left-color: var(--color-added);
}

.removed-box {
    background-color: #f8d7da;
    border-left-color: var(--color-removed);
}

h2 {
    color: #2c3e50;
    margin-bottom: 20px;
    padding-bottom: 10px;
    border-bottom: 2px solid var(--color-border);
}

h3 {
    color: #34495e;
    margin-top: 30px;
    margin-bottom: 15px;
}

.summary-table {
    width: 100%;
    border-collapse: collapse;
    margin: 20px 0;
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.summary-table th,
.summary-table td {
    padding: 12px;
    text-align: left;
    border-bottom: 1px solid var(--color-border);
}

.summary-table th {
    background-color: #2c3e50;
    color: white;
    font-weight: bold;
}

.summary-table tr:hover {
    background-color: #f8f9fa;
}

.summary-table tr:last-child td {
    border-bottom: none;
}

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

.unchanged {
    color: var(--color-unchanged);
}

/* Hierarchical Table Styles */
.hierarchical-table .expandable-row {
    cursor: pointer;
    transition: background-color 0.2s;
}

.hierarchical-table .expandable-row:hover {
    background-color: #e8f4f8;
}

.hierarchical-table .expand-icon {
    display: inline-block;
    width: 16px;
    transition: transform 0.3s;
    font-size: 0.8em;
    margin-right: 5px;
}

.hierarchical-table .expandable-row.expanded .expand-icon {
    transform: rotate(90deg);
}

.hierarchical-table .expandable-instance-row.expanded .expand-icon {
    transform: rotate(90deg);
}

.hierarchical-table .child-row {
    background-color: #f8f9fa;
    font-size: 0.95em;
}

.hierarchical-table .instance-row {
    background-color: #f8f9fa;
    font-size: 0.95em;
}

.hierarchical-table .instance-row:hover {
    background-color: #e0e7eb;
}

.hierarchical-table .expandable-instance-row {
    cursor: pointer;
}

.hierarchical-table .grandchild-row {
    background-color: #ffffff;
    font-size: 0.9em;
}

.hierarchical-table .child-component {
    padding-left: 30px;
    color: #555;
}

.hierarchical-table .instance-component {
    padding-left: 40px;
    color: #333;
}

.hierarchical-table .grandchild-component {
    padding-left: 70px;
    color: #666;
    font-size: 0.9em;
}

.hierarchical-table .component-name {
    font-weight: 500;
}

.hierarchical-table .instance-component a {
    color: #2c3e50;
    text-decoration: none;
    font-weight: 500;
}

.hierarchical-table .instance-component a:hover {
    color: #3498db;
    text-decoration: underline;
}

.info-text {
    color: #666;
    font-style: italic;
    margin-bottom: 10px;
    font-size: 0.9em;
}

.critical-changes {
    background-color: #fff3cd;
    border-left: 4px solid var(--color-modified);
    padding: 15px;
    border-radius: 4px;
    margin: 20px 0;
}

.critical-changes ul {
    padding-left: 20px;
    margin: 10px 0;
}

.critical-changes li {
    margin: 5px 0;
}

/* Tree View Styles */
.tree-view {
    margin: 20px 0;
}

.tree-view ul {
    list-style-type: none;
    padding-left: 20px;
}

.tree-view .tree {
    padding-left: 0;
}

.tree-view li {
    margin: 5px 0;
}

.tree-toggle {
    cursor: pointer;
    user-select: none;
    padding: 5px;
    display: inline-block;
    border-radius: 4px;
    transition: background-color 0.2s;
}

.tree-toggle:hover {
    background-color: #f0f0f0;
}

.tree-toggle::before {
    content: '▶ ';
    display: inline-block;
    margin-right: 6px;
    transition: transform 0.3s;
}

.tree-toggle.expanded::before {
    transform: rotate(90deg);
}

.nested {
    display: none;
}

.nested.active {
    display: block;
}

/* Property Table Styles */
.property-table {
    width: 100%;
    border-collapse: collapse;
    margin: 15px 0;
    background-color: white;
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    font-size: 0.9em;
}

.property-table thead {
    background-color: #34495e;
    color: white;
}

.property-table th {
    padding: 12px;
    text-align: left;
    font-weight: bold;
    border-bottom: 2px solid #2c3e50;
}

.property-table td {
    padding: 10px 12px;
    border-bottom: 1px solid #e0e0e0;
    vertical-align: top;
}

.property-table tbody tr:hover {
    background-color: #f8f9fa;
}

.property-table .property-name {
    color: #2c3e50;
    min-width: 200px;
}

.property-table .value-cell {
    font-family: 'Consolas', 'Monaco', monospace;
    font-size: 0.95em;
    color: #555;
    max-width: 300px;
    word-wrap: break-word;
}

.property-table .changed-row {
    background-color: #fff3cd15;
}

.property-table .unchanged-row {
    opacity: 0.8;
}

.property-table .status-changed {
    color: var(--color-modified);
    font-weight: bold;
    text-align: center;
}

.property-table .status-unchanged {
    color: var(--color-unchanged);
    text-align: center;
}

/* Table Filter Styles */
.table-filter-container {
    margin: 15px 0 10px 0;
    display: flex;
    gap: 15px;
    align-items: center;
    flex-wrap: wrap;
}

.property-filter {
    flex: 1;
    min-width: 250px;
    padding: 8px 12px;
    border: 1px solid #ddd;
    border-radius: 4px;
    font-size: 0.9em;
    transition: border-color 0.3s;
}

.property-filter:focus {
    outline: none;
    border-color: #3498db;
    box-shadow: 0 0 0 3px rgba(52, 152, 219, 0.1);
}

.filter-checkbox {
    display: flex;
    align-items: center;
    gap: 6px;
    font-size: 0.9em;
    cursor: pointer;
    user-select: none;
}

.filter-checkbox input[type=""checkbox""] {
    cursor: pointer;
    width: 16px;
    height: 16px;
}

.property-table tbody tr.hidden {
    display: none;
}

.value-comparison {
    font-family: 'Consolas', 'Monaco', monospace;
    font-size: 0.9em;
}

.source-value {
    color: var(--color-removed);
    text-decoration: line-through;
}

.target-value {
    color: var(--color-added);
    font-weight: bold;
}

.arrow {
    margin: 0 8px;
    color: #666;
}

.different {
    background-color: #fff3cd20;
    padding: 5px;
    border-radius: 4px;
    margin: 2px 0;
}

.same {
    opacity: 0.7;
}

/* Component List Styles */
.component-list {
    list-style-type: none;
    padding: 0;
}

.component-list > li {
    padding: 15px;
    margin: 10px 0;
    border: 1px solid var(--color-border);
    border-radius: 4px;
    background-color: white;
}

.component-list > li.added {
    border-left: 4px solid var(--color-added);
    background-color: #d4edda20;
}

.component-list > li.removed {
    border-left: 4px solid var(--color-removed);
    background-color: #f8d7da20;
}

.component-list > li.modified {
    border-left: 4px solid var(--color-modified);
    background-color: #fff3cd20;
}

.property-list {
    list-style-type: none;
    padding-left: 20px;
    margin-top: 10px;
}

.property-list li {
    padding: 5px;
    margin: 5px 0;
}

@media (max-width: 768px) {
    .tab-nav {
        flex-wrap: wrap;
    }
    
    .tab-content {
        padding: 15px;
    }
    
    .summary-table {
        font-size: 0.9em;
    }
    
    .summary-table th,
    .summary-table td {
        padding: 8px;
    }
}
";
        
        File.WriteAllText(Path.Combine(directory, "styles.css"), css);
    }
    
    private void GenerateJavaScript(string directory)
    {
        var js = @"// Tab switching
function openTab(evt, tabName) {
    var i, tabcontent, tabbuttons;
    
    // Hide all tab contents
    tabcontent = document.getElementsByClassName('tab-content');
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = 'none';
    }
    
    // Remove active class from all buttons
    tabbuttons = document.getElementsByClassName('tab-button');
    for (i = 0; i < tabbuttons.length; i++) {
        tabbuttons[i].className = tabbuttons[i].className.replace(' active', '');
    }
    
    // Show current tab and mark button as active
    document.getElementById(tabName).style.display = 'block';
    evt.currentTarget.className += ' active';
}

// Navigate to a specific tab and update active button
function navigateToTab(evt, tabName) {
    evt.preventDefault();
    evt.stopPropagation();
    
    var i, tabcontent, tabbuttons;
    
    // Hide all tab contents
    tabcontent = document.getElementsByClassName('tab-content');
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = 'none';
    }
    
    // Remove active class from all buttons
    tabbuttons = document.getElementsByClassName('tab-button');
    for (i = 0; i < tabbuttons.length; i++) {
        tabbuttons[i].className = tabbuttons[i].className.replace(' active', '');
    }
    
    // Show current tab
    var targetTab = document.getElementById(tabName);
    if (targetTab) {
        targetTab.style.display = 'block';
        
        // Find and activate the corresponding tab button
        var targetButton = document.querySelector('[onclick*=""' + tabName + '""]');
        if (targetButton) {
            targetButton.className += ' active';
        }
    }
    
    return false;
}

// Hierarchical table expand/collapse
document.addEventListener('DOMContentLoaded', function() {
    // Handle expandable parent rows (Entities, App Modules, etc.)
    var expandableRows = document.querySelectorAll('.expandable-row');
    
    expandableRows.forEach(function(row) {
        row.addEventListener('click', function(e) {
            // Don't trigger if clicking on a link
            if (e.target.tagName === 'A') return;
            
            var parentId = this.getAttribute('data-parent');
            var childRows = document.querySelectorAll('[data-child-of=""' + parentId + '""]');
            var isExpanded = this.classList.contains('expanded');
            
            if (isExpanded) {
                // Collapse - also collapse all grandchildren
                this.classList.remove('expanded');
                childRows.forEach(function(childRow) {
                    childRow.style.display = 'none';
                    childRow.classList.remove('expanded');
                    
                    // Hide grandchildren too
                    var instanceId = childRow.getAttribute('data-instance');
                    if (instanceId) {
                        var grandchildRows = document.querySelectorAll('[data-grandchild-of=""' + instanceId + '""]');
                        grandchildRows.forEach(function(grandchildRow) {
                            grandchildRow.style.display = 'none';
                        });
                    }
                });
            } else {
                // Expand
                this.classList.add('expanded');
                childRows.forEach(function(childRow) {
                    childRow.style.display = 'table-row';
                });
            }
        });
    });
    
    // Handle expandable instance rows (individual entities like 'Account')
    var expandableInstanceRows = document.querySelectorAll('.expandable-instance-row');
    
    expandableInstanceRows.forEach(function(row) {
        row.addEventListener('click', function(e) {
            // Don't trigger if clicking on a link
            if (e.target.tagName === 'A') return;
            
            var instanceId = this.getAttribute('data-instance');
            var grandchildRows = document.querySelectorAll('[data-grandchild-of=""' + instanceId + '""]');
            var isExpanded = this.classList.contains('expanded');
            
            if (isExpanded) {
                // Collapse
                this.classList.remove('expanded');
                grandchildRows.forEach(function(grandchildRow) {
                    grandchildRow.style.display = 'none';
                });
            } else {
                // Expand
                this.classList.add('expanded');
                grandchildRows.forEach(function(grandchildRow) {
                    grandchildRow.style.display = 'table-row';
                });
            }
        });
    });
    
    // Tree view toggle
    var togglers = document.getElementsByClassName('tree-toggle');
    
    for (var i = 0; i < togglers.length; i++) {
        togglers[i].addEventListener('click', function() {
            this.classList.toggle('expanded');
            var nested = this.parentElement.querySelector('.nested');
            if (nested) {
                nested.classList.toggle('active');
            }
        });
    }
    
    // Auto-expand first level of tree
    var firstLevelTogglers = document.querySelectorAll('.tree > li > .tree-toggle');
    firstLevelTogglers.forEach(function(toggler) {
        toggler.click();
    });
});

// Property table filtering
function filterPropertyTable(input) {
    var filter = input.value.toLowerCase();
    var table = input.closest('.nested').querySelector('.property-table');
    var rows = table.querySelectorAll('tbody tr');
    
    rows.forEach(function(row) {
        var propertyName = row.cells[0].textContent.toLowerCase();
        var value1 = row.cells[1].textContent.toLowerCase();
        var value2 = row.cells[2].textContent.toLowerCase();
        
        if (propertyName.indexOf(filter) > -1 || 
            value1.indexOf(filter) > -1 || 
            value2.indexOf(filter) > -1) {
            if (!row.classList.contains('hidden-by-change-filter')) {
                row.classList.remove('hidden');
            }
        } else {
            row.classList.add('hidden');
        }
    });
}

function filterChangedOnly(checkbox) {
    var table = checkbox.closest('.nested').querySelector('.property-table');
    var rows = table.querySelectorAll('tbody tr');
    
    rows.forEach(function(row) {
        if (checkbox.checked) {
            if (row.classList.contains('unchanged-row')) {
                row.classList.add('hidden');
                row.classList.add('hidden-by-change-filter');
            }
        } else {
            row.classList.remove('hidden-by-change-filter');
            // Re-apply text filter if exists
            var filterInput = checkbox.closest('.nested').querySelector('.property-filter');
            if (filterInput && filterInput.value) {
                // Trigger the text filter again
                filterPropertyTable(filterInput);
            } else {
                row.classList.remove('hidden');
            }
        }
    });
}

// Search functionality (placeholder for future enhancement)
function filterTree(searchText) {
    // TODO: Implement search/filter logic
}
";
        
        File.WriteAllText(Path.Combine(directory, "script.js"), js);
    }
}
