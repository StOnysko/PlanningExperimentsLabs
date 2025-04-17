namespace PlanningExperimentsLabs.rgr._1;

public static class ParallelSorter
{
    public static List<Music> Sort(List<Music> list, string sortBy)
    {
        return sortBy switch
        {
            "genre" => list.AsParallel().OrderBy(m => m.Genre).ToList(),
            "artist" => list.AsParallel().OrderBy(m => m.Artist).ToList(),
            "title" => list.AsParallel().OrderBy(m => m.Title).ToList(),
            _ => list
        };
    }
}