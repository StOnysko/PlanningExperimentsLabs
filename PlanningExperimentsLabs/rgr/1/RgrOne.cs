namespace PlanningExperimentsLabs.rgr._1;

using System;
using System.Diagnostics;

public static class RgrOne
    {
        public static void Start()
        {
            const int recordCount = 100000; // Можна змінювати кількість записів
            const string sortBy = "genre";   // "genre", "artist", або "title"

            Console.WriteLine($"Створення {recordCount} музичних записів...");
            var musicList = MusicGenerator.Generate(recordCount);

            // Послідовне сортування
            Console.WriteLine($"\n🔹 Послідовне сортування за {sortBy}...");
            var stopwatch = Stopwatch.StartNew();
            var sequentialSorted = SequentialSorter.Sort(musicList, sortBy);
            stopwatch.Stop();
            Console.WriteLine($"Час: {stopwatch.ElapsedMilliseconds} мс");

            // Паралельне сортування
            Console.WriteLine($"\n🔹 Паралельне сортування за {sortBy}...");
            stopwatch.Restart();
            var parallelSorted = ParallelSorter.Sort(musicList, sortBy);
            stopwatch.Stop();
            Console.WriteLine($"Час: {stopwatch.ElapsedMilliseconds} мс");

            Console.WriteLine("\n✅ Завершено.");
        }
    }

