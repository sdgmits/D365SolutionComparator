using System.Xml.Linq;

namespace D365SolutionComparator.Models;

public class RelationshipDefinition
{
    public string SchemaName { get; set; } = string.Empty;
    public string? RelationshipType { get; set; }
    public string? ReferencingEntity { get; set; }
    public string? ReferencedEntity { get; set; }
    public string? ReferencingAttribute { get; set; }
    public string? ReferencedAttribute { get; set; }
    public string? CascadeAssign { get; set; }
    public string? CascadeDelete { get; set; }
    public string? CascadeMerge { get; set; }
    public string? CascadeReparent { get; set; }
    public string? CascadeShare { get; set; }
    public string? CascadeUnshare { get; set; }
    public bool? IsCustomizable { get; set; }
    public bool? IsValidForAdvancedFind { get; set; }
    
    public XElement? OriginalXml { get; set; }
}
