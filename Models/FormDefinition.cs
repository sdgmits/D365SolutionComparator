using System.Xml.Linq;

namespace D365SolutionComparator.Models;

public class FormDefinition
{
    public string FormId { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? FormType { get; set; }
    public bool? IsDefault { get; set; }
    public bool? IsDesktop { get; set; }
    public bool? IsTablet { get; set; }
    public bool? IsPhone { get; set; }
    public string? FormXml { get; set; }
    public List<string>? JavaScriptLibraries { get; set; }
    
    // Event Handlers
    public List<FormEventHandler>? EventHandlers { get; set; }
    
    // Form Structure
    public List<FormTab>? Tabs { get; set; }
    public FormHeader? Header { get; set; }
    public FormFooter? Footer { get; set; }
    public List<FormNavigationItem>? Navigation { get; set; }
    
    public XElement? OriginalXml { get; set; }
}

public class FormEventHandler
{
    public string? Event { get; set; } // OnLoad, OnSave, etc.
    public string? Library { get; set; }
    public string? FunctionName { get; set; }
    public bool? Enabled { get; set; }
    public string? Parameters { get; set; }
    public int? Order { get; set; }
}

public class FormTab
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Label { get; set; }
    public bool? Visible { get; set; }
    public bool? Expanded { get; set; }
    public bool? ShowLabel { get; set; }
    public int? Order { get; set; }
    public List<FormSection>? Sections { get; set; }
}

public class FormSection
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Label { get; set; }
    public bool? Visible { get; set; }
    public bool? ShowLabel { get; set; }
    public bool? ShowBar { get; set; }
    public int? Order { get; set; }
    public int? Columns { get; set; }
    public List<FormControl>? Controls { get; set; }
}

public class FormControl
{
    public string? Id { get; set; }
    public string? DataField { get; set; }
    public string? ClassId { get; set; } // Control type GUID
    public string? ControlType { get; set; } // Friendly name
    public bool? Visible { get; set; }
    public bool? Disabled { get; set; }
    public string? Label { get; set; }
    public bool? ShowLabel { get; set; }
    public int? Order { get; set; }
    public int? RowSpan { get; set; }
    public int? ColSpan { get; set; }
    public int? Row { get; set; }
    public int? Col { get; set; }
    public string? RequiredLevel { get; set; }
    public List<FormControlParameter>? Parameters { get; set; }
    public List<FormEventHandler>? EventHandlers { get; set; }
}

public class FormControlParameter
{
    public string? Name { get; set; }
    public string? Value { get; set; }
}

public class FormHeader
{
    public List<FormControl>? Controls { get; set; }
}

public class FormFooter
{
    public List<FormControl>? Controls { get; set; }
}

public class FormNavigationItem
{
    public string? Id { get; set; }
    public string? RelationshipName { get; set; }
    public string? TargetEntity { get; set; }
    public bool? Visible { get; set; }
    public int? Order { get; set; }
}
