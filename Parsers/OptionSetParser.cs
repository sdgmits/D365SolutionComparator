using System.Xml.Linq;
using D365SolutionComparator.Models;

namespace D365SolutionComparator.Parsers;

public class OptionSetParser
{
    public List<OptionSetDefinition> ParseGlobalOptionSets(XDocument customizationsXml)
    {
        var optionSets = new List<OptionSetDefinition>();
        
        var optionSetNodes = customizationsXml.Descendants("optionset")
            .Where(os => os.Attribute("isGlobal")?.Value == "1" || os.Attribute("isGlobal")?.Value?.ToLower() == "true");
        
        foreach (var osNode in optionSetNodes)
        {
            try
            {
                var optionSet = ParseOptionSet(osNode);
                optionSets.Add(optionSet);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Warning: Failed to parse option set: {ex.Message}");
            }
        }
        
        return optionSets;
    }
    
    private OptionSetDefinition ParseOptionSet(XElement osNode)
    {
        var optionSet = new OptionSetDefinition
        {
            Name = osNode.Element("Name")?.Value ?? "Unknown",
            DisplayName = osNode.Element("displaynames")
                ?.Elements("displayname")
                ?.FirstOrDefault()?
                .Attribute("description")?.Value,
            Description = osNode.Element("Descriptions")
                ?.Elements("Description")
                ?.FirstOrDefault()?
                .Attribute("description")?.Value,
            IsGlobal = ParseBool(osNode.Attribute("isGlobal")?.Value),
            IsCustomizable = ParseBool(osNode.Element("IsCustomizable")?.Value),
            OriginalXml = osNode
        };
        
        // Parse options
        var options = new Dictionary<int, string>();
        var optionNodes = osNode.Element("options")?.Elements("option");
        
        if (optionNodes != null)
        {
            foreach (var optionNode in optionNodes)
            {
                var valueStr = optionNode.Attribute("value")?.Value;
                var label = optionNode.Element("labels")
                    ?.Elements("label")
                    ?.FirstOrDefault()?
                    .Attribute("description")?.Value;
                
                if (int.TryParse(valueStr, out int value) && !string.IsNullOrEmpty(label))
                {
                    options[value] = label;
                }
            }
        }
        
        if (options.Any())
        {
            optionSet.Options = options;
        }
        
        // Parse default value
        var defaultValueStr = osNode.Element("DefaultValue")?.Value;
        if (int.TryParse(defaultValueStr, out int defaultValue))
        {
            optionSet.DefaultValue = defaultValue;
        }
        
        return optionSet;
    }
    
    private bool? ParseBool(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;
        
        if (value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase))
            return true;
        
        if (value == "0" || value.Equals("false", StringComparison.OrdinalIgnoreCase))
            return false;
        
        return null;
    }
}
