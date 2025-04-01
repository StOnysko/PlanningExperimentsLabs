namespace PlanningExperimentsLabs.Data.Local;

public abstract class FileManager
{
    private static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

    public const string LaboratoryOneSorted = "LR1_NoThreadUsageSorting.txt";
    public const string LaboratoryOneThreadUsage = "LR1_ThreadUsageSorting.txt";

    public static void CreateFile(string fileName, List<Data.Weather.Weather> list)
    {
        try
        {
            var filePath = Path.Combine(DesktopPath, fileName);
            var lines = list.Select(weather => weather.ToString()).ToList();
            File.WriteAllLines(filePath, lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while creating file: {ex.Message}");
        }
    }
}