using System.Diagnostics;
using PlanningExperimentsLabs.Laboratories.Data.Local;
using PlanningExperimentsLabs.Laboratories.Data.Weather;

namespace PlanningExperimentsLabs.Laboratories._2;

public static class SecondLaboratory
{
    private const int InstanceCount = 1500000;
    
    private static void SortWithTask()
    {
        var weatherList = WeatherRepository.GenerateWeatherCollection(1000);
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var task = new Task(() =>
        {
            var sortedWeatherList = WeatherRepository.SortWeatherCollection(weatherList);
            stopwatch.Stop();

            FileManager.CreateFile(FileManager.LaboratoryOneThreadUsage, sortedWeatherList);
            Console.WriteLine($"[Task] Sorting completed in {stopwatch.ElapsedMilliseconds} ms");
        });

        task.Start();
        task.Wait();
    }

    private static void SortWithTaskFactory()
    {
        var weatherList = WeatherRepository.GenerateWeatherCollection(1000);
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var task = Task.Factory.StartNew(() =>
        {
            var sortedWeatherList = WeatherRepository.SortWeatherCollection(weatherList);
            stopwatch.Stop();

            FileManager.CreateFile(FileManager.LaboratoryOneThreadUsage, sortedWeatherList);
            Console.WriteLine($"[Task.Factory] Sorting completed in {stopwatch.ElapsedMilliseconds} ms");
        });

        task.Wait();
    }

    private static void SortWithTaskResult(int instanceCount)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var task = new Task<List<Weather>>(() =>
            WeatherRepository.SortWeatherCollection(WeatherRepository.GenerateWeatherCollection(instanceCount)));
        task.Start();
        var sortedWeatherList = task.Result;

        stopwatch.Stop();
        FileManager.CreateFile(FileManager.LaboratoryOneThreadUsage, sortedWeatherList);
        Console.WriteLine($"[Task<T>.Result] Sorting completed in {stopwatch.ElapsedMilliseconds} ms");
    }

    private static void SynchronizationExamples()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var task1 = Task.Run(() =>
        {
            Thread.Sleep(1000);
            Console.WriteLine("[Task 1] Completed");
        });

        var task2 = Task.Run(() =>
        {
            Thread.SpinWait(1000000);
            Console.WriteLine("[Task 2] Completed");
        });

        Task.WaitAll(task1, task2);
        stopwatch.Stop();
        Console.WriteLine($"[Sync] Both tasks completed in {stopwatch.ElapsedMilliseconds} ms");

        stopwatch.Restart();
        var task3 = Task.Run(() =>
        {
            Thread.Sleep(500);
            Console.WriteLine("[Task 3] Completed");
        });

        var task4 = Task.Run(() =>
        {
            Thread.Sleep(1500);
            Console.WriteLine("[Task 4] Completed");
        });

        Task.WaitAny(task3, task4);
        stopwatch.Stop();
        Console.WriteLine($"[Sync] At least one task completed in {stopwatch.ElapsedMilliseconds} ms");
    }
    
    public static void Start()
    {
        Console.WriteLine("=== Sorting Performance Tests ===");
        SortWithTask();
        SortWithTaskFactory();
        SortWithTaskResult(InstanceCount);
        Console.WriteLine("\n=== Synchronization Examples ===");
        SynchronizationExamples();
    }
}