namespace WeatherDashBoardDemo
{
    public class Model
    {
        public string CountryName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Model(string countryName, double latitude, double longitude)
        {
            CountryName = countryName;
            Latitude = latitude;
            Longitude = longitude;
        }
    }

    public class WeatherData
    {
        public Daily? DailyTemp { get; set; }
        public Hourly? HourlyTemp { get; set; }
    }
    public class Daily
    {
        public List<string>? Time { get; set; }
        public List<double>? Temperature_2m_mean { get; set; }
        public List<double>? Precipitation_probability_mean { get; set; }
        public List<double>? Wind_speed_10m_mean { get; set; }
    }
    public class Hourly
    {
        public List<string>? Time { get; set; }
        public List<double>? Temperature_2m { get; set; }
    }

    public class HourlyTemperatureData
    {
        public DateTime? Time { get; set; }
        public double Temperature { get; set; }
    }
}
