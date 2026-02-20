using D365SolutionComparator.Models;

namespace D365SolutionComparator.ReportGenerators;

public interface IReportGenerator
{
    void GenerateReport(ComparisonResult result, string outputPath);
}
