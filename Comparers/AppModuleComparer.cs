using D365SolutionComparator.Models;

namespace D365SolutionComparator.Comparers;

public class AppModuleComparer : IComparer<AppModuleDefinition>
{
    public ComparisonResult Compare(AppModuleDefinition? source, AppModuleDefinition? target)
    {
        var result = new ComparisonResult
        {
            ComponentType = "AppModule",
            ComponentName = source?.UniqueName ?? target?.UniqueName ?? "Unknown"
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
        
        CompareProperty(result, "DisplayName", source.DisplayName, target.DisplayName);
        CompareProperty(result, "Description", source.Description, target.Description);
        CompareProperty(result, "FormFactor", source.FormFactor, target.FormFactor);
        
        // Compare lists
        CompareListProperty(result, "Entities", source.Entities, target.Entities);
        CompareListProperty(result, "Dashboards", source.Dashboards, target.Dashboards);
        CompareListProperty(result, "BusinessProcessFlows", source.BusinessProcessFlows, target.BusinessProcessFlows);
        
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
    
    private void CompareListProperty(ComparisonResult result, string propertyName, List<string>? sourceList, List<string>? targetList)
    {
        var sourceStr = sourceList != null && sourceList.Any() ? string.Join(", ", sourceList) : null;
        var targetStr = targetList != null && targetList.Any() ? string.Join(", ", targetList) : null;
        
        bool isDifferent = sourceStr != targetStr;
        
        result.PropertyChanges[propertyName] = new PropertyChange
        {
            PropertyName = propertyName,
            SourceValue = sourceStr,
            TargetValue = targetStr,
            IsDifferent = isDifferent
        };
    }
}
