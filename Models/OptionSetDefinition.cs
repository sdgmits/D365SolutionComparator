using System.Xml.Linq;

namespace D365SolutionComparator.Models;

public class OptionSetDefinition
{
    public string Name { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public bool? IsGlobal { get; set; }
    public bool? IsCustomizable { get; set; }
    public Dictionary<int, string>? Options { get; set; }
    public int? DefaultValue { get; set; }
    
    public XElement? OriginalXml { get; set; }
}
