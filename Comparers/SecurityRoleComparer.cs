using D365SolutionComparator.Models;
using Newtonsoft.Json;

namespace D365SolutionComparator.Comparers;

public class SecurityRoleComparer : IComparer<SecurityRoleDefinition>
{
    public ComparisonResult Compare(SecurityRoleDefinition? source, SecurityRoleDefinition? target)
    {
        var result = new ComparisonResult
        {
            ComponentType = "SecurityRole",
            ComponentName = source?.Name ?? target?.Name ?? "Unknown"
        };
        
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
        
        if (source == null || target == null)
        {
            result.ChangeType = ChangeType.Unchanged;
            return result;
        }
        
        CompareProperty(result, "RoleId", source.RoleId, target.RoleId);
        
        // Compare privileges in detail
        ComparePrivileges(result, source.Privileges, target.Privileges);
        
        result.ChangeType = result.PropertyChanges.Any(p => p.Value.IsDifferent)
            ? ChangeType.Modified
            : ChangeType.Unchanged;
        
        return result;
    }
    
    private void ComparePrivileges(ComparisonResult result, 
        Dictionary<string, Dictionary<string, string>>? sourcePrivileges, 
        Dictionary<string, Dictionary<string, string>>? targetPrivileges)
    {
        var privilegeChanges = new Dictionary<string, (string sourceLevel, string targetLevel)>();
        
        // Get all unique privilege names
        var allPrivileges = new HashSet<string>();
        if (sourcePrivileges != null) allPrivileges.UnionWith(sourcePrivileges.Keys);
        if (targetPrivileges != null) allPrivileges.UnionWith(targetPrivileges.Keys);
        
        foreach (var privilegeName in allPrivileges.OrderBy(p => p))
        {
            var sourceLevel = sourcePrivileges?.ContainsKey(privilegeName) == true && sourcePrivileges[privilegeName].ContainsKey("Level")
                ? sourcePrivileges[privilegeName]["Level"]
                : "None";
            var targetLevel = targetPrivileges?.ContainsKey(privilegeName) == true && targetPrivileges[privilegeName].ContainsKey("Level")
                ? targetPrivileges[privilegeName]["Level"]
                : "None";
            
            if (sourceLevel != targetLevel)
            {
                privilegeChanges[privilegeName] = (sourceLevel, targetLevel);
            }
        }
        
        // Store privilege comparison details for UI display (only if there are changes)
        if (privilegeChanges.Any())
        {
            var privilegeComparisonJson = JsonConvert.SerializeObject(privilegeChanges);
            CompareProperty(result, "PrivilegeDetails", privilegeComparisonJson, $"{privilegeChanges.Count}");
            result.PropertyChanges["PrivilegeDetails"].IsDifferent = true;
        }
        
        // Summary for table display - only mark as different if counts actually differ or privileges changed
        var sourceCount = sourcePrivileges?.Count ?? 0;
        var targetCount = targetPrivileges?.Count ?? 0;
        var changedCount = privilegeChanges.Count;
        
        // Only add this property if there's an actual difference
        if (sourceCount != targetCount || changedCount > 0)
        {
            var sourceText = $"{sourceCount} privileges";
            var targetText = changedCount > 0 
                ? $"{targetCount} privileges ({changedCount} changed)" 
                : $"{targetCount} privileges";
            
            CompareProperty(result, "TotalPrivileges", sourceText, targetText);
        }
    }
    
    private void CompareProperty<T>(ComparisonResult result, string propertyName, T? sourceValue, T? targetValue)
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
}
