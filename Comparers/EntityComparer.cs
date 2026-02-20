using D365SolutionComparator.Models;

namespace D365SolutionComparator.Comparers;

public class EntityComparer : IComparer<EntityDefinition>
{
    public ComparisonResult Compare(EntityDefinition? source, EntityDefinition? target)
    {
        var result = new ComparisonResult
        {
            ComponentType = "Entity",
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
        CompareProperty(result, "DisplayName", source.DisplayName, target.DisplayName);
        CompareProperty(result, "PluralName", source.PluralName, target.PluralName);
        CompareProperty(result, "Description", source.Description, target.Description);
        CompareProperty(result, "OwnershipType", source.OwnershipType, target.OwnershipType);
        CompareProperty(result, "IsActivity", source.IsActivity, target.IsActivity);
        CompareProperty(result, "IsCustomEntity", source.IsCustomEntity, target.IsCustomEntity);
        CompareProperty(result, "IsAuditEnabled", source.IsAuditEnabled, target.IsAuditEnabled);
        CompareProperty(result, "IsValidForQueue", source.IsValidForQueue, target.IsValidForQueue);
        CompareProperty(result, "IsConnectionsEnabled", source.IsConnectionsEnabled, target.IsConnectionsEnabled);
        CompareProperty(result, "IsDuplicateDetectionEnabled", source.IsDuplicateDetectionEnabled, target.IsDuplicateDetectionEnabled);
        CompareProperty(result, "IsMailMergeEnabled", source.IsMailMergeEnabled, target.IsMailMergeEnabled);
        CompareProperty(result, "PrimaryNameAttribute", source.PrimaryNameAttribute, target.PrimaryNameAttribute);
        CompareProperty(result, "PrimaryImageAttribute", source.PrimaryImageAttribute, target.PrimaryImageAttribute);
        CompareProperty(result, "EntityColor", source.EntityColor, target.EntityColor);
        CompareProperty(result, "EntityHelpUrlEnabled", source.EntityHelpUrlEnabled, target.EntityHelpUrlEnabled);
        CompareProperty(result, "AutoRouteToOwnerQueue", source.AutoRouteToOwnerQueue, target.AutoRouteToOwnerQueue);
        
        // Compare child components
        // Attributes
        var attributeComparer = new AttributeComparer();
        result.ChildComparisons.AddRange(
            CompareCollections(source.Attributes, target.Attributes, 
                a => a.LogicalName, attributeComparer)
        );
        
        // Forms
        var formComparer = new FormComparer();
        result.ChildComparisons.AddRange(
            CompareCollections(source.Forms, target.Forms,
                f => f.FormId, formComparer)
        );
        
        // Views
        var viewComparer = new ViewComparer();
        result.ChildComparisons.AddRange(
            CompareCollections(source.Views, target.Views,
                v => v.SavedQueryId, viewComparer)
        );
        
        // Relationships
        var relationshipComparer = new RelationshipComparer();
        result.ChildComparisons.AddRange(
            CompareCollections(source.Relationships, target.Relationships,
                r => r.SchemaName, relationshipComparer)
        );
        
        // Business Rules
        var businessRuleComparer = new BusinessRuleComparer();
        result.ChildComparisons.AddRange(
            CompareCollections(source.BusinessRules, target.BusinessRules,
                br => br.Name, businessRuleComparer)
        );
        
        // Determine overall change type
        result.ChangeType = (result.PropertyChanges.Any(p => p.Value.IsDifferent) 
            || result.ChildComparisons.Any(c => c.ChangeType != ChangeType.Unchanged))
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
    
    private List<ComparisonResult> CompareCollections<T>(
        List<T> sourceList,
        List<T> targetList,
        Func<T, string> keySelector,
        IComparer<T> comparer)
    {
        var results = new List<ComparisonResult>();
        
        var sourceDict = sourceList?.ToDictionary(keySelector) ?? new Dictionary<string, T>();
        var targetDict = targetList?.ToDictionary(keySelector) ?? new Dictionary<string, T>();
        
        var allKeys = sourceDict.Keys.Union(targetDict.Keys).Distinct();
        
        foreach (var key in allKeys)
        {
            sourceDict.TryGetValue(key, out T? sourceItem);
            targetDict.TryGetValue(key, out T? targetItem);
            
            var comparison = comparer.Compare(sourceItem, targetItem);
            results.Add(comparison);
        }
        
        return results;
    }
}
