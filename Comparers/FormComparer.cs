using D365SolutionComparator.Models;

namespace D365SolutionComparator.Comparers;

public class FormComparer : IComparer<FormDefinition>
{
    public ComparisonResult Compare(FormDefinition? source, FormDefinition? target)
    {
        var result = new ComparisonResult
        {
            ComponentType = "Form",
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
        
        CompareProperty(result, "FormType", source.FormType, target.FormType);
        CompareProperty(result, "IsDefault", source.IsDefault, target.IsDefault);
        CompareProperty(result, "IsDesktop", source.IsDesktop, target.IsDesktop);
        CompareProperty(result, "IsTablet", source.IsTablet, target.IsTablet);
        CompareProperty(result, "IsPhone", source.IsPhone, target.IsPhone);
        
        // Compare FormXml (simplified - could be enhanced with deep XML comparison)
        var sourceXmlHash = GetStringHash(source.FormXml);
        var targetXmlHash = GetStringHash(target.FormXml);
        CompareProperty(result, "FormXml", sourceXmlHash, targetXmlHash);
        
        // Compare JavaScript libraries
        CompareListProperty(result, "JavaScriptLibraries", source.JavaScriptLibraries, target.JavaScriptLibraries);
        
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
    
    private string GetStringHash(string? input)
    {
        if (string.IsNullOrEmpty(input)) return "null";
        return input.Length > 50 ? $"[{input.Length} chars]" : input;
    }
}
