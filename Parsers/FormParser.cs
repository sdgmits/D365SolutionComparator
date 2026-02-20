using System.Xml.Linq;
using D365SolutionComparator.Models;

namespace D365SolutionComparator.Parsers;

public class FormParser
{
    public List<FormDefinition> ParseForms(XElement entityNode)
    {
        var forms = new List<FormDefinition>();
        
        var formNodes = entityNode.Element("FormXml")?.Element("forms")?.Elements("systemform");
        if (formNodes == null) return forms;
        
        foreach (var formNode in formNodes)
        {
            try
            {
                var form = ParseForm(formNode);
                forms.Add(form);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Warning: Failed to parse form: {ex.Message}");
            }
        }
        
        return forms;
    }
    
    private FormDefinition ParseForm(XElement formNode)
    {
        var form = new FormDefinition
        {
            FormId = formNode.Element("formid")?.Value ?? Guid.NewGuid().ToString(),
            Name = formNode.Element("LocalizedNames")
                ?.Elements("LocalizedName")
                ?.FirstOrDefault()?
                .Attribute("description")?.Value ?? "Unknown",
            FormType = GetFormTypeName(formNode.Element("type")?.Value),
            IsDefault = ParseBool(formNode.Element("IsDefault")?.Value),
            IsDesktop = ParseBool(formNode.Element("IsDesktop")?.Value),
            IsTablet = ParseBool(formNode.Element("IsTablet")?.Value),
            IsPhone = ParseBool(formNode.Element("IsPhone")?.Value),
            FormXml = formNode.Element("FormXml")?.ToString(),
            OriginalXml = formNode
        };
        
        // Parse JavaScript libraries
        var jsLibraries = formNode.Element("FormXml")?.Descendants("Library")
            .Select(lib => lib.Attribute("name")?.Value)
            .Where(name => !string.IsNullOrEmpty(name))
            .ToList();
        
        if (jsLibraries != null && jsLibraries.Any())
        {
            form.JavaScriptLibraries = jsLibraries!;
        }
        
        return form;
    }
    
    private string GetFormTypeName(string? typeValue)
    {
        return typeValue switch
        {
            "2" => "Main",
            "5" => "Mobile - Express",
            "6" => "Quick View",
            "7" => "Quick Create",
            "8" => "Dialog",
            "9" => "Task Flow",
            "10" => "Interactive Experience",
            "11" => "Card",
            "12" => "Main - Interactive Experience",
            _ => typeValue ?? "Unknown"
        };
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
