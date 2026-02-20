using D365SolutionComparator.Models;

namespace D365SolutionComparator.Comparers;

public class AttributeComparer : IComparer<AttributeDefinition>
{
    public ComparisonResult Compare(AttributeDefinition? source, AttributeDefinition? target)
    {
        var result = new ComparisonResult
        {
            ComponentType = "Attribute",
            ComponentName = source?.LogicalName ?? target?.LogicalName ?? "Unknown"
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
        
        if (source == null || target == null)
        {
            result.ChangeType = ChangeType.Unchanged;
            return result;
        }
        
        // Compare properties
        CompareProperty(result, "SchemaName", source.SchemaName, target.SchemaName);
        CompareProperty(result, "AttributeType", source.AttributeType, target.AttributeType);
        CompareProperty(result, "DisplayName", source.DisplayName, target.DisplayName);
        CompareProperty(result, "Description", source.Description, target.Description);
        CompareProperty(result, "RequiredLevel", source.RequiredLevel, target.RequiredLevel);
        CompareProperty(result, "IsAuditEnabled", source.IsAuditEnabled, target.IsAuditEnabled);
        CompareProperty(result, "IsSecured", source.IsSecured, target.IsSecured);
        CompareProperty(result, "IsCustomizable", source.IsCustomizable, target.IsCustomizable);
        CompareProperty(result, "MaxLength", source.MaxLength, target.MaxLength);
        CompareProperty(result, "Format", source.Format, target.Format);
        CompareProperty(result, "ImeMode", source.ImeMode, target.ImeMode);
        CompareProperty(result, "MinValue", source.MinValue, target.MinValue);
        CompareProperty(result, "MaxValue", source.MaxValue, target.MaxValue);
        CompareProperty(result, "Precision", source.Precision, target.Precision);
        CompareProperty(result, "DateTimeBehavior", source.DateTimeBehavior, target.DateTimeBehavior);
        CompareProperty(result, "TrueOption", source.TrueOption, target.TrueOption);
        CompareProperty(result, "FalseOption", source.FalseOption, target.FalseOption);
        
        // Compare lists
        CompareListProperty(result, "Targets", source.Targets, target.Targets);
        
        // Compare option set values
        CompareDictionaryProperty(result, "OptionSetValues", source.OptionSetValues, target.OptionSetValues);
        
        // Determine overall change type
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
