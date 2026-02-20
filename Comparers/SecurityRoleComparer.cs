using D365SolutionComparator.Models;

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
        
        // Compare privileges (simplified)
        var sourcePrivStr = source.Privileges != null ? $"{source.Privileges.Count} privileges" : "0 privileges";
        var targetPrivStr = target.Privileges != null ? $"{target.Privileges.Count} privileges" : "0 privileges";
        CompareProperty(result, "Privileges", sourcePrivStr, targetPrivStr);
        
        result.ChangeType = result.PropertyChanges.Any(p => p.Value.IsDifferent)
            ? ChangeType.Modified
            : ChangeType.Unchanged;
        
        return result;
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
