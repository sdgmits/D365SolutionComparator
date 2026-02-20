using System.Xml.Linq;

namespace D365SolutionComparator.Models;

public class ViewDefinition
{
    public string SavedQueryId { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? QueryType { get; set; }
    public string? FetchXml { get; set; }
    public string? LayoutXml { get; set; }
    public string? ColumnSetXml { get; set; }
    public bool? IsDefault { get; set; }
    public bool? IsUserDefined { get; set; }
    public string? ReturnedTypeCode { get; set; }
    
    public XElement? OriginalXml { get; set; }
}
