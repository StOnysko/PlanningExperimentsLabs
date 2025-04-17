using PlanningExperimentsLabs.rgr._1.data;
using PlanningExperimentsLabs.rgr._1.sorter;

namespace PlanningExperimentsLabs.rgr._1;

using System;
using System.Diagnostics;

public static class RgrOne
{
    public static void Start()
    {
        const int recordCount = 20000000; 
        const string sortBy = "genre"; 

        Console.WriteLine($"Generating {recordCount} music records...");
        var musicList = MusicGenerator.Generate(recordCount);

        // Sequential sorting
        Console.WriteLine($"\nSequential sorting by {sortBy}...");
        var stopwatch = Stopwatch.StartNew();
        var sequentialSorted = SequentialSorter.Sort(musicList, sortBy);
        stopwatch.Stop();
        Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds} ms");

        // Parallel sorting
        Console.WriteLine($"\nParallel sorting by {sortBy}...");
        stopwatch.Restart();
        var parallelSorted = ParallelSorter.Sort(musicList, sortBy);
        stopwatch.Stop();
        Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds} ms");

        Console.WriteLine("\nCompleted.");
    }
}