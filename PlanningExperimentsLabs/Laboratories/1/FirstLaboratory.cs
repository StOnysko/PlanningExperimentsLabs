using System.Diagnostics;
using PlanningExperimentsLabs.Data.Local;
using PlanningExperimentsLabs.Data.Weather;

namespace PlanningExperimentsLabs.Laboratories._1;

public static class FirstLaboratory
{
    private const int InstancesCount = 200000;

    private static void SortWithoutThreadClass(int instanceCount)
    {
        var sw = new Stopwatch();
        sw.Start();

        var weatherList = WeatherRepository.GenerateWeatherCollection(instanceCount);
        var sortedWeatherList = WeatherRepository.SortWeatherCollection(weatherList);
        FileManager.CreateFile(FileManager.LaboratoryOneSorted, sortedWeatherList);

        sw.Stop();

        Console.WriteLine($"Sorting took: {sw.ElapsedMilliseconds} ms");
    }

    private static void SortWithThreadClass(int instanceCount)
    {
        var weatherList = WeatherRepository.GenerateWeatherCollection(instanceCount);
        var stopwatch = new Stopwatch();

        var sortingThread = new Thread(() =>
        {
            stopwatch.Start();
            var sortedWeatherList = WeatherRepository.SortWeatherCollection(weatherList);
            stopwatch.Stop();

            FileManager.CreateFile(FileManager.LaboratoryOneThreadUsage, sortedWeatherList);
            Console.WriteLine($"Sorting with another thread usage took: {stopwatch.ElapsedMilliseconds} ms");
        });

        sortingThread.Start();
        sortingThread.Join();
    }

    public static void Start()
    {
        Console.WriteLine($"Instances to sort: {InstancesCount}");
        SortWithoutThreadClass(InstancesCount);
        SortWithThreadClass(InstancesCount);
    }
}