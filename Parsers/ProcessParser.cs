using System.Xml.Linq;
using D365SolutionComparator.Models;

namespace D365SolutionComparator.Parsers;

public class ProcessParser
{
    public List<ProcessDefinition> ParseProcesses(XDocument customizationsXml)
    {
        var processes = new List<ProcessDefinition>();
        
        var workflowNodes = customizationsXml.Descendants("Workflow");
        
        foreach (var workflowNode in workflowNodes)
        {
            try
            {
                var process = ParseProcess(workflowNode);
                processes.Add(process);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Warning: Failed to parse process: {ex.Message}");
            }
        }
        
        return processes;
    }
    
    private ProcessDefinition ParseProcess(XElement workflowNode)
    {
        var process = new ProcessDefinition
        {
            Name = workflowNode.Element("Name")?.Value ?? "Unknown",
            Category = GetCategoryName(workflowNode.Element("Category")?.Value),
            ProcessType = workflowNode.Element("Type")?.Value,
            IsTransacted = ParseBool(workflowNode.Element("IsTransacted")?.Value),
            Scope = GetScopeName(workflowNode.Element("Scope")?.Value),
            Mode = GetModeName(workflowNode.Element("Mode")?.Value),
            XamlDefinition = workflowNode.Element("Xaml")?.Value,
            OriginalXml = workflowNode
        };
        
        return process;
    }
    
    private string GetCategoryName(string? categoryValue)
    {
        return categoryValue switch
        {
            "0" => "Workflow",
            "1" => "Dialog",
            "2" => "Business Rule",
            "3" => "Action",
            "4" => "Business Process Flow",
            "5" => "Modern Flow",
            _ => categoryValue ?? "Unknown"
        };
    }
    
    private string GetScopeName(string? scopeValue)
    {
        return scopeValue switch
        {
            "1" => "User",
            "2" => "Business Unit",
            "3" => "Parent: Child Business Units",
            "4" => "Organization",
            _ => scopeValue ?? "Unknown"
        };
    }
    
    private string GetModeName(string? modeValue)
    {
        return modeValue switch
        {
            "0" => "Background",
            "1" => "Real-time",
            _ => modeValue ?? "Unknown"
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
