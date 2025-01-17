namespace IntervalCalls.Server
{
    public class WeatherForecast
    {
        public DateTime date { get; set; }

        public int temperatureC { get; set; }

        public int temperatureF => 32 + (int)(temperatureC / 0.5556);

        public string? summary { get; set; }

        public override string ToString()
        {
            return $"Date: {date}, Temperature in Celsius: {temperatureC}, Temperature in Fahrenheit: {temperatureF}, Summary: {summary}";
        }
    }
}
