namespace PlanningExperimentsLabs.rgr._1;

public static class SequentialSorter
{
    public static List<Music> Sort(List<Music> list, string sortBy)
    {
        return sortBy switch
        {
            "genre" => list.OrderBy(m => m.Genre).ToList(),
            "artist" => list.OrderBy(m => m.Artist).ToList(),
            "title" => list.OrderBy(m => m.Title).ToList(),
            _ => list
        };
    }
}