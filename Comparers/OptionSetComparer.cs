using D365SolutionComparator.Models;

namespace D365SolutionComparator.Comparers;

public class OptionSetComparer : IComparer<OptionSetDefinition>
{
    public ComparisonResult Compare(OptionSetDefinition? source, OptionSetDefinition? target)
    {
        var result = new ComparisonResult
        {
            ComponentType = "OptionSet",
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
        
        CompareProperty(result, "DisplayName", source.DisplayName, target.DisplayName);
        CompareProperty(result, "Description", source.Description, target.Description);
        CompareProperty(result, "IsGlobal", source.IsGlobal, target.IsGlobal);
        CompareProperty(result, "IsCustomizable", source.IsCustomizable, target.IsCustomizable);
        CompareProperty(result, "DefaultValue", source.DefaultValue, target.DefaultValue);
        
        // Compare options
        CompareDictionaryProperty(result, "Options", source.Options, target.Options);
        
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
    
    private void CompareDictionaryProperty(ComparisonResult result, string propertyName, 
        Dictionary<int, string>? sourceDict, Dictionary<int, string>? targetDict)
    {
        var sourceStr = sourceDict != null && sourceDict.Any() 
            ? string.Join(", ", sourceDict.Select(kvp => $"{kvp.Key}:{kvp.Value}"))
            : null;
        var targetStr = targetDict != null && targetDict.Any()
            ? string.Join(", ", targetDict.Select(kvp => $"{kvp.Key}:{kvp.Value}"))
            : null;
        
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
