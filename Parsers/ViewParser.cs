using System.Xml.Linq;
using D365SolutionComparator.Models;

namespace D365SolutionComparator.Parsers;

public class ViewParser
{
    public List<ViewDefinition> ParseViews(XElement entityNode)
    {
        var views = new List<ViewDefinition>();
        
        var viewNodes = entityNode.Element("SavedQueries")?.Element("savedqueries")?.Elements("savedquery");
        if (viewNodes == null) return views;
        
        foreach (var viewNode in viewNodes)
        {
            try
            {
                var view = ParseView(viewNode);
                views.Add(view);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Warning: Failed to parse view: {ex.Message}");
            }
        }
        
        return views;
    }
    
    private ViewDefinition ParseView(XElement viewNode)
    {
        var view = new ViewDefinition
        {
            SavedQueryId = viewNode.Element("savedqueryid")?.Value ?? Guid.NewGuid().ToString(),
            Name = viewNode.Element("LocalizedNames")
                ?.Elements("LocalizedName")
                ?.FirstOrDefault()?
                .Attribute("description")?.Value ?? 
                viewNode.Element("name")?.Value ?? "Unknown",
            QueryType = viewNode.Element("querytype")?.Value,
            FetchXml = viewNode.Element("fetchxml")?.Value,
            LayoutXml = viewNode.Element("layoutxml")?.Value,
            ColumnSetXml = viewNode.Element("columnsetxml")?.Value,
            IsDefault = ParseBool(viewNode.Element("isdefault")?.Value),
            IsUserDefined = ParseBool(viewNode.Element("isuserdefined")?.Value),
            ReturnedTypeCode = viewNode.Element("returnedtypecode")?.Value,
            OriginalXml = viewNode
        };
        
        return view;
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
