using System.Xml.Linq;

namespace D365SolutionComparator.Models;

public class SecurityRoleDefinition
{
    public string RoleId { get; set; } = string.Empty;
    public string? Name { get; set; }
    public Dictionary<string, Dictionary<string, string>>? Privileges { get; set; }
    
    public XElement? OriginalXml { get; set; }
}
