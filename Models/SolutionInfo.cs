using System.Xml.Linq;

namespace D365SolutionComparator.Models;

public class SolutionInfo
{
    public string SourcePath { get; set; } = string.Empty;
    public string SolutionName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public List<EntityDefinition> Entities { get; set; } = new();
    public List<WebResourceDefinition> WebResources { get; set; } = new();
    public List<ProcessDefinition> Processes { get; set; } = new();
    public List<OptionSetDefinition> GlobalOptionSets { get; set; } = new();
    public List<SecurityRoleDefinition> SecurityRoles { get; set; } = new();
    public List<AppModuleDefinition> AppModules { get; set; } = new();
    public XDocument? OriginalXml { get; set; }
}
