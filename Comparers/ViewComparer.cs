using D365SolutionComparator.Models;

namespace D365SolutionComparator.Comparers;

public class ViewComparer : IComparer<ViewDefinition>
{
    public ComparisonResult Compare(ViewDefinition? source, ViewDefinition? target)
    {
        var result = new ComparisonResult
        {
            ComponentType = "View",
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
        
        CompareProperty(result, "QueryType", source.QueryType, target.QueryType);
        CompareProperty(result, "IsDefault", source.IsDefault, target.IsDefault);
        CompareProperty(result, "IsUserDefined", source.IsUserDefined, target.IsUserDefined);
        CompareProperty(result, "ReturnedTypeCode", source.ReturnedTypeCode, target.ReturnedTypeCode);
        
        // Compare FetchXml
        var sourceFetchHash = GetStringHash(source.FetchXml);
        var targetFetchHash = GetStringHash(target.FetchXml);
        CompareProperty(result, "FetchXml", sourceFetchHash, targetFetchHash);
        
        // Compare LayoutXml
        var sourceLayoutHash = GetStringHash(source.LayoutXml);
        var targetLayoutHash = GetStringHash(target.LayoutXml);
        CompareProperty(result, "LayoutXml", sourceLayoutHash, targetLayoutHash);
        
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
    
    private string GetStringHash(string? input)
    {
        if (string.IsNullOrEmpty(input)) return "null";
        return input.Length > 50 ? $"[{input.Length} chars]" : input;
    }
}
