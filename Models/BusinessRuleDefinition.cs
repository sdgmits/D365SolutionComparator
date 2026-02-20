using System.Xml.Linq;

namespace D365SolutionComparator.Models;

public class BusinessRuleDefinition
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Scope { get; set; }
    public string? ConditionsXml { get; set; }
    public string? ActionsXml { get; set; }
    
    public XElement? OriginalXml { get; set; }
}
