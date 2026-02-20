namespace D365SolutionComparator.Models;

public class ComparisonResult
{
    public string ComponentType { get; set; } = string.Empty;
    public string? ComponentName { get; set; }
    public ChangeType ChangeType { get; set; }
    public Dictionary<string, PropertyChange> PropertyChanges { get; set; } = new();
    public List<ComparisonResult> ChildComparisons { get; set; } = new();
    
    // For solution-level comparison
    public string? Source1Path { get; set; }
    public string? Source2Path { get; set; }
    public DateTime ComparisonDate { get; set; } = DateTime.Now;
    
    public List<ComparisonResult> EntityComparisons => 
        ChildComparisons.Where(c => c.ComponentType == "Entity").ToList();
    
    public List<ComparisonResult> WebResourceComparisons => 
        ChildComparisons.Where(c => c.ComponentType == "WebResource").ToList();
    
    public List<ComparisonResult> ProcessComparisons => 
        ChildComparisons.Where(c => c.ComponentType == "Process").ToList();
    
    public List<ComponentStatistics> GetStatistics()
    {
        var stats = new List<ComponentStatistics>();
        
        // Group by component type
        var componentGroups = new Dictionary<string, ComponentStatistics>();
        
        CollectStatistics(this, componentGroups);
        
        return componentGroups.Values.OrderBy(s => s.ComponentType).ToList();
    }
    
    public List<HierarchicalStatistics> GetHierarchicalStatistics()
    {
        var hierarchicalStats = new List<HierarchicalStatistics>();
        
        // Define parent component types (top-level)
        var parentTypes = new[] { "Entity", "AppModule", "WebResource", "Process", "OptionSet", "SecurityRole" };
        
        foreach (var parentType in parentTypes)
        {
            var parentComponents = ChildComparisons.Where(c => c.ComponentType == parentType).ToList();
            if (!parentComponents.Any()) continue;
            
            var parentStat = new HierarchicalStatistics
            {
                ComponentType = parentType,
                ComponentName = null // Top-level summary
            };
            
            foreach (var parent in parentComponents)
            {
                parentStat.Total++;
                
                switch (parent.ChangeType)
                {
                    case ChangeType.Added:
                        parentStat.Added++;
                        break;
                    case ChangeType.Removed:
                        parentStat.Removed++;
                        break;
                    case ChangeType.Modified:
                        parentStat.Modified++;
                        break;
                }
                
                // Create component instance for this entity/component
                var instance = new ComponentInstance
                {
                    Name = parent.ComponentName ?? "Unknown",
                    ChangeType = parent.ChangeType
                };
                
                // Collect child statistics for this instance
                CollectChildStatistics(parent, instance.ChildStatistics);
                
                // Only add instances that have changes (added, removed, or have child changes)
                if (parent.ChangeType != ChangeType.Unchanged || instance.HasChildChanges)
                {
                    parentStat.ComponentInstances.Add(instance);
                }
                
                // Also collect in the parent's aggregated child statistics
                CollectChildStatistics(parent, parentStat.ChildStatistics);
            }
            
            hierarchicalStats.Add(parentStat);
        }
        
        return hierarchicalStats;
    }
    
    private void CollectChildStatistics(ComparisonResult parent, Dictionary<string, ComponentStatistics> childStats)
    {
        foreach (var child in parent.ChildComparisons)
        {
            if (!childStats.ContainsKey(child.ComponentType))
            {
                childStats[child.ComponentType] = new ComponentStatistics
                {
                    ComponentType = child.ComponentType
                };
            }
            
            var stat = childStats[child.ComponentType];
            stat.Total++;
            
            switch (child.ChangeType)
            {
                case ChangeType.Added:
                    stat.Added++;
                    break;
                case ChangeType.Removed:
                    stat.Removed++;
                    break;
                case ChangeType.Modified:
                    stat.Modified++;
                    break;
            }
        }
    }
    
    private void CollectStatistics(ComparisonResult result, Dictionary<string, ComponentStatistics> groups)
    {
        foreach (var child in result.ChildComparisons)
        {
            if (!groups.ContainsKey(child.ComponentType))
            {
                groups[child.ComponentType] = new ComponentStatistics
                {
                    ComponentType = child.ComponentType
                };
            }
            
            var stat = groups[child.ComponentType];
            stat.Total++;
            
            switch (child.ChangeType)
            {
                case ChangeType.Added:
                    stat.Added++;
                    break;
                case ChangeType.Removed:
                    stat.Removed++;
                    break;
                case ChangeType.Modified:
                    stat.Modified++;
                    break;
            }
            
            // Recursively collect from children
            CollectStatistics(child, groups);
        }
    }
}

public class ComponentStatistics
{
    public string ComponentType { get; set; } = string.Empty;
    public int Added { get; set; }
    public int Removed { get; set; }
    public int Modified { get; set; }
    public int Total { get; set; }
}

public class HierarchicalStatistics
{
    public string ComponentType { get; set; } = string.Empty;
    public string? ComponentName { get; set; }
    public int Added { get; set; }
    public int Removed { get; set; }
    public int Modified { get; set; }
    public int Total { get; set; }
    public Dictionary<string, ComponentStatistics> ChildStatistics { get; set; } = new();
    public List<ComponentInstance> ComponentInstances { get; set; } = new();
    
    public bool HasChanges => Added > 0 || Removed > 0 || Modified > 0;
    public bool HasChildChanges => ChildStatistics.Values.Any(c => c.Added > 0 || c.Removed > 0 || c.Modified > 0);
}

public class ComponentInstance
{
    public string Name { get; set; } = string.Empty;
    public ChangeType ChangeType { get; set; }
    public Dictionary<string, ComponentStatistics> ChildStatistics { get; set; } = new();
    
    public bool HasChildChanges => ChildStatistics.Values.Any(c => c.Added > 0 || c.Removed > 0 || c.Modified > 0);
}
