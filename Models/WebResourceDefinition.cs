using System.Xml.Linq;

namespace D365SolutionComparator.Models;

public class WebResourceDefinition
{
    public string Name { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? WebResourceType { get; set; }
    public string? ContentHash { get; set; }
    public bool? IsCustomizable { get; set; }
    public bool? IsEnabledForMobileClient { get; set; }
    public string? LanguageCode { get; set; }
    
    public XElement? OriginalXml { get; set; }
}
