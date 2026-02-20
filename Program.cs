using System.CommandLine;
using D365SolutionComparator.Parsers;
using D365SolutionComparator.Comparers;
using D365SolutionComparator.ReportGenerators;
using D365SolutionComparator.Models;

namespace D365SolutionComparator;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("D365 Solution Comparator - Compare Dynamics 365 CE solution exports");
        
        var source1Option = new Option<string>(
            aliases: new[] { "--source1", "-s1" },
            description: "Path to first solution (ZIP file or extracted folder)");
        source1Option.IsRequired = true;
        
        var source2Option = new Option<string>(
            aliases: new[] { "--source2", "-s2" },
            description: "Path to second solution (ZIP file or extracted folder)");
        source2Option.IsRequired = true;
        
        var outputOption = new Option<string>(
            aliases: new[] { "--output", "-o" },
            description: "Output path for the HTML report",
            getDefaultValue: () => "comparison-report.html");
        
        var verboseOption = new Option<bool>(
            aliases: new[] { "--verbose", "-v" },
            description: "Enable verbose logging");
        
        rootCommand.AddOption(source1Option);
        rootCommand.AddOption(source2Option);
        rootCommand.AddOption(outputOption);
        rootCommand.AddOption(verboseOption);
        
        rootCommand.SetHandler(async (string source1, string source2, string output, bool verbose) =>
        {
            await CompareSolutions(source1, source2, output, verbose);
        }, source1Option, source2Option, outputOption, verboseOption);
        
        return await rootCommand.InvokeAsync(args);
    }
    
    static async Task CompareSolutions(string source1Path, string source2Path, string outputPath, bool verbose)
    {
        string? solution1Dir = null;
        string? solution2Dir = null;
        
        try
        {
            Console.WriteLine();
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║      🎯 D365 Solution Comparator                         ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            
            // Validate input paths
            if (!File.Exists(source1Path) && !Directory.Exists(source1Path))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Error: Source 1 path does not exist: {source1Path}");
                Console.ResetColor();
                return;
            }
            
            if (!File.Exists(source2Path) && !Directory.Exists(source2Path))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Error: Source 2 path does not exist: {source2Path}");
                Console.ResetColor();
                return;
            }
            
            // 1. Extract solutions
            Console.WriteLine("📦 Step 1: Extracting solutions...");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            var extractor = new SolutionExtractor();
            solution1Dir = extractor.ExtractSolution(source1Path);
            solution2Dir = extractor.ExtractSolution(source2Path);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ Solutions extracted");
            Console.ResetColor();
            Console.WriteLine();
            
            // 2. Parse customizations.xml
            Console.WriteLine("📄 Step 2: Parsing customizations.xml files...");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            var parser = new CustomizationsXmlParser();
            
            Console.WriteLine("  Solution 1:");
            string customizations1 = extractor.GetCustomizationsXmlPath(solution1Dir);
            var xml1 = parser.LoadCustomizationsXml(customizations1);
            var solution1Data = parser.ParseSolution(xml1, source1Path);
            
            Console.WriteLine();
            Console.WriteLine("  Solution 2:");
            string customizations2 = extractor.GetCustomizationsXmlPath(solution2Dir);
            var xml2 = parser.LoadCustomizationsXml(customizations2);
            var solution2Data = parser.ParseSolution(xml2, source2Path);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("✅ Parsing complete");
            Console.ResetColor();
            Console.WriteLine();
            
            // 3. Compare solutions
            Console.WriteLine("🔍 Step 3: Comparing solutions...");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            var comparator = new SolutionComparator();
            var comparisonResult = comparator.Compare(solution1Data, solution2Data);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ Comparison complete");
            Console.ResetColor();
            Console.WriteLine();
            
            // 4. Generate report
            Console.WriteLine("📊 Step 4: Generating HTML report...");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            var reportGenerator = new HtmlReportGenerator();
            reportGenerator.GenerateReport(comparisonResult, outputPath);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ Report generated successfully");
            Console.ResetColor();
            Console.WriteLine();
            
            // 5. Display summary
            Console.WriteLine("📈 Step 5: Comparison Summary");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            DisplaySummary(comparisonResult);
            
            Console.WriteLine();
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("║  ✅ Comparison completed successfully!                    ║");
            Console.ResetColor();
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"📄 Open the report in your browser: {Path.GetFullPath(outputPath)}");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  ❌ Error occurred during comparison                      ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"Error: {ex.Message}");
            
            if (verbose)
            {
                Console.WriteLine();
                Console.WriteLine("Stack Trace:");
                Console.WriteLine(ex.StackTrace);
            }
            Console.ResetColor();
            Console.WriteLine();
        }
        finally
        {
            // Cleanup temp directories
            if (solution1Dir != null)
            {
                var extractor = new SolutionExtractor();
                extractor.Cleanup(solution1Dir, source1Path);
            }
            
            if (solution2Dir != null)
            {
                var extractor = new SolutionExtractor();
                extractor.Cleanup(solution2Dir, source2Path);
            }
        }
        
        await Task.CompletedTask;
    }
    
    static void DisplaySummary(ComparisonResult result)
    {
        var stats = result.GetStatistics();
        
        if (!stats.Any())
        {
            Console.WriteLine("  No changes detected.");
            return;
        }
        
        // Create summary table
        Console.WriteLine();
        Console.WriteLine("  ┌─────────────────────────┬────────┬─────────┬──────────┬───────┐");
        Console.WriteLine("  │ Component Type          │ Added  │ Removed │ Modified │ Total │");
        Console.WriteLine("  ├─────────────────────────┼────────┼─────────┼──────────┼───────┤");
        
        foreach (var stat in stats)
        {
            var componentType = stat.ComponentType.PadRight(23);
            var added = stat.Added.ToString().PadLeft(6);
            var removed = stat.Removed.ToString().PadLeft(7);
            var modified = stat.Modified.ToString().PadLeft(8);
            var total = stat.Total.ToString().PadLeft(5);
            
            Console.Write("  │ " + componentType + " │");
            
            // Color code the counts
            if (stat.Added > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(added);
                Console.ResetColor();
            }
            else
            {
                Console.Write(added);
            }
            Console.Write(" │");
            
            if (stat.Removed > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(removed);
                Console.ResetColor();
            }
            else
            {
                Console.Write(removed);
            }
            Console.Write(" │");
            
            if (stat.Modified > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(modified);
                Console.ResetColor();
            }
            else
            {
                Console.Write(modified);
            }
            Console.Write(" │");
            
            Console.WriteLine(total + " │");
        }
        
        Console.WriteLine("  └─────────────────────────┴────────┴─────────┴──────────┴───────┘");
        Console.WriteLine();
        
        // Highlight critical changes
        var criticalCount = 0;
        foreach (var entity in result.EntityComparisons)
        {
            if (entity.ChangeType == ChangeType.Added || entity.ChangeType == ChangeType.Removed)
            {
                criticalCount++;
            }
            else if (entity.ChangeType == ChangeType.Modified)
            {
                var attributeChanges = entity.ChildComparisons.Count(c => c.ComponentType == "Attribute" && c.ChangeType != ChangeType.Unchanged);
                if (attributeChanges > 5)
                {
                    criticalCount++;
                }
            }
        }
        
        if (criticalCount > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  ⚠️  {criticalCount} critical change(s) detected. Review the HTML report for details.");
            Console.ResetColor();
        }
    }
}
