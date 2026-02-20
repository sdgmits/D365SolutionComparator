using System.Xml.Linq;

namespace D365SolutionComparator.Models;

public class ProcessDefinition
{
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? ProcessType { get; set; }
    public bool? IsTransacted { get; set; }
    public string? Scope { get; set; }
    public string? Mode { get; set; }
    public string? XamlDefinition { get; set; }
    
    public XElement? OriginalXml { get; set; }
}
