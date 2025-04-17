namespace PlanningExperimentsLabs.rgr._2;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;

public static class RgrTwo
{
    public static void Start()
    {
        const int sampleSize = 100000000;

        Console.WriteLine($"\nSample size: {sampleSize:N0}");

        Console.WriteLine("\n1. Sequential algorithm");
        var sequentialPi = MeasureExecutionTime(() => CalculatePiSequential(sampleSize), out var sequentialTime);
        Console.WriteLine($"   Estimated Pi: {sequentialPi:F10}");
        Console.WriteLine($"   Error: {Math.Abs(sequentialPi - Math.PI):F10}");
        Console.WriteLine($"   Execution time: {sequentialTime:F3} ms");

        Console.WriteLine("\n2. Parallel algorithm (Task)");
        var parallelTaskPi = MeasureExecutionTime(() => CalculatePiParallelTask(sampleSize), out var parallelTaskTime);
        Console.WriteLine($"   Estimated Pi: {parallelTaskPi:F10}");
        Console.WriteLine($"   Error: {Math.Abs(parallelTaskPi - Math.PI):F10}");
        Console.WriteLine($"   Execution time: {parallelTaskTime:F3} ms");
        Console.WriteLine($"   Speedup: {sequentialTime / parallelTaskTime:F2}x");

        Console.WriteLine("\n3. Parallel algorithm (Parallel.For)");
        var parallelForPi = MeasureExecutionTime(() => CalculatePiParallelFor(sampleSize), out var parallelForTime);
        Console.WriteLine($"   Estimated Pi: {parallelForPi:F10}");
        Console.WriteLine($"   Error: {Math.Abs(parallelForPi - Math.PI):F10}");
        Console.WriteLine($"   Execution time: {parallelForTime:F3} ms");
        Console.WriteLine($"   Speedup: {sequentialTime / parallelForTime:F2}x");

        Console.WriteLine("\n4. Parallel algorithm (Thread)");
        var parallelThreadPi =
            MeasureExecutionTime(() => CalculatePiParallelThread(sampleSize), out double parallelThreadTime);
        Console.WriteLine($"   Estimated Pi: {parallelThreadPi:F10}");
        Console.WriteLine($"   Error: {Math.Abs(parallelThreadPi - Math.PI):F10}");
        Console.WriteLine($"   Execution time: {parallelThreadTime:F3} ms");
        Console.WriteLine($"   Speedup: {sequentialTime / parallelThreadTime:F2}x");
    }

    private static T MeasureExecutionTime<T>(Func<T> function, out double elapsedMs)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = function();
        stopwatch.Stop();
        elapsedMs = stopwatch.Elapsed.TotalMilliseconds;
        return result;
    }

    private static double CalculatePiSequential(int sampleSize)
    {
        var random = new Random();
        var pointsInsideCircle = 0;

        for (var i = 0; i < sampleSize; i++)
        {
            var x = random.NextDouble();
            var y = random.NextDouble();

            if (x * x + y * y <= 1)
            {
                pointsInsideCircle++;
            }
        }

        return 4.0 * pointsInsideCircle / sampleSize;
    }

    private static double CalculatePiParallelTask(int sampleSize)
    {
        var processorCount = Environment.ProcessorCount;
        var pointsPerTask = sampleSize / processorCount;

        var tasks = new Task<int>[processorCount];

        for (var i = 0; i < processorCount; i++)
        {
            var taskIndex = i; // Локальна копія для замикання
            tasks[i] = Task.Run(() =>
            {
                var localRandom = new Random(Guid.NewGuid().GetHashCode());
                var localPointsInside = 0;

                var localPointsCount = (taskIndex < processorCount - 1)
                    ? pointsPerTask
                    : pointsPerTask + sampleSize % processorCount;

                for (var j = 0; j < localPointsCount; j++)
                {
                    var x = localRandom.NextDouble();
                    var y = localRandom.NextDouble();

                    if (x * x + y * y <= 1)
                    {
                        localPointsInside++;
                    }
                }

                return localPointsInside;
            });
        }

        Task.WaitAll(tasks);

        var totalPointsInside = tasks.Sum(task => task.Result);

        // Обчислення π
        return 4.0 * totalPointsInside / sampleSize;
    }

    private static double CalculatePiParallelFor(int sampleSize)
    {
        var insidePoints = new ConcurrentBag<int>();

        const int blockSize = 10000; // Можна експериментувати з цим значенням
        var blocks = sampleSize / blockSize;

        Parallel.For(0, blocks, _ =>
        {
            var localRandom = new Random(Guid.NewGuid().GetHashCode());
            var localPointsInside = 0;

            // Обчислення в межах одного блоку
            for (var j = 0; j < blockSize; j++)
            {
                var x = localRandom.NextDouble();
                var y = localRandom.NextDouble();

                if (x * x + y * y <= 1)
                {
                    localPointsInside++;
                }
            }

            insidePoints.Add(localPointsInside);
        });

        var remainingPoints = sampleSize % blockSize;
        if (remainingPoints > 0)
        {
            var localRandom = new Random(Guid.NewGuid().GetHashCode());
            var localPointsInside = 0;

            for (var j = 0; j < remainingPoints; j++)
            {
                var x = localRandom.NextDouble();
                var y = localRandom.NextDouble();

                if (x * x + y * y <= 1)
                {
                    localPointsInside++;
                }
            }

            insidePoints.Add(localPointsInside);
        }

        var pointsInsideCircle = insidePoints.Sum();

        return 4.0 * pointsInsideCircle / sampleSize;
    }

    private static double CalculatePiParallelThread(int sampleSize)
    {
        var processorCount = Environment.ProcessorCount;
        var pointsPerThread = sampleSize / processorCount;

        var pointsInsideCircle = new int[processorCount];
        var threads = new Thread[processorCount];

        for (var i = 0; i < processorCount; i++)
        {
            var threadIndex = i;

            threads[i] = new Thread(() =>
            {
                var localRandom = new Random(Guid.NewGuid().GetHashCode());
                var localPointsInside = 0;

                var localPointsCount = (threadIndex < processorCount - 1)
                    ? pointsPerThread
                    : pointsPerThread + sampleSize % processorCount;

                for (var j = 0; j < localPointsCount; j++)
                {
                    var x = localRandom.NextDouble();
                    var y = localRandom.NextDouble();

                    if (x * x + y * y <= 1)
                    {
                        localPointsInside++;
                    }
                }

                pointsInsideCircle[threadIndex] = localPointsInside;
            });

            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        var totalPointsInside = pointsInsideCircle.Sum();

        return 4.0 * totalPointsInside / sampleSize;
    }
}