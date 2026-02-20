using D365SolutionComparator.Models;

namespace D365SolutionComparator.Comparers;

public class RelationshipComparer : IComparer<RelationshipDefinition>
{
    public ComparisonResult Compare(RelationshipDefinition? source, RelationshipDefinition? target)
    {
        var result = new ComparisonResult
        {
            ComponentType = "Relationship",
            ComponentName = source?.SchemaName ?? target?.SchemaName ?? "Unknown"
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
        
        CompareProperty(result, "RelationshipType", source.RelationshipType, target.RelationshipType);
        CompareProperty(result, "ReferencingEntity", source.ReferencingEntity, target.ReferencingEntity);
        CompareProperty(result, "ReferencedEntity", source.ReferencedEntity, target.ReferencedEntity);
        CompareProperty(result, "ReferencingAttribute", source.ReferencingAttribute, target.ReferencingAttribute);
        CompareProperty(result, "ReferencedAttribute", source.ReferencedAttribute, target.ReferencedAttribute);
        CompareProperty(result, "CascadeAssign", source.CascadeAssign, target.CascadeAssign);
        CompareProperty(result, "CascadeDelete", source.CascadeDelete, target.CascadeDelete);
        CompareProperty(result, "CascadeMerge", source.CascadeMerge, target.CascadeMerge);
        CompareProperty(result, "CascadeReparent", source.CascadeReparent, target.CascadeReparent);
        CompareProperty(result, "CascadeShare", source.CascadeShare, target.CascadeShare);
        CompareProperty(result, "CascadeUnshare", source.CascadeUnshare, target.CascadeUnshare);
        CompareProperty(result, "IsCustomizable", source.IsCustomizable, target.IsCustomizable);
        CompareProperty(result, "IsValidForAdvancedFind", source.IsValidForAdvancedFind, target.IsValidForAdvancedFind);
        
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
