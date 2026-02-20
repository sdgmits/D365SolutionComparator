using System.Xml.Linq;

namespace D365SolutionComparator.Models;

public class AttributeDefinition
{
    public string LogicalName { get; set; } = string.Empty;
    public string? SchemaName { get; set; }
    public string? AttributeType { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public string? RequiredLevel { get; set; }
    public bool? IsAuditEnabled { get; set; }
    public bool? IsSecured { get; set; }
    public bool? IsCustomizable { get; set; }
    
    // String-specific properties
    public int? MaxLength { get; set; }
    public string? Format { get; set; }
    public string? ImeMode { get; set; }
    
    // Integer/Decimal/Money-specific properties
    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }
    public int? Precision { get; set; }
    
    // DateTime-specific properties
    public string? DateTimeBehavior { get; set; }
    
    // Lookup-specific properties
    public List<string>? Targets { get; set; }
    
    // OptionSet-specific properties
    public Dictionary<int, string>? OptionSetValues { get; set; }
    
    // Two Options (Boolean)-specific properties
    public string? TrueOption { get; set; }
    public string? FalseOption { get; set; }
    
    public XElement? OriginalXml { get; set; }
}
