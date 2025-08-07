using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.Json;


namespace WeatherDashBoardDemo
{
    public class WeatherViewModel : INotifyPropertyChanged
    {
        #region Private Fields

        private Model selectedCountry;
        private double dailyTemperature;
        private double dailyWindSpeed;
        private double dailyPrecipitation;

        #endregion

        #region Public Properties

        public ObservableCollection<Model> CountriesDetail { get; set; }
        public Model SelectedCountry
        {
            get => selectedCountry;
            set
            {
                if (selectedCountry != value)
                {
                    selectedCountry = value;
                    OnPropertyChanged();
                    FetchWeatherData(selectedCountry);
                }
            }
        }
        public double DailyTemperature
        {
            get => dailyTemperature;
            set { dailyTemperature = value; OnPropertyChanged(); }
        }
        public double DailyWindSpeed
        {
            get => dailyWindSpeed;
            set { dailyWindSpeed = value; OnPropertyChanged(); }
        }
        public double DailyPrecipitation
        {
            get => dailyPrecipitation;
            set { dailyPrecipitation = value; OnPropertyChanged(); }
        }
        public ObservableCollection<HourlyTemperatureData> HourlyTemperatures { get; set; } = new();
        public ObservableCollection<HourlyTemperatureData> CurrentTemperatures { get; set; } = new();
        public ObservableCollection<HourlyTemperatureData> ForecastTemperatures { get; set; } = new();

        #endregion

        public WeatherViewModel()
        {
            CountriesDetail = new ObservableCollection<Model>();
            LoadCountriesFromCSVFile();
        }

        private void LoadCountriesFromCSVFile()
        {
            try
            {
                Assembly executingAssembly = typeof(WeatherViewModel).GetTypeInfo().Assembly;
                var resourceName = "WeatherDashBoardDemo.Resources.Data.countriesdetail.csv";
                using Stream? inputStream = executingAssembly?.GetManifestResourceStream(resourceName);
                if (inputStream == null)
                {
                    Debug.WriteLine("Could not find countries CSV resource.");
                    return;
                }
                using StreamReader reader = new(inputStream);
                // Remove header
                string? header = reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    var data = line.Split(',');
                    // Format: Country,Lattitude,Longitude
                    if (data.Length >= 3 &&
                        double.TryParse(data[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double lat) &&
                        double.TryParse(data[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double lon))
                    {
                        CountriesDetail.Add(new Model(data[0], lat, lon));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading CSV: {ex.Message}");
            }
        }

        /// <summary>
        /// Fetches weather data from Open-Meteo API for the specified country
        /// </summary>
        /// <param name="country"></param>
        private async void FetchWeatherData(Model country)
        {
            
            if (country == null)
                return;

            string url = $"https://api.open-meteo.com/v1/forecast" +
                $"?latitude={country.Lattitude.ToString(CultureInfo.InvariantCulture)}" +
                $"&longitude={country.Longtitude.ToString(CultureInfo.InvariantCulture)}" +
                "&daily=temperature_2m_mean,precipitation_probability_mean,wind_speed_10m_mean" +
                "&hourly=temperature_2m" +
                "&timezone=auto&forecast_days=7";

            try
            {
                using HttpClient client = new();
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var data = JsonSerializer.Deserialize<WeatherData>(json, options);
                    if (data != null && data.daily != null && data.hourly != null)
                    {
                        //Current Weather metrics
                        DailyTemperature = data.daily.temperature_2m_mean?.Count > 0 ? data.daily.temperature_2m_mean[0] : 0;
                        DailyWindSpeed = data.daily.wind_speed_10m_mean?.Count > 0 ? data.daily.wind_speed_10m_mean[0] : 0;
                        DailyPrecipitation = data.daily.precipitation_probability_mean?.Count > 0 ? data.daily.precipitation_probability_mean[0] : 0;

                        // Temperature Forcast
                        HourlyTemperatures.Clear();
                        if (data.hourly?.temperature_2m != null && data.hourly?.time != null)
                        {
                            DateTime now = DateTime.Now;
                            int max = Math.Min(168, data.hourly.temperature_2m.Count);

                            for (int i = 0; i < max; i++)
                            {
                                if (DateTime.TryParse(data.hourly.time[i], out DateTime hTime))
                                {
                                    HourlyTemperatures.Add(new HourlyTemperatureData
                                    {
                                        Time = hTime,
                                        Temperature = data.hourly.temperature_2m[i]
                                    });
                                }
                            }
                            OnPropertyChanged(nameof(HourlyTemperatures));
                        }
                    }
                    SplitHourlyDataForTodayAndForecast();
                    OnPropertyChanged(nameof(CurrentTemperatures));
                    OnPropertyChanged(nameof(ForecastTemperatures));
                }
                else
                {
                    Debug.WriteLine("API call failed with status " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching weather data: {ex.Message}");
            }
        }

        private void SplitHourlyDataForTodayAndForecast()
        {
            CurrentTemperatures.Clear();
            ForecastTemperatures.Clear();

            var now = DateTime.Now;
            var today = now.Date;
            var tomorrow = today.AddDays(1);
            bool addedMidnightForecast = false;

            foreach (var item in HourlyTemperatures)
            {
                // Today's bucket
                if (item.Time < tomorrow)
                {
                    CurrentTemperatures.Add(item);
                }
                // include exactly midnight of "tomorrow"
                else if (item.Time == tomorrow)
                {
                    CurrentTemperatures.Add(item);
                    ForecastTemperatures.Add(item);
                    addedMidnightForecast = true;
                }
                // Forecast bucket
                else if (item.Time > tomorrow || (item.Time == tomorrow && !addedMidnightForecast))
                {
                    ForecastTemperatures.Add(item);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
       
