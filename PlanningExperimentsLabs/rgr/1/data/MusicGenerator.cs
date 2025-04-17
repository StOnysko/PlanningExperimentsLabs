namespace PlanningExperimentsLabs.rgr._1.data;

public static class MusicGenerator
{
    public static List<Music>? Generate(int count)
    {
        var random = new Random();
        var genres = new[] { "Rock", "Pop", "Jazz", "Classical", "Hip-Hop" };
        var artists = new[] { "Artist A", "Artist B", "Artist C", "Artist D" };

        var list = new List<Music>(count);
        for (var i = 0; i < count; i++)
        {
            list.Add(new Music
            {
                Genre = genres[random.Next(genres.Length)],
                Artist = artists[random.Next(artists.Length)],
                Title = $"Song {i}",
                Description = "Some description...",
                Album = $"Album {random.Next(1, 20)}"
            });
        }

        return list;
    }
}