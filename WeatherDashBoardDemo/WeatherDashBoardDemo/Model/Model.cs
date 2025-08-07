namespace WeatherDashBoardDemo
{
    public class Model
    {
        public string CountryName { get; set; }
        public double Lattitude { get; set; }
        public double Longtitude { get; set; }
        public Model(string countryName, double lattitude, double longtitude)
        {
            CountryName = countryName;
            Lattitude = lattitude;
            Longtitude = longtitude;
        }
    }

    public class WeatherData
    {
        public Daily daily { get; set; }
        public Hourly hourly { get; set; }
    }
    public class Daily
    {
        public List<string> time { get; set; }
        public List<double> temperature_2m_mean { get; set; }
        public List<double> precipitation_probability_mean { get; set; }
        public List<double> wind_speed_10m_mean { get; set; }
    }
    public class Hourly
    {
        public List<string> time { get; set; }
        public List<double> temperature_2m { get; set; }
    }

    public class HourlyTemperatureData
    {
        public DateTime Time { get; set; }
        public double Temperature { get; set; }
    }
}
