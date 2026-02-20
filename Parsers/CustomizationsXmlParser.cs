using System.Xml.Linq;
using D365SolutionComparator.Models;

namespace D365SolutionComparator.Parsers;

public class CustomizationsXmlParser
{
    public XDocument LoadCustomizationsXml(string xmlPath)
    {
        return XDocument.Load(xmlPath);
    }
    
    public SolutionInfo ParseSolution(XDocument customizationsXml, string sourcePath)
    {
        var solution = new SolutionInfo
        {
            SourcePath = sourcePath,
            OriginalXml = customizationsXml
        };
        
        // Parse solution information
        var solutionManifest = customizationsXml.Descendants("SolutionManifest").FirstOrDefault();
        if (solutionManifest != null)
        {
            solution.SolutionName = solutionManifest.Element("UniqueName")?.Value ?? "Unknown";
            solution.Version = solutionManifest.Element("Version")?.Value ?? "0.0.0.0";
        }
        
        Console.WriteLine($"  Solution: {solution.SolutionName} v{solution.Version}");
        
        // Parse entities
        var entityParser = new EntityParser();
        solution.Entities = entityParser.ParseEntities(customizationsXml);
        Console.WriteLine($"  Found {solution.Entities.Count} entities");
        
        // Parse web resources
        var webResourceParser = new WebResourceParser();
        solution.WebResources = webResourceParser.ParseWebResources(customizationsXml);
        Console.WriteLine($"  Found {solution.WebResources.Count} web resources");
        
        // Parse processes
        var processParser = new ProcessParser();
        solution.Processes = processParser.ParseProcesses(customizationsXml);
        Console.WriteLine($"  Found {solution.Processes.Count} processes");
        
        // Parse global option sets
        var optionSetParser = new OptionSetParser();
        solution.GlobalOptionSets = optionSetParser.ParseGlobalOptionSets(customizationsXml);
        Console.WriteLine($"  Found {solution.GlobalOptionSets.Count} global option sets");
        
        // Parse security roles
        var securityRoleParser = new SecurityRoleParser();
        solution.SecurityRoles = securityRoleParser.ParseSecurityRoles(customizationsXml);
        Console.WriteLine($"  Found {solution.SecurityRoles.Count} security roles");
        
        // Parse app modules
        var appModuleParser = new AppModuleParser();
        solution.AppModules = appModuleParser.ParseAppModules(customizationsXml);
        Console.WriteLine($"  Found {solution.AppModules.Count} app modules");
        
        return solution;
    }
}
