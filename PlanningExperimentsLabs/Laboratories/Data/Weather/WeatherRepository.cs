namespace PlanningExperimentsLabs.Laboratories.Data.Weather;

public static class WeatherRepository
{
    private static Laboratories.Data.Weather.Weather GenerateInstance()
    {
        var rand = new Random();
        var date = DateOnly.FromDateTime(DateTime.Now.AddDays(rand.Next(-30, 30)));
        var time = new TimeOnly(rand.Next(0, 24), rand.Next(0, 60));
        var temperature = Math.Round(rand.NextDouble() * 40 - 10, 1);
        var precipitation = Math.Round(rand.NextDouble() * 10, 1);
        var wind = Math.Round(rand.NextDouble() * 20, 1);
        string[] descriptions = ["Sunny", "Cloudy", "Rainy", "Stormy", "Snowy", "Foggy"];
        var description = descriptions[rand.Next(descriptions.Length)];

        return new Weather(date, time, temperature, precipitation, wind, description);
    }

    public static List<Weather> GenerateWeatherCollection(int count)
    {
        var weatherList = new List<Weather>();
        for (var i = 0; i < count; i++)
        {
            weatherList.Add(GenerateInstance());
        }

        return weatherList;
    }

    public static List<Weather> SortWeatherCollection(List<Weather> weatherList)
    {
        return weatherList
            .OrderBy(w => w.Date)
            .ThenBy(w => w.Time)
            .ThenBy(w => w.Temperature)
            .ToList();
    }
}