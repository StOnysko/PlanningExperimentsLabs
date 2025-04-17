using PlanningExperimentsLabs.rgr._1.data;

namespace PlanningExperimentsLabs.rgr._1.sorter;

public static class ParallelSorter
{
    public static List<Music>? Sort(List<Music>? list, string sortBy)
    {
        if (list == null || list.Count == 0)
            return [];

        var parallelQuery = list.AsParallel().WithExecutionMode(ParallelExecutionMode.ForceParallelism);

        return sortBy switch
        {
            "genre" => parallelQuery.OrderBy(m => m.Genre).ToList(),
            "artist" => parallelQuery.OrderBy(m => m.Artist).ToList(),
            "title" => parallelQuery.OrderBy(m => m.Title).ToList(),
            _ => list
        };
    }
}
