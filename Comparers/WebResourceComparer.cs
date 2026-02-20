using D365SolutionComparator.Models;

namespace D365SolutionComparator.Comparers;

public class WebResourceComparer : IComparer<WebResourceDefinition>
{
    public ComparisonResult Compare(WebResourceDefinition? source, WebResourceDefinition? target)
    {
        var result = new ComparisonResult
        {
            ComponentType = "WebResource",
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
        CompareProperty(result, "WebResourceType", source.WebResourceType, target.WebResourceType);
        CompareProperty(result, "ContentHash", source.ContentHash, target.ContentHash);
        CompareProperty(result, "IsCustomizable", source.IsCustomizable, target.IsCustomizable);
        CompareProperty(result, "IsEnabledForMobileClient", source.IsEnabledForMobileClient, target.IsEnabledForMobileClient);
        CompareProperty(result, "LanguageCode", source.LanguageCode, target.LanguageCode);
        
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
