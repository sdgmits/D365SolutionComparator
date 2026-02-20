using System.Xml.Linq;

namespace D365SolutionComparator.Models;

public class EntityDefinition
{
    public string LogicalName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? PluralName { get; set; }
    public string? Description { get; set; }
    public string? OwnershipType { get; set; }
    public bool? IsActivity { get; set; }
    public bool? IsCustomEntity { get; set; }
    public bool? IsAuditEnabled { get; set; }
    public bool? IsValidForQueue { get; set; }
    public bool? IsConnectionsEnabled { get; set; }
    public bool? IsDuplicateDetectionEnabled { get; set; }
    public bool? IsMailMergeEnabled { get; set; }
    public string? PrimaryNameAttribute { get; set; }
    public string? PrimaryImageAttribute { get; set; }
    public string? EntityColor { get; set; }
    public bool? EntityHelpUrlEnabled { get; set; }
    public bool? AutoRouteToOwnerQueue { get; set; }
    
    public List<AttributeDefinition> Attributes { get; set; } = new();
    public List<RelationshipDefinition> Relationships { get; set; } = new();
    public List<FormDefinition> Forms { get; set; } = new();
    public List<ViewDefinition> Views { get; set; } = new();
    public List<BusinessRuleDefinition> BusinessRules { get; set; } = new();
    
    public XElement? OriginalXml { get; set; }
}
