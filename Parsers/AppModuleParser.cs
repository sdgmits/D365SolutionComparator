using System.Xml.Linq;
using D365SolutionComparator.Models;

namespace D365SolutionComparator.Parsers;

public class AppModuleParser
{
    public List<AppModuleDefinition> ParseAppModules(XDocument customizationsXml)
    {
        var appModules = new List<AppModuleDefinition>();
        
        var appModuleNodes = customizationsXml.Descendants("AppModule");
        
        foreach (var appNode in appModuleNodes)
        {
            try
            {
                var appModule = ParseAppModule(appNode);
                appModules.Add(appModule);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Warning: Failed to parse app module: {ex.Message}");
            }
        }
        
        return appModules;
    }
    
    private AppModuleDefinition ParseAppModule(XElement appNode)
    {
        var appModule = new AppModuleDefinition
        {
            UniqueName = appNode.Element("UniqueName")?.Value ?? "Unknown",
            DisplayName = appNode.Element("LocalizedNames")
                ?.Elements("LocalizedName")
                ?.FirstOrDefault()?
                .Attribute("description")?.Value,
            Description = appNode.Element("Descriptions")
                ?.Elements("Description")
                ?.FirstOrDefault()?
                .Attribute("description")?.Value,
            FormFactor = appNode.Element("FormFactor")?.Value,
            OriginalXml = appNode
        };
        
        // Parse components
        var componentNodes = appNode.Element("AppModuleComponents")?.Elements("AppModuleComponent");
        if (componentNodes != null)
        {
            var entities = new List<string>();
            var dashboards = new List<string>();
            var businessProcessFlows = new List<string>();
            
            foreach (var compNode in componentNodes)
            {
                var componentType = compNode.Attribute("type")?.Value;
                var objectId = compNode.Attribute("objectId")?.Value;
                
                if (string.IsNullOrEmpty(objectId)) continue;
                
                switch (componentType)
                {
                    case "1": // Entity
                        entities.Add(objectId);
                        break;
                    case "60": // Dashboard
                        dashboards.Add(objectId);
                        break;
                    case "29": // Business Process Flow
                        businessProcessFlows.Add(objectId);
                        break;
                }
            }
            
            if (entities.Any()) appModule.Entities = entities;
            if (dashboards.Any()) appModule.Dashboards = dashboards;
            if (businessProcessFlows.Any()) appModule.BusinessProcessFlows = businessProcessFlows;
        }
        
        return appModule;
    }
}
