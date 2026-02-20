using System.Xml.Linq;

namespace D365SolutionComparator.Models;

public class FormDefinition
{
    public string FormId { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? FormType { get; set; }
    public bool? IsDefault { get; set; }
    public bool? IsDesktop { get; set; }
    public bool? IsTablet { get; set; }
    public bool? IsPhone { get; set; }
    public string? FormXml { get; set; }
    public List<string>? JavaScriptLibraries { get; set; }
    
    public XElement? OriginalXml { get; set; }
}
