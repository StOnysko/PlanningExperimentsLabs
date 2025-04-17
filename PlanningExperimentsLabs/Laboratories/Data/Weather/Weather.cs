namespace PlanningExperimentsLabs.Laboratories.Data.Weather;

public class Weather(
    DateOnly date,
    TimeOnly time,
    double temperature,
    double precipitation,
    double wind,
    string description)
{
    public DateOnly Date { get; set; } = date;
    public TimeOnly Time { get; set; } = time;
    public double Temperature { get; set; } = temperature;
    private double Precipitation { get; set; } = precipitation;
    private double Wind { get; set; } = wind;
    private string Description { get; set; } = description;

    public override string ToString()
    {
        return
            $"Date: {Date.ToShortDateString()}, Time: {Time}, Temperature: {Temperature}°C, Precipitation: {Precipitation}mm, Wind: {Wind} km/h, Description: {Description}";
    }
}