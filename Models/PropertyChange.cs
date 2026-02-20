namespace D365SolutionComparator.Models;

public class PropertyChange
{
    public string PropertyName { get; set; } = string.Empty;
    public string? SourceValue { get; set; }
    public string? TargetValue { get; set; }
    public bool IsDifferent { get; set; }
}
