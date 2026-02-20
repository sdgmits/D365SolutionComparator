using D365SolutionComparator.Models;

namespace D365SolutionComparator.Comparers;

public interface IComparer<T>
{
    ComparisonResult Compare(T? source, T? target);
}
