using System.Diagnostics;
using PlanningExperimentsLabs.Laboratories.Data.Local;
using PlanningExperimentsLabs.Laboratories.Data.Weather;

namespace PlanningExperimentsLabs.Laboratories._3;

public static class ThirdLaboratory
{
    private static int _counter;
    private static readonly object Locker = new object();
    private static readonly Mutex Mutex = new Mutex();

    public static void SortWithLock(int instanceCount)
    {
        var weatherList = WeatherRepository.GenerateWeatherCollection(instanceCount);
        var sortedWeatherList = WeatherRepository.SortWeatherCollection(weatherList);

        lock (Locker)
        {
            _counter++;
            FileManager.CreateFile("1", sortedWeatherList);
        }
    }

    public static void SortWithMonitor(int instanceCount)
    {
        var weatherList = WeatherRepository.GenerateWeatherCollection(instanceCount);
        var sortedWeatherList = WeatherRepository.SortWeatherCollection(weatherList);

        Monitor.Enter(Locker);
        try
        {
            _counter++;
            FileManager.CreateFile("2", sortedWeatherList);
        }
        finally
        {
            Monitor.Exit(Locker);
        }
    }

    public static void SortWithMutex(int instanceCount)
    {
        var weatherList = WeatherRepository.GenerateWeatherCollection(instanceCount);
        var sortedWeatherList = WeatherRepository.SortWeatherCollection(weatherList);

        Mutex.WaitOne();
        try
        {
            _counter++;
            FileManager.CreateFile("3", sortedWeatherList);
        }
        finally
        {
            Mutex.ReleaseMutex();
        }
    }

    public static void SortWithInterlocked(int instanceCount)
    {
        var weatherList = WeatherRepository.GenerateWeatherCollection(instanceCount);
        var sortedWeatherList = WeatherRepository.SortWeatherCollection(weatherList);

        Interlocked.Increment(ref _counter);
        FileManager.CreateFile("4", sortedWeatherList);
    }

    public static void Start(int instanceCount)
    {
        Console.WriteLine("=== Running Sorting Methods with Critical Sections ===");

        var sw = new Stopwatch();

        sw.Start();
        SortWithLock(instanceCount);
        sw.Stop();
        Console.WriteLine($"[Lock] Sorting took: {sw.ElapsedMilliseconds} ms");

        sw.Restart();
        SortWithMonitor(instanceCount);
        sw.Stop();
        Console.WriteLine($"[Monitor] Sorting took: {sw.ElapsedMilliseconds} ms");

        sw.Restart();
        SortWithMutex(instanceCount);
        sw.Stop();
        Console.WriteLine($"[Mutex] Sorting took: {sw.ElapsedMilliseconds} ms");

        sw.Restart();
        SortWithInterlocked(instanceCount);
        sw.Stop();
        Console.WriteLine($"[Interlocked] Sorting took: {sw.ElapsedMilliseconds} ms");
    }
}