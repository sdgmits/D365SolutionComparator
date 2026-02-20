using System.Xml.Linq;
using System.Security.Cryptography;
using System.Text;
using D365SolutionComparator.Models;

namespace D365SolutionComparator.Parsers;

public class WebResourceParser
{
    public List<WebResourceDefinition> ParseWebResources(XDocument customizationsXml)
    {
        var webResources = new List<WebResourceDefinition>();
        
        var webResourceNodes = customizationsXml.Descendants("WebResource");
        
        foreach (var wrNode in webResourceNodes)
        {
            try
            {
                var webResource = ParseWebResource(wrNode);
                webResources.Add(webResource);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Warning: Failed to parse web resource: {ex.Message}");
            }
        }
        
        return webResources;
    }
    
    private WebResourceDefinition ParseWebResource(XElement wrNode)
    {
        var webResource = new WebResourceDefinition
        {
            Name = wrNode.Element("Name")?.Value ?? "Unknown",
            DisplayName = wrNode.Element("DisplayName")?.Value,
            WebResourceType = GetWebResourceTypeName(wrNode.Element("WebResourceType")?.Value),
            IsCustomizable = ParseBool(wrNode.Element("IsCustomizable")?.Value),
            IsEnabledForMobileClient = ParseBool(wrNode.Element("IsEnabledForMobileClient")?.Value),
            LanguageCode = wrNode.Element("LanguageCode")?.Value,
            OriginalXml = wrNode
        };
        
        // Calculate content hash
        var content = wrNode.Element("Content")?.Value;
        if (!string.IsNullOrEmpty(content))
        {
            webResource.ContentHash = CalculateHash(content);
        }
        
        return webResource;
    }
    
    private string GetWebResourceTypeName(string? typeValue)
    {
        return typeValue switch
        {
            "1" => "HTML",
            "2" => "CSS",
            "3" => "JavaScript",
            "4" => "XML",
            "5" => "PNG",
            "6" => "JPG",
            "7" => "GIF",
            "8" => "XAP",
            "9" => "XSL",
            "10" => "ICO",
            "11" => "SVG",
            "12" => "String (RESX)",
            _ => typeValue ?? "Unknown"
        };
    }
    
    private string CalculateHash(string content)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash).Substring(0, 16);
        }
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
