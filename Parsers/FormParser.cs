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
            FormXml = formNode.Element("form")?.ToString(),
            OriginalXml = formNode
        };
        
        // The <form> element is a direct child of <systemform>
        var formElement = formNode.Element("form");
        if (formElement != null)
        {
            ParseFormStructure(form, formElement);
        }
        
        return form;
    }
    
    private void ParseFormStructure(FormDefinition form, XElement formElement)
    {
        // Parse JavaScript libraries
        var jsLibraries = formElement.Descendants("Library")
            .Select(lib => lib.Attribute("name")?.Value)
            .Where(name => !string.IsNullOrEmpty(name))
            .ToList();
        
        if (jsLibraries.Any())
        {
            form.JavaScriptLibraries = jsLibraries!;
        }
        
        // Parse form-level event handlers
        form.EventHandlers = ParseEventHandlers(formElement.Element("events"));
        
        // Parse tabs directly from the form element
        var tabsElement = formElement.Element("tabs");
        if (tabsElement != null)
        {
            form.Tabs = ParseTabs(tabsElement);
        }
        
        // Parse header
        var headerElement = formElement.Element("header");
        if (headerElement != null)
        {
            form.Header = ParseHeader(headerElement);
        }
        
        // Parse footer
        var footerElement = formElement.Element("footer");
        if (footerElement != null)
        {
            form.Footer = ParseFooter(footerElement);
        }
        
        // Parse navigation
        var navigationElement = formElement.Element("Navigation");
        if (navigationElement != null)
        {
            form.Navigation = ParseNavigation(navigationElement);
        }
    }
    
    private List<FormEventHandler> ParseEventHandlers(XElement? eventsElement)
    {
        var handlers = new List<FormEventHandler>();
        if (eventsElement == null) return handlers;
        
        foreach (var eventElement in eventsElement.Elements("event"))
        {
            var eventName = eventElement.Attribute("name")?.Value;
            var handlersElement = eventElement.Element("Handlers");
            if (handlersElement == null) continue;
            
            int order = 0;
            foreach (var handlerElement in handlersElement.Elements("Handler"))
            {
                handlers.Add(new FormEventHandler
                {
                    Event = eventName,
                    Library = handlerElement.Attribute("libraryName")?.Value,
                    FunctionName = handlerElement.Attribute("functionName")?.Value,
                    Enabled = ParseBool(handlerElement.Attribute("enabled")?.Value),
                    Parameters = handlerElement.Attribute("parameters")?.Value,
                    Order = order++
                });
            }
        }
        
        return handlers;
    }
    
    private List<FormTab> ParseTabs(XElement tabsElement)
    {
        var tabs = new List<FormTab>();
        int tabOrder = 0;
        
        foreach (var tabElement in tabsElement.Elements("tab"))
        {
            var tab = new FormTab
            {
                Id = tabElement.Attribute("id")?.Value,
                Name = tabElement.Attribute("name")?.Value,
                Visible = ParseBool(tabElement.Attribute("visible")?.Value),
                Expanded = ParseBool(tabElement.Attribute("expanded")?.Value),
                ShowLabel = ParseBool(tabElement.Attribute("showlabel")?.Value),
                Order = tabOrder++
            };
            
            // Parse tab label
            var labelsElement = tabElement.Element("labels");
            if (labelsElement != null)
            {
                tab.Label = labelsElement.Elements("label")
                    .FirstOrDefault()?
                    .Attribute("description")?.Value;
            }
            
            // Parse sections
            var columnsElement = tabElement.Element("columns");
            if (columnsElement != null)
            {
                tab.Sections = ParseSections(columnsElement);
            }
            
            tabs.Add(tab);
        }
        
        return tabs;
    }
    
    private List<FormSection> ParseSections(XElement columnsElement)
    {
        var sections = new List<FormSection>();
        int sectionOrder = 0;
        
        foreach (var columnElement in columnsElement.Elements("column"))
        {
            var sectionsElement = columnElement.Element("sections");
            if (sectionsElement == null) continue;
            
            foreach (var sectionElement in sectionsElement.Elements("section"))
            {
                var section = new FormSection
                {
                    Id = sectionElement.Attribute("id")?.Value,
                    Name = sectionElement.Attribute("name")?.Value,
                    Visible = ParseBool(sectionElement.Attribute("visible")?.Value),
                    ShowLabel = ParseBool(sectionElement.Attribute("showlabel")?.Value),
                    ShowBar = ParseBool(sectionElement.Attribute("showbar")?.Value),
                    Order = sectionOrder++,
                    Columns = ParseInt(sectionElement.Attribute("columns")?.Value)
                };
                
                // Parse section label
                var labelsElement = sectionElement.Element("labels");
                if (labelsElement != null)
                {
                    section.Label = labelsElement.Elements("label")
                        .FirstOrDefault()?
                        .Attribute("description")?.Value;
                }
                
                // Parse controls
                var rowsElement = sectionElement.Element("rows");
                if (rowsElement != null)
                {
                    section.Controls = ParseControls(rowsElement);
                }
                
                sections.Add(section);
            }
        }
        
        return sections;
    }
    
    private List<FormControl> ParseControls(XElement rowsElement)
    {
        var controls = new List<FormControl>();
        
        int rowIndex = 0;
        foreach (var rowElement in rowsElement.Elements("row"))
        {
            var cellElement = rowElement.Element("cell");
            if (cellElement != null)
            {
                int colIndex = 0;
                foreach (var cell in rowElement.Elements("cell"))
                {
                    var controlElement = cell.Element("control");
                    if (controlElement != null)
                    {
                        var control = ParseControl(controlElement);
                        control.Row = rowIndex;
                        control.Col = colIndex;
                        control.RowSpan = ParseInt(cell.Attribute("rowspan")?.Value) ?? 1;
                        control.ColSpan = ParseInt(cell.Attribute("colspan")?.Value) ?? 1;
                        controls.Add(control);
                    }
                    colIndex++;
                }
            }
            rowIndex++;
        }
        
        return controls;
    }
    
    private FormControl ParseControl(XElement controlElement)
    {
        var control = new FormControl
        {
            Id = controlElement.Attribute("id")?.Value,
            DataField = controlElement.Attribute("datafieldname")?.Value ?? controlElement.Attribute("datafield")?.Value,
            ClassId = controlElement.Attribute("classid")?.Value,
            Visible = ParseBool(controlElement.Attribute("visible")?.Value),
            Disabled = ParseBool(controlElement.Attribute("disabled")?.Value),
            ShowLabel = ParseBool(controlElement.Attribute("showlabel")?.Value)
        };
        
        // Determine control type from classid
        control.ControlType = GetControlTypeName(control.ClassId);
        
        // Parse control label
        var labelsElement = controlElement.Element("labels");
        if (labelsElement != null)
        {
            control.Label = labelsElement.Elements("label")
                .FirstOrDefault()?
                .Attribute("description")?.Value;
        }
        
        // Parse parameters
        var parametersElement = controlElement.Element("parameters");
        if (parametersElement != null)
        {
            control.Parameters = ParseControlParameters(parametersElement);
        }
        
        // Parse control-level event handlers
        control.EventHandlers = ParseEventHandlers(controlElement.Element("events"));
        
        return control;
    }
    
    private List<FormControlParameter> ParseControlParameters(XElement parametersElement)
    {
        var parameters = new List<FormControlParameter>();
        
        foreach (var paramElement in parametersElement.Elements())
        {
            parameters.Add(new FormControlParameter
            {
                Name = paramElement.Name.LocalName,
                Value = paramElement.Value
            });
        }
        
        return parameters;
    }
    
    private FormHeader ParseHeader(XElement headerElement)
    {
        var header = new FormHeader();
        var rowsElement = headerElement.Element("rows");
        if (rowsElement != null)
        {
            header.Controls = ParseControls(rowsElement);
        }
        return header;
    }
    
    private FormFooter ParseFooter(XElement footerElement)
    {
        var footer = new FormFooter();
        var rowsElement = footerElement.Element("rows");
        if (rowsElement != null)
        {
            footer.Controls = ParseControls(rowsElement);
        }
        return footer;
    }
    
    private List<FormNavigationItem> ParseNavigation(XElement navigationElement)
    {
        var navItems = new List<FormNavigationItem>();
        int order = 0;
        
        var navBarElement = navigationElement.Element("NavBar");
        if (navBarElement == null) return navItems;
        
        foreach (var navBarItemElement in navBarElement.Elements("NavBarItem"))
        {
            navItems.Add(new FormNavigationItem
            {
                Id = navBarItemElement.Attribute("Id")?.Value,
                RelationshipName = navBarItemElement.Attribute("RelationshipName")?.Value,
                TargetEntity = navBarItemElement.Attribute("TargetEntityType")?.Value,
                Visible = ParseBool(navBarItemElement.Attribute("Visible")?.Value),
                Order = order++
            });
        }
        
        return navItems;
    }
    
    private string GetControlTypeName(string? classId)
    {
        return classId?.ToUpper() switch
        {
            "{4273EDBD-AC1D-40D3-9FB2-095C621B552D}" => "TextBox",
            "{5D68B988-0661-4DB2-BC3E-17598AD3BE6C}" => "Lookup",
            "{3EF39988-22BB-4F0B-BBBE-64B5A3748AEE}" => "OptionSet",
            "{67FAC785-CD58-4F9F-ABB3-4B7DDC6ED5ED}" => "MultiSelectOptionSet",
            "{06375649-C143-495E-A496-C962E5B4488E}" => "CheckBox",
            "{C3EFE0C3-0EC6-42BE-8349-CBD9079DFD8E}" => "DateTime",
            "{533B9E00-756B-4312-95A0-DC888637AC78}" => "Decimal",
            "{C6D124CA-7EDA-4A60-AEA9-7FB8D318B68F}" => "Integer",
            "{ADA2203E-B4CD-49BE-9DDF-234642B43B52}" => "Float",
            "{F9A8A302-114E-466A-B582-6771B2AE0D92}" => "Money",
            "{E0DECE4B-6FC8-4A8F-A065-082708572369}" => "MultiLine",
            "{71716B6C-711E-476C-8AB8-5D11542BFB47}" => "Subgrid",
            "{9FDF5F91-88B1-47F4-AD53-C11EFC01A01D}" => "IFrame",
            "{FD2A7985-3187-444E-908D-6624B4F19A90}" => "WebResource",
            "{F3015350-44A2-4AA0-97B5-00166532B5E9}" => "Notes",
            "{06A6EE7C-1B4A-4E9D-8C66-A45AD998F4B3}" => "Timer",
            _ => classId ?? "Unknown"
        };
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
    
    private int? ParseInt(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;
        
        if (int.TryParse(value, out int result))
            return result;
        
        return null;
    }
}
