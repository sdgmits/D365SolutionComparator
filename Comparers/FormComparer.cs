using D365SolutionComparator.Models;

namespace D365SolutionComparator.Comparers;

public class FormComparer : IComparer<FormDefinition>
{
    public ComparisonResult Compare(FormDefinition? source, FormDefinition? target)
    {
        var result = new ComparisonResult
        {
            ComponentType = "Form",
            ComponentName = source?.Name ?? target?.Name ?? "Unknown"
        };
        
        if (source == null && target != null)
        {
            result.ChangeType = ChangeType.Added;
            return result;
        }
        else if (source != null && target == null)
        {
            result.ChangeType = ChangeType.Removed;
            return result;
        }
        
        if (source == null || target == null)
        {
            result.ChangeType = ChangeType.Unchanged;
            return result;
        }
        
        // Compare basic properties
        CompareProperty(result, "FormType", source.FormType, target.FormType);
        CompareProperty(result, "IsDefault", source.IsDefault, target.IsDefault);
        CompareProperty(result, "IsDesktop", source.IsDesktop, target.IsDesktop);
        CompareProperty(result, "IsTablet", source.IsTablet, target.IsTablet);
        CompareProperty(result, "IsPhone", source.IsPhone, target.IsPhone);
        
        // Compare JavaScript libraries
        CompareListProperty(result, "JavaScriptLibraries", source.JavaScriptLibraries, target.JavaScriptLibraries);
        
        // Compare form-level event handlers
        CompareEventHandlers(result, "FormEventHandlers", source.EventHandlers, target.EventHandlers);
        
        // Compare tabs
        CompareTabs(result, source.Tabs, target.Tabs);
        
        // Compare header
        CompareHeader(result, source.Header, target.Header);
        
        // Compare footer
        CompareFooter(result, source.Footer, target.Footer);
        
        // Compare navigation
        CompareNavigation(result, source.Navigation, target.Navigation);
        
        result.ChangeType = result.PropertyChanges.Any(p => p.Value.IsDifferent)
            ? ChangeType.Modified
            : ChangeType.Unchanged;
        
        return result;
    }
    
    private void CompareEventHandlers(ComparisonResult result, string prefix, List<FormEventHandler>? source, List<FormEventHandler>? target)
    {
        var sourceHandlers = source ?? new List<FormEventHandler>();
        var targetHandlers = target ?? new List<FormEventHandler>();
        
        // Compare count
        CompareProperty(result, $"{prefix}.Count", sourceHandlers.Count, targetHandlers.Count);
        
        // Compare each handler
        var maxCount = Math.Max(sourceHandlers.Count, targetHandlers.Count);
        for (int i = 0; i < maxCount; i++)
        {
            var sourceHandler = i < sourceHandlers.Count ? sourceHandlers[i] : null;
            var targetHandler = i < targetHandlers.Count ? targetHandlers[i] : null;
            
            if (sourceHandler == null && targetHandler != null)
            {
                AddPropertyChange(result, $"{prefix}[{i}]", "null", FormatEventHandler(targetHandler), true);
            }
            else if (sourceHandler != null && targetHandler == null)
            {
                AddPropertyChange(result, $"{prefix}[{i}]", FormatEventHandler(sourceHandler), "null", true);
            }
            else if (sourceHandler != null && targetHandler != null)
            {
                CompareProperty(result, $"{prefix}[{i}].Event", sourceHandler.Event, targetHandler.Event);
                CompareProperty(result, $"{prefix}[{i}].Library", sourceHandler.Library, targetHandler.Library);
                CompareProperty(result, $"{prefix}[{i}].FunctionName", sourceHandler.FunctionName, targetHandler.FunctionName);
                CompareProperty(result, $"{prefix}[{i}].Enabled", sourceHandler.Enabled, targetHandler.Enabled);
                CompareProperty(result, $"{prefix}[{i}].Parameters", sourceHandler.Parameters, targetHandler.Parameters);
                CompareProperty(result, $"{prefix}[{i}].Order", sourceHandler.Order, targetHandler.Order);
            }
        }
    }
    
    private void CompareTabs(ComparisonResult result, List<FormTab>? source, List<FormTab>? target)
    {
        var sourceTabs = source ?? new List<FormTab>();
        var targetTabs = target ?? new List<FormTab>();
        
        // Compare tab count
        CompareProperty(result, "Tabs.Count", sourceTabs.Count, targetTabs.Count);
        
        // Match tabs by name/id - handle duplicates by using index
        var sourceTabDict = new Dictionary<string, FormTab>();
        for (int i = 0; i < sourceTabs.Count; i++)
        {
            var key = $"{sourceTabs[i].Name ?? sourceTabs[i].Id ?? ""}_{i}";
            sourceTabDict[key] = sourceTabs[i];
        }
        
        var targetTabDict = new Dictionary<string, FormTab>();
        for (int i = 0; i < targetTabs.Count; i++)
        {
            var key = $"{targetTabs[i].Name ?? targetTabs[i].Id ?? ""}_{i}";
            targetTabDict[key] = targetTabs[i];
        }
        
        var allTabNames = sourceTabDict.Keys.Union(targetTabDict.Keys).ToList();
        
        foreach (var tabName in allTabNames)
        {
            var sourceTab = sourceTabDict.ContainsKey(tabName) ? sourceTabDict[tabName] : null;
            var targetTab = targetTabDict.ContainsKey(tabName) ? targetTabDict[tabName] : null;
            
            var displayName = sourceTab?.Name ?? targetTab?.Name ?? tabName;
            
            if (sourceTab == null && targetTab != null)
            {
                AddPropertyChange(result, $"Tab[{displayName}]", "null", "Added", true);
            }
            else if (sourceTab != null && targetTab == null)
            {
                AddPropertyChange(result, $"Tab[{displayName}]", "Exists", "Removed", true);
            }
            else if (sourceTab != null && targetTab != null)
            {
                CompareProperty(result, $"Tab[{displayName}].Label", sourceTab.Label, targetTab.Label);
                CompareProperty(result, $"Tab[{displayName}].Visible", sourceTab.Visible, targetTab.Visible);
                CompareProperty(result, $"Tab[{displayName}].Expanded", sourceTab.Expanded, targetTab.Expanded);
                CompareProperty(result, $"Tab[{displayName}].ShowLabel", sourceTab.ShowLabel, targetTab.ShowLabel);
                CompareProperty(result, $"Tab[{displayName}].Order", sourceTab.Order, targetTab.Order);
                
                // Compare sections
                CompareSections(result, $"Tab[{displayName}]", sourceTab.Sections, targetTab.Sections);
            }
        }
    }
    
    private void CompareSections(ComparisonResult result, string tabPrefix, List<FormSection>? source, List<FormSection>? target)
    {
        var sourceSections = source ?? new List<FormSection>();
        var targetSections = target ?? new List<FormSection>();
        
        CompareProperty(result, $"{tabPrefix}.Sections.Count", sourceSections.Count, targetSections.Count);
        
        // Match sections by name/id - handle duplicates by using index
        var sourceSectionDict = new Dictionary<string, FormSection>();
        for (int i = 0; i < sourceSections.Count; i++)
        {
            var key = $"{sourceSections[i].Name ?? sourceSections[i].Id ?? ""}_{i}";
            sourceSectionDict[key] = sourceSections[i];
        }
        
        var targetSectionDict = new Dictionary<string, FormSection>();
        for (int i = 0; i < targetSections.Count; i++)
        {
            var key = $"{targetSections[i].Name ?? targetSections[i].Id ?? ""}_{i}";
            targetSectionDict[key] = targetSections[i];
        }
        
        var allSectionNames = sourceSectionDict.Keys.Union(targetSectionDict.Keys).ToList();
        
        foreach (var sectionName in allSectionNames)
        {
            var sourceSection = sourceSectionDict.ContainsKey(sectionName) ? sourceSectionDict[sectionName] : null;
            var targetSection = targetSectionDict.ContainsKey(sectionName) ? targetSectionDict[sectionName] : null;
            
            var displayName = sourceSection?.Name ?? targetSection?.Name ?? sectionName;
            
            if (sourceSection == null && targetSection != null)
            {
                AddPropertyChange(result, $"{tabPrefix}.Section[{displayName}]", "null", "Added", true);
            }
            else if (sourceSection != null && targetSection == null)
            {
                AddPropertyChange(result, $"{tabPrefix}.Section[{displayName}]", "Exists", "Removed", true);
            }
            else if (sourceSection != null && targetSection != null)
            {
                CompareProperty(result, $"{tabPrefix}.Section[{displayName}].Label", sourceSection.Label, targetSection.Label);
                CompareProperty(result, $"{tabPrefix}.Section[{displayName}].Visible", sourceSection.Visible, targetSection.Visible);
                CompareProperty(result, $"{tabPrefix}.Section[{displayName}].ShowLabel", sourceSection.ShowLabel, targetSection.ShowLabel);
                CompareProperty(result, $"{tabPrefix}.Section[{displayName}].ShowBar", sourceSection.ShowBar, targetSection.ShowBar);
                CompareProperty(result, $"{tabPrefix}.Section[{displayName}].Order", sourceSection.Order, targetSection.Order);
                CompareProperty(result, $"{tabPrefix}.Section[{displayName}].Columns", sourceSection.Columns, targetSection.Columns);
                
                // Compare controls
                CompareControls(result, $"{tabPrefix}.Section[{displayName}]", sourceSection.Controls, targetSection.Controls);
            }
        }
    }
    
    private void CompareControls(ComparisonResult result, string sectionPrefix, List<FormControl>? source, List<FormControl>? target)
    {
        var sourceControls = source ?? new List<FormControl>();
        var targetControls = target ?? new List<FormControl>();
        
        CompareProperty(result, $"{sectionPrefix}.Controls.Count", sourceControls.Count, targetControls.Count);
        
        // Match controls by DataField or Id - handle duplicates by using index
        var sourceControlDict = new Dictionary<string, FormControl>();
        for (int i = 0; i < sourceControls.Count; i++)
        {
            var key = $"{sourceControls[i].DataField ?? sourceControls[i].Id ?? ""}_{i}";
            sourceControlDict[key] = sourceControls[i];
        }
        
        var targetControlDict = new Dictionary<string, FormControl>();
        for (int i = 0; i < targetControls.Count; i++)
        {
            var key = $"{targetControls[i].DataField ?? targetControls[i].Id ?? ""}_{i}";
            targetControlDict[key] = targetControls[i];
        }
        
        var allControlNames = sourceControlDict.Keys.Union(targetControlDict.Keys).ToList();
        
        foreach (var controlName in allControlNames)
        {
            var sourceControl = sourceControlDict.ContainsKey(controlName) ? sourceControlDict[controlName] : null;
            var targetControl = targetControlDict.ContainsKey(controlName) ? targetControlDict[controlName] : null;
            
            var displayName = sourceControl?.DataField ?? targetControl?.DataField ?? 
                             sourceControl?.Id ?? targetControl?.Id ?? controlName;
            
            if (sourceControl == null && targetControl != null)
            {
                AddPropertyChange(result, $"{sectionPrefix}.Control[{displayName}]", "null", "Added", true);
            }
            else if (sourceControl != null && targetControl == null)
            {
                AddPropertyChange(result, $"{sectionPrefix}.Control[{displayName}]", "Exists", "Removed", true);
            }
            else if (sourceControl != null && targetControl != null)
            {
                CompareProperty(result, $"{sectionPrefix}.Control[{displayName}].ControlType", sourceControl.ControlType, targetControl.ControlType);
                CompareProperty(result, $"{sectionPrefix}.Control[{displayName}].Label", sourceControl.Label, targetControl.Label);
                CompareProperty(result, $"{sectionPrefix}.Control[{displayName}].Visible", sourceControl.Visible, targetControl.Visible);
                CompareProperty(result, $"{sectionPrefix}.Control[{displayName}].Disabled", sourceControl.Disabled, targetControl.Disabled);
                CompareProperty(result, $"{sectionPrefix}.Control[{displayName}].ShowLabel", sourceControl.ShowLabel, targetControl.ShowLabel);
                CompareProperty(result, $"{sectionPrefix}.Control[{displayName}].Row", sourceControl.Row, targetControl.Row);
                CompareProperty(result, $"{sectionPrefix}.Control[{displayName}].Col", sourceControl.Col, targetControl.Col);
                CompareProperty(result, $"{sectionPrefix}.Control[{displayName}].RowSpan", sourceControl.RowSpan, targetControl.RowSpan);
                CompareProperty(result, $"{sectionPrefix}.Control[{displayName}].ColSpan", sourceControl.ColSpan, targetControl.ColSpan);
                CompareProperty(result, $"{sectionPrefix}.Control[{displayName}].RequiredLevel", sourceControl.RequiredLevel, targetControl.RequiredLevel);
                
                // Compare control parameters
                CompareControlParameters(result, $"{sectionPrefix}.Control[{displayName}]", sourceControl.Parameters, targetControl.Parameters);
                
                // Compare control event handlers
                CompareEventHandlers(result, $"{sectionPrefix}.Control[{displayName}].EventHandlers", sourceControl.EventHandlers, targetControl.EventHandlers);
            }
        }
    }
    
    private void CompareControlParameters(ComparisonResult result, string controlPrefix, List<FormControlParameter>? source, List<FormControlParameter>? target)
    {
        var sourceParams = source ?? new List<FormControlParameter>();
        var targetParams = target ?? new List<FormControlParameter>();
        
        if (sourceParams.Count == 0 && targetParams.Count == 0) return;
        
        CompareProperty(result, $"{controlPrefix}.Parameters.Count", sourceParams.Count, targetParams.Count);
        
        var sourceParamDict = sourceParams.ToDictionary(p => p.Name ?? "", p => p.Value);
        var targetParamDict = targetParams.ToDictionary(p => p.Name ?? "", p => p.Value);
        
        var allParamNames = sourceParamDict.Keys.Union(targetParamDict.Keys).ToList();
        
        foreach (var paramName in allParamNames)
        {
            var sourceValue = sourceParamDict.ContainsKey(paramName) ? sourceParamDict[paramName] : null;
            var targetValue = targetParamDict.ContainsKey(paramName) ? targetParamDict[paramName] : null;
            
            CompareProperty(result, $"{controlPrefix}.Parameters[{paramName}]", sourceValue, targetValue);
        }
    }
    
    private void CompareHeader(ComparisonResult result, FormHeader? source, FormHeader? target)
    {
        if (source == null && target == null) return;
        
        var sourceControls = source?.Controls ?? new List<FormControl>();
        var targetControls = target?.Controls ?? new List<FormControl>();
        
        CompareControls(result, "Header", sourceControls, targetControls);
    }
    
    private void CompareFooter(ComparisonResult result, FormFooter? source, FormFooter? target)
    {
        if (source == null && target == null) return;
        
        var sourceControls = source?.Controls ?? new List<FormControl>();
        var targetControls = target?.Controls ?? new List<FormControl>();
        
        CompareControls(result, "Footer", sourceControls, targetControls);
    }
    
    private void CompareNavigation(ComparisonResult result, List<FormNavigationItem>? source, List<FormNavigationItem>? target)
    {
        var sourceNav = source ?? new List<FormNavigationItem>();
        var targetNav = target ?? new List<FormNavigationItem>();
        
        if (sourceNav.Count == 0 && targetNav.Count == 0) return;
        
        CompareProperty(result, "Navigation.Count", sourceNav.Count, targetNav.Count);
        
        // Match navigation items by relationship name or id
        var sourceNavDict = sourceNav.ToDictionary(n => n.RelationshipName ?? n.Id ?? "", n => n);
        var targetNavDict = targetNav.ToDictionary(n => n.RelationshipName ?? n.Id ?? "", n => n);
        
        var allNavNames = sourceNavDict.Keys.Union(targetNavDict.Keys).ToList();
        
        foreach (var navName in allNavNames)
        {
            var sourceItem = sourceNavDict.ContainsKey(navName) ? sourceNavDict[navName] : null;
            var targetItem = targetNavDict.ContainsKey(navName) ? targetNavDict[navName] : null;
            
            if (sourceItem == null && targetItem != null)
            {
                AddPropertyChange(result, $"Navigation[{navName}]", "null", "Added", true);
            }
            else if (sourceItem != null && targetItem == null)
            {
                AddPropertyChange(result, $"Navigation[{navName}]", "Exists", "Removed", true);
            }
            else if (sourceItem != null && targetItem != null)
            {
                CompareProperty(result, $"Navigation[{navName}].TargetEntity", sourceItem.TargetEntity, targetItem.TargetEntity);
                CompareProperty(result, $"Navigation[{navName}].Visible", sourceItem.Visible, targetItem.Visible);
                CompareProperty(result, $"Navigation[{navName}].Order", sourceItem.Order, targetItem.Order);
            }
        }
    }
    
    private void CompareProperty<T>(ComparisonResult result, string propertyName, T? sourceValue, T? targetValue)
    {
        bool isDifferent = !EqualityComparer<T>.Default.Equals(sourceValue, targetValue);
        
        result.PropertyChanges[propertyName] = new PropertyChange
        {
            PropertyName = propertyName,
            SourceValue = sourceValue?.ToString(),
            TargetValue = targetValue?.ToString(),
            IsDifferent = isDifferent
        };
    }
    
    private void AddPropertyChange(ComparisonResult result, string propertyName, string? sourceValue, string? targetValue, bool isDifferent)
    {
        result.PropertyChanges[propertyName] = new PropertyChange
        {
            PropertyName = propertyName,
            SourceValue = sourceValue,
            TargetValue = targetValue,
            IsDifferent = isDifferent
        };
    }
    
    private void CompareListProperty(ComparisonResult result, string propertyName, List<string>? sourceList, List<string>? targetList)
    {
        var sourceStr = sourceList != null && sourceList.Any() ? string.Join(", ", sourceList) : null;
        var targetStr = targetList != null && targetList.Any() ? string.Join(", ", targetList) : null;
        
        bool isDifferent = sourceStr != targetStr;
        
        result.PropertyChanges[propertyName] = new PropertyChange
        {
            PropertyName = propertyName,
            SourceValue = sourceStr,
            TargetValue = targetStr,
            IsDifferent = isDifferent
        };
    }
    
    private string FormatEventHandler(FormEventHandler handler)
    {
        return $"{handler.Event}: {handler.Library}.{handler.FunctionName}";
    }
}
