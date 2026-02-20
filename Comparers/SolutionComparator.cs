using D365SolutionComparator.Models;

namespace D365SolutionComparator.Comparers;

public class SolutionComparator
{
    public ComparisonResult Compare(SolutionInfo source, SolutionInfo target)
    {
        var result = new ComparisonResult
        {
            ComponentType = "Solution",
            ComponentName = $"{source.SolutionName} vs {target.SolutionName}",
            ChangeType = ChangeType.Modified,
            Source1Path = source.SourcePath,
            Source2Path = target.SourcePath,
            ComparisonDate = DateTime.Now
        };
        
        Console.WriteLine();
        Console.WriteLine("🔍 Comparing solutions...");
        
        // Compare entities
        Console.Write("  Comparing entities... ");
        var entityComparer = new EntityComparer();
        var entityComparisons = CompareCollections(
            source.Entities, 
            target.Entities,
            e => e.LogicalName,
            entityComparer
        );
        result.ChildComparisons.AddRange(entityComparisons);
        Console.WriteLine($"Done ({entityComparisons.Count} entities)");
        
        // Compare web resources
        Console.Write("  Comparing web resources... ");
        var webResourceComparer = new WebResourceComparer();
        var webResourceComparisons = CompareCollections(
            source.WebResources,
            target.WebResources,
            wr => wr.Name,
            webResourceComparer
        );
        result.ChildComparisons.AddRange(webResourceComparisons);
        Console.WriteLine($"Done ({webResourceComparisons.Count} web resources)");
        
        // Compare processes
        Console.Write("  Comparing processes... ");
        var processComparer = new ProcessComparer();
        var processComparisons = CompareCollections(
            source.Processes,
            target.Processes,
            p => p.Name,
            processComparer
        );
        result.ChildComparisons.AddRange(processComparisons);
        Console.WriteLine($"Done ({processComparisons.Count} processes)");
        
        // Compare global option sets
        Console.Write("  Comparing global option sets... ");
        var optionSetComparer = new OptionSetComparer();
        var optionSetComparisons = CompareCollections(
            source.GlobalOptionSets,
            target.GlobalOptionSets,
            os => os.Name,
            optionSetComparer
        );
        result.ChildComparisons.AddRange(optionSetComparisons);
        Console.WriteLine($"Done ({optionSetComparisons.Count} option sets)");
        
        // Compare security roles
        Console.Write("  Comparing security roles... ");
        var securityRoleComparer = new SecurityRoleComparer();
        var securityRoleComparisons = CompareCollections(
            source.SecurityRoles,
            target.SecurityRoles,
            sr => sr.Name,
            securityRoleComparer
        );
        result.ChildComparisons.AddRange(securityRoleComparisons);
        Console.WriteLine($"Done ({securityRoleComparisons.Count} security roles)");
        
        // Compare app modules
        Console.Write("  Comparing app modules... ");
        var appModuleComparer = new AppModuleComparer();
        var appModuleComparisons = CompareCollections(
            source.AppModules,
            target.AppModules,
            am => am.UniqueName,
            appModuleComparer
        );
        result.ChildComparisons.AddRange(appModuleComparisons);
        Console.WriteLine($"Done ({appModuleComparisons.Count} app modules)");
        
        return result;
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
