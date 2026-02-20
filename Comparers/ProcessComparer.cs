using D365SolutionComparator.Models;

namespace D365SolutionComparator.Comparers;

public class ProcessComparer : IComparer<ProcessDefinition>
{
    public ComparisonResult Compare(ProcessDefinition? source, ProcessDefinition? target)
    {
        var result = new ComparisonResult
        {
            ComponentType = "Process",
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
        
        CompareProperty(result, "Category", source.Category, target.Category);
        CompareProperty(result, "ProcessType", source.ProcessType, target.ProcessType);
        CompareProperty(result, "IsTransacted", source.IsTransacted, target.IsTransacted);
        CompareProperty(result, "Scope", source.Scope, target.Scope);
        CompareProperty(result, "Mode", source.Mode, target.Mode);
        
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
