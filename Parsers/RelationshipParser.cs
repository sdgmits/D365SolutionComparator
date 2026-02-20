using System.Xml.Linq;
using D365SolutionComparator.Models;

namespace D365SolutionComparator.Parsers;

public class RelationshipParser
{
    public List<RelationshipDefinition> ParseRelationships(XElement entityNode)
    {
        var relationships = new List<RelationshipDefinition>();
        
        var relationshipNodes = entityNode.Element("EntityRelationships")?.Elements("EntityRelationship");
        if (relationshipNodes == null) return relationships;
        
        foreach (var relNode in relationshipNodes)
        {
            try
            {
                var relationship = ParseRelationship(relNode);
                relationships.Add(relationship);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Warning: Failed to parse relationship: {ex.Message}");
            }
        }
        
        return relationships;
    }
    
    private RelationshipDefinition ParseRelationship(XElement relNode)
    {
        var relationship = new RelationshipDefinition
        {
            SchemaName = relNode.Attribute("Name")?.Value ?? "Unknown",
            RelationshipType = relNode.Element("EntityRelationshipType")?.Value,
            ReferencingEntity = relNode.Element("EntityRelationshipRoles")
                ?.Element("EntityRelationshipRole")
                ?.Element("NavPaneDisplayOption")?.Value,
            ReferencedEntity = relNode.Element("EntityRelationshipRoles")
                ?.Elements("EntityRelationshipRole")
                ?.Skip(1)
                ?.FirstOrDefault()?
                .Element("NavPaneDisplayOption")?.Value,
            ReferencingAttribute = relNode.Element("ReferencingEntityNavigationPropertyName")?.Value,
            ReferencedAttribute = relNode.Element("ReferencedAttribute")?.Value,
            IsCustomizable = ParseBool(relNode.Element("IsCustomizable")?.Value),
            IsValidForAdvancedFind = ParseBool(relNode.Element("IsValidForAdvancedFind")?.Value),
            OriginalXml = relNode
        };
        
        // Parse cascade configuration
        var cascadeConfig = relNode.Element("CascadeConfiguration");
        if (cascadeConfig != null)
        {
            relationship.CascadeAssign = cascadeConfig.Element("Assign")?.Value;
            relationship.CascadeDelete = cascadeConfig.Element("Delete")?.Value;
            relationship.CascadeMerge = cascadeConfig.Element("Merge")?.Value;
            relationship.CascadeReparent = cascadeConfig.Element("Reparent")?.Value;
            relationship.CascadeShare = cascadeConfig.Element("Share")?.Value;
            relationship.CascadeUnshare = cascadeConfig.Element("Unshare")?.Value;
        }
        
        return relationship;
    }
    
    private bool? ParseBool(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;
        
        if (value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase))
            return true;
        
        if (value == "0" || value.Equals("false", StringComparison.OrdinalIgnoreCase))
            return false;
        
        return null;
    }
}
