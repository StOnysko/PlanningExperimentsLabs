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

        // Sequential calculation
        Console.WriteLine("\n1. Sequential algorithm");
        var sequentialPi = MeasureExecutionTime(() => CalculatePiSequential(sampleSize), out var sequentialTime);
        Console.WriteLine($"   Estimated Pi: {sequentialPi:F10}");
        Console.WriteLine($"   Error: {Math.Abs(sequentialPi - Math.PI):F10}");
        Console.WriteLine($"   Execution time: {sequentialTime:F3} ms");

        // Parallel using Task
        Console.WriteLine("\n2. Parallel algorithm (Task)");
        var parallelTaskPi = MeasureExecutionTime(() => CalculatePiParallelTask(sampleSize), out var parallelTaskTime);
        Console.WriteLine($"   Estimated Pi: {parallelTaskPi:F10}");
        Console.WriteLine($"   Error: {Math.Abs(parallelTaskPi - Math.PI):F10}");
        Console.WriteLine($"   Execution time: {parallelTaskTime:F3} ms");
        Console.WriteLine($"   Speedup: {sequentialTime / parallelTaskTime:F2}x");

        // Parallel using Parallel.For
        Console.WriteLine("\n3. Parallel algorithm (Parallel.For)");
        var parallelForPi = MeasureExecutionTime(() => CalculatePiParallelFor(sampleSize), out var parallelForTime);
        Console.WriteLine($"   Estimated Pi: {parallelForPi:F10}");
        Console.WriteLine($"   Error: {Math.Abs(parallelForPi - Math.PI):F10}");
        Console.WriteLine($"   Execution time: {parallelForTime:F3} ms");
        Console.WriteLine($"   Speedup: {sequentialTime / parallelForTime:F2}x");

        // Parallel using Thread
        Console.WriteLine("\n4. Parallel algorithm (Thread)");
        var parallelThreadPi =
            MeasureExecutionTime(() => CalculatePiParallelThread(sampleSize), out double parallelThreadTime);
        Console.WriteLine($"   Estimated Pi: {parallelThreadPi:F10}");
        Console.WriteLine($"   Error: {Math.Abs(parallelThreadPi - Math.PI):F10}");
        Console.WriteLine($"   Execution time: {parallelThreadTime:F3} ms");
        Console.WriteLine($"   Speedup: {sequentialTime / parallelThreadTime:F2}x");
    }

    // Допоміжний метод для вимірювання часу виконання
    private static T MeasureExecutionTime<T>(Func<T> function, out double elapsedMs)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = function();
        stopwatch.Stop();
        elapsedMs = stopwatch.Elapsed.TotalMilliseconds;
        return result;
    }

    // Послідовне обчислення π
    private static double CalculatePiSequential(int sampleSize)
    {
        var random = new Random();
        var pointsInsideCircle = 0;

        for (var i = 0; i < sampleSize; i++)
        {
            var x = random.NextDouble();
            var y = random.NextDouble();

            // Перевірка, чи точка знаходиться всередині чверті кола
            if (x * x + y * y <= 1)
            {
                pointsInsideCircle++;
            }
        }

        // Обчислення π як відношення точок в колі до загальної кількості
        return 4.0 * pointsInsideCircle / sampleSize;
    }

    // Паралельне обчислення π з використанням Task
    private static double CalculatePiParallelTask(int sampleSize)
    {
        var processorCount = Environment.ProcessorCount;
        var pointsPerTask = sampleSize / processorCount;

        var tasks = new Task<int>[processorCount];

        // Створення та запуск задач
        for (var i = 0; i < processorCount; i++)
        {
            var taskIndex = i; // Локальна копія для замикання
            tasks[i] = Task.Run(() =>
            {
                // Кожен потік повинен мати власний генератор випадкових чисел
                var localRandom = new Random(Guid.NewGuid().GetHashCode());
                var localPointsInside = 0;

                // В останньому потоці обробляємо залишок
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

        // Очікування завершення всіх задач і сумування результатів
        Task.WaitAll(tasks);

        var totalPointsInside = tasks.Sum(task => task.Result);

        // Обчислення π
        return 4.0 * totalPointsInside / sampleSize;
    }

    // Паралельне обчислення π з використанням Parallel.For
    private static double CalculatePiParallelFor(int sampleSize)
    {
        // Використання ConcurrentBag для безпечного збору результатів 
        // з різних потоків без необхідності ручного блокування
        var insidePoints = new ConcurrentBag<int>();

        // Розбиваємо обчислення на блоки для підвищення ефективності
        const int blockSize = 10000; // Можна експериментувати з цим значенням
        var blocks = sampleSize / blockSize;

        // Parallel.For автоматично розподіляє роботу між доступними потоками
        Parallel.For(0, blocks, i =>
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

            // Додавання результату блоку до загального
            insidePoints.Add(localPointsInside);
        });

        // Обробка залишкових точок (якщо sampleSize не кратне blockSize)
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

        // Підрахунок загальної кількості точок всередині кола
        var pointsInsideCircle = insidePoints.Sum();

        // Обчислення π
        return 4.0 * pointsInsideCircle / sampleSize;
    }

    // Паралельне обчислення π з використанням Thread
    private static double CalculatePiParallelThread(int sampleSize)
    {
        var processorCount = Environment.ProcessorCount;
        var pointsPerThread = sampleSize / processorCount;

        // Масив для зберігання результатів з кожного потоку
        var pointsInsideCircle = new int[processorCount];
        var threads = new Thread[processorCount];

        // Створення та запуск потоків
        for (var i = 0; i < processorCount; i++)
        {
            var threadIndex = i; // Локальна копія для замикання

            threads[i] = new Thread(() =>
            {
                // Кожен потік повинен мати власний генератор випадкових чисел
                var localRandom = new Random(Guid.NewGuid().GetHashCode());
                var localPointsInside = 0;

                // В останньому потоці обробляємо залишок
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

                // Зберігаємо результат потоку
                pointsInsideCircle[threadIndex] = localPointsInside;
            });

            threads[i].Start();
        }

        // Очікування завершення всіх потоків
        foreach (var thread in threads)
        {
            thread.Join();
        }

        // Підрахунок загальної кількості точок всередині кола
        var totalPointsInside = pointsInsideCircle.Sum();

        // Обчислення π
        return 4.0 * totalPointsInside / sampleSize;
    }
}