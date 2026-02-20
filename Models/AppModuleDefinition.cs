using System.Xml.Linq;

namespace D365SolutionComparator.Models;

public class AppModuleDefinition
{
    public string UniqueName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public List<string>? Entities { get; set; }
    public List<string>? Dashboards { get; set; }
    public List<string>? BusinessProcessFlows { get; set; }
    public string? FormFactor { get; set; }
    
    public XElement? OriginalXml { get; set; }
}
