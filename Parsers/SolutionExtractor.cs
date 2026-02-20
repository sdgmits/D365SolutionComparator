using System.IO.Compression;

namespace D365SolutionComparator.Parsers;

public class SolutionExtractor
{
    public string ExtractSolution(string inputPath)
    {
        // Check if input is ZIP file or directory
        if (File.Exists(inputPath) && Path.GetExtension(inputPath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
        {
            // Extract to temp directory
            string tempDir = Path.Combine(Path.GetTempPath(), "D365SolutionComparator", Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            
            Console.WriteLine($"  Extracting ZIP: {Path.GetFileName(inputPath)}");
            ZipFile.ExtractToDirectory(inputPath, tempDir);
            Console.WriteLine($"  Extracted to: {tempDir}");
            
            return tempDir;
        }
        else if (Directory.Exists(inputPath))
        {
            // Already extracted
            Console.WriteLine($"  Using extracted directory: {inputPath}");
            return inputPath;
        }
        else
        {
            throw new ArgumentException($"Input must be a ZIP file or extracted directory: {inputPath}");
        }
    }
    
    public string GetCustomizationsXmlPath(string solutionDirectory)
    {
        // Typically located at: solutionDirectory/customizations.xml
        string customizationsPath = Path.Combine(solutionDirectory, "customizations.xml");
        
        if (!File.Exists(customizationsPath))
        {
            throw new FileNotFoundException($"customizations.xml not found in solution directory: {solutionDirectory}");
        }
        
        return customizationsPath;
    }
    
    public void Cleanup(string directory, string originalInputPath)
    {
        // Only cleanup if we extracted a ZIP (temp directory)
        if (!string.Equals(directory, originalInputPath, StringComparison.OrdinalIgnoreCase) 
            && directory.Contains(Path.GetTempPath()))
        {
            try
            {
                Directory.Delete(directory, true);
                Console.WriteLine($"  Cleaned up temp directory: {directory}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Warning: Could not cleanup temp directory: {ex.Message}");
            }
        }
    }
}
