namespace D365SolutionComparator.Utilities;

public static class Logger
{
    public static void Info(string message)
    {
        Console.WriteLine($"[INFO] {message}");
    }
    
    public static void Warning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[WARN] {message}");
        Console.ResetColor();
    }
    
    public static void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {message}");
        Console.ResetColor();
    }
    
    public static void Success(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[SUCCESS] {message}");
        Console.ResetColor();
    }
    
    public static void Debug(string message, bool verbose = false)
    {
        if (verbose)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"[DEBUG] {message}");
            Console.ResetColor();
        }
    }
}
