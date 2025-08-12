using System.Text.Json.Serialization;

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

    internal class WeatherData
    {
        [JsonPropertyName("daily")]
        public Daily? DailyTemp { get; set; }

        [JsonPropertyName("hourly")]
        public Hourly? HourlyTemp { get; set; }
    }
    internal class Daily
    {
        [JsonPropertyName("temperature_2m_mean")]
        public List<double>? TemperatureMean { get; set; }

        [JsonPropertyName("precipitation_probability_mean")]
        public List<double>? PrecipitationMean { get; set; }

        [JsonPropertyName("wind_speed_10m_mean")]
        public List<double>? WindSpeedMean { get; set; }
    }
    internal class Hourly
    {
        public List<string>? Time { get; set; }

        [JsonPropertyName("temperature_2m")]
        public List<double>? Temperature { get; set; }
    }

    public class HourlyTemperatureData
    {
        public DateTime? Time { get; set; }
        public double Temperature { get; set; }
    }
}
