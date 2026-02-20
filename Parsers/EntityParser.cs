using System.Xml.Linq;
using D365SolutionComparator.Models;

namespace D365SolutionComparator.Parsers;

public class EntityParser
{
    public List<EntityDefinition> ParseEntities(XDocument customizationsXml)
    {
        var entities = new List<EntityDefinition>();
        
        var entityNodes = customizationsXml.Descendants("Entity");
        
        foreach (var entityNode in entityNodes)
        {
            try
            {
                var entity = ParseEntity(entityNode);
                entities.Add(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Warning: Failed to parse entity: {ex.Message}");
            }
        }
        
        return entities;
    }
    
    private EntityDefinition ParseEntity(XElement entityNode)
    {
        var entity = new EntityDefinition
        {
            LogicalName = entityNode.Element("Name")?.Value ?? "Unknown",
            OriginalXml = entityNode
        };
        
        var entityInfo = entityNode.Element("EntityInfo")?.Element("entity");
        if (entityInfo != null)
        {
            // Parse display name
            entity.DisplayName = entityInfo.Element("LocalizedNames")
                ?.Elements("LocalizedName")
                ?.FirstOrDefault()?
                .Attribute("description")?.Value;
            
            // Parse plural name
            entity.PluralName = entityInfo.Element("LocalizedCollectionNames")
                ?.Elements("LocalizedCollectionName")
                ?.FirstOrDefault()?
                .Attribute("description")?.Value;
            
            // Parse description
            entity.Description = entityInfo.Element("Descriptions")
                ?.Elements("Description")
                ?.FirstOrDefault()?
                .Attribute("description")?.Value;
            
            // Parse boolean properties
            entity.IsActivity = ParseBool(entityInfo.Element("IsActivity")?.Value);
            entity.IsCustomEntity = ParseBool(entityInfo.Element("IsCustomEntity")?.Value);
            entity.IsAuditEnabled = ParseBool(entityInfo.Element("IsAuditEnabled")?.Value);
            entity.IsValidForQueue = ParseBool(entityInfo.Element("IsValidForQueue")?.Value);
            entity.IsConnectionsEnabled = ParseBool(entityInfo.Element("IsConnectionsEnabled")?.Value);
            entity.IsDuplicateDetectionEnabled = ParseBool(entityInfo.Element("IsDuplicateDetectionEnabled")?.Value);
            entity.IsMailMergeEnabled = ParseBool(entityInfo.Element("IsMailMergeEnabled")?.Value);
            entity.EntityHelpUrlEnabled = ParseBool(entityInfo.Element("EntityHelpUrlEnabled")?.Value);
            entity.AutoRouteToOwnerQueue = ParseBool(entityInfo.Element("AutoRouteToOwnerQueue")?.Value);
            
            // Parse string properties
            entity.OwnershipType = entityInfo.Element("OwnershipType")?.Value;
            entity.PrimaryNameAttribute = entityInfo.Element("PrimaryNameAttribute")?.Value;
            entity.PrimaryImageAttribute = entityInfo.Element("PrimaryImageAttribute")?.Value;
            entity.EntityColor = entityInfo.Element("EntityColor")?.Value;
            
            // Parse attributes
            var attributeParser = new AttributeParser();
            entity.Attributes = attributeParser.ParseAttributes(entityInfo);
            
            // Parse relationships
            var relationshipParser = new RelationshipParser();
            entity.Relationships = relationshipParser.ParseRelationships(entityNode);
        }
        
        // Parse forms
        var formParser = new FormParser();
        entity.Forms = formParser.ParseForms(entityNode);
        
        // Parse views
        var viewParser = new ViewParser();
        entity.Views = viewParser.ParseViews(entityNode);
        
        // Parse business rules (workflows with category = 2)
        entity.BusinessRules = ParseBusinessRules(entityNode);
        
        return entity;
    }
    
    private List<BusinessRuleDefinition> ParseBusinessRules(XElement entityNode)
    {
        var businessRules = new List<BusinessRuleDefinition>();
        
        // Business rules are stored in workflows with category = 2
        var workflowNodes = entityNode.Descendants("Workflow")
            .Where(w => w.Element("Category")?.Value == "2");
        
        foreach (var workflowNode in workflowNodes)
        {
            var businessRule = new BusinessRuleDefinition
            {
                Name = workflowNode.Element("Name")?.Value ?? "Unknown",
                Description = workflowNode.Element("Description")?.Value,
                Scope = workflowNode.Element("Scope")?.Value,
                OriginalXml = workflowNode
            };
            
            businessRules.Add(businessRule);
        }
        
        return businessRules;
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
