using System.Xml.Linq;
using D365SolutionComparator.Models;

namespace D365SolutionComparator.Parsers;

public class AttributeParser
{
    public List<AttributeDefinition> ParseAttributes(XElement entityInfo)
    {
        var attributes = new List<AttributeDefinition>();
        
        var attributeNodes = entityInfo.Element("attributes")?.Elements("attribute");
        if (attributeNodes == null) return attributes;
        
        foreach (var attrNode in attributeNodes)
        {
            try
            {
                var attribute = ParseAttribute(attrNode);
                attributes.Add(attribute);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Warning: Failed to parse attribute: {ex.Message}");
            }
        }
        
        return attributes;
    }
    
    private AttributeDefinition ParseAttribute(XElement attrNode)
    {
        var attribute = new AttributeDefinition
        {
            LogicalName = attrNode.Element("LogicalName")?.Value ?? attrNode.Element("Name")?.Value ?? "Unknown",
            SchemaName = attrNode.Element("SchemaName")?.Value,
            AttributeType = attrNode.Element("Type")?.Value,
            OriginalXml = attrNode
        };
        
        // Parse display name
        attribute.DisplayName = attrNode.Element("displaynames")
            ?.Elements("displayname")
            ?.FirstOrDefault()?
            .Attribute("description")?.Value;
        
        // Parse description
        attribute.Description = attrNode.Element("Descriptions")
            ?.Elements("Description")
            ?.FirstOrDefault()?
            .Attribute("description")?.Value;
        
        // Parse common properties
        attribute.RequiredLevel = attrNode.Element("RequiredLevel")?.Value;
        attribute.IsAuditEnabled = ParseBool(attrNode.Element("IsAuditEnabled")?.Value);
        attribute.IsSecured = ParseBool(attrNode.Element("IsSecured")?.Value);
        attribute.IsCustomizable = ParseBool(attrNode.Element("IsCustomizable")?.Value);
        
        // Parse type-specific properties based on AttributeType
        var attrType = attribute.AttributeType?.ToLower();
        
        switch (attrType)
        {
            case "string":
            case "memo":
                attribute.MaxLength = ParseInt(attrNode.Element("MaxLength")?.Value);
                attribute.Format = attrNode.Element("Format")?.Value;
                attribute.ImeMode = attrNode.Element("ImeMode")?.Value;
                break;
            
            case "integer":
            case "decimal":
            case "money":
            case "double":
                attribute.MinValue = ParseInt(attrNode.Element("MinValue")?.Value);
                attribute.MaxValue = ParseInt(attrNode.Element("MaxValue")?.Value);
                attribute.Precision = ParseInt(attrNode.Element("Precision")?.Value);
                break;
            
            case "datetime":
                attribute.Format = attrNode.Element("Format")?.Value;
                attribute.DateTimeBehavior = attrNode.Element("DateTimeBehavior")?.Value;
                attribute.ImeMode = attrNode.Element("ImeMode")?.Value;
                break;
            
            case "lookup":
            case "customer":
            case "owner":
                var targets = attrNode.Element("Targets")?.Elements("Target")
                    .Select(t => t.Value)
                    .ToList();
                if (targets != null && targets.Any())
                {
                    attribute.Targets = targets;
                }
                break;
            
            case "picklist":
            case "state":
            case "status":
                attribute.OptionSetValues = ParseOptionSetValues(attrNode);
                break;
            
            case "boolean":
                var trueOption = attrNode.Element("TrueOption")?.Element("Label")?.Attribute("description")?.Value;
                var falseOption = attrNode.Element("FalseOption")?.Element("Label")?.Attribute("description")?.Value;
                attribute.TrueOption = trueOption;
                attribute.FalseOption = falseOption;
                break;
        }
        
        return attribute;
    }
    
    private Dictionary<int, string>? ParseOptionSetValues(XElement attrNode)
    {
        var optionSetNode = attrNode.Element("optionset");
        if (optionSetNode == null) return null;
        
        var options = new Dictionary<int, string>();
        var optionNodes = optionSetNode.Element("options")?.Elements("option");
        
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
        
        return options.Any() ? options : null;
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
    
    private int? ParseInt(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;
        
        return int.TryParse(value, out int result) ? result : null;
    }
}
