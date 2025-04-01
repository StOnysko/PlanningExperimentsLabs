using System.Diagnostics;
using static PlanningExperimentsLabs.Laboratories._3.ThirdLaboratory;

namespace PlanningExperimentsLabs.Laboratories._4;

public static class FourthLaboratory
{
    private static void RunWithContinueWhenAll(int instanceCount)
    {
        var sw = new Stopwatch();
        sw.Start();

        var tasks = new[]
        {
            Task.Run(() => SortWithLock(instanceCount)),
            Task.Run(() => SortWithMonitor(instanceCount)),
            Task.Run(() => SortWithMutex(instanceCount)),
            Task.Run(() => SortWithInterlocked(instanceCount))
        };

        Task.Factory.ContinueWhenAll(tasks, _ =>
        {
            sw.Stop();
            Console.WriteLine($"[ContinueWhenAll] All tasks completed in {sw.ElapsedMilliseconds} ms");
        }).Wait();
    }

    private static void RunWithContinueWhenAny(int instanceCount)
    {
        var sw = new Stopwatch();
        sw.Start();

        var tasks = new[]
        {
            Task.Run(() => SortWithLock(instanceCount)),
            Task.Run(() => SortWithMonitor(instanceCount)),
            Task.Run(() => SortWithMutex(instanceCount)),
            Task.Run(() => SortWithInterlocked(instanceCount))
        };

        Task.Factory.ContinueWhenAny(tasks, _ =>
        {
            sw.Stop();
            Console.WriteLine($"[ContinueWhenAny] At least one task completed in {sw.ElapsedMilliseconds} ms");
        }).Wait();
    }

    private static void RunWithNestedTasks(int instanceCount)
    {
        var sw = new Stopwatch();
        sw.Start();

        var parentTask = Task.Factory.StartNew(() =>
        {
            Task.Factory.StartNew(() => SortWithLock(instanceCount), TaskCreationOptions.AttachedToParent);
            Task.Factory.StartNew(() => SortWithMonitor(instanceCount), TaskCreationOptions.AttachedToParent);
            Task.Factory.StartNew(() => SortWithMutex(instanceCount), TaskCreationOptions.AttachedToParent);
            Task.Factory.StartNew(() => SortWithInterlocked(instanceCount), TaskCreationOptions.AttachedToParent);
        });

        parentTask.Wait();
        sw.Stop();
        Console.WriteLine($"[NestedTasks] Execution completed in {sw.ElapsedMilliseconds} ms");
    }


    public static void Start(int instanceCount)
    {
        RunWithContinueWhenAll(instanceCount);
        RunWithContinueWhenAny(instanceCount);
        RunWithNestedTasks(instanceCount);
    }
}