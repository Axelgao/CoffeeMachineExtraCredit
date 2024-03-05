using CoffeeMachine.Controllers;
using CoffeeMachine.Interfaces;
using CoffeeMachine.Models;
using Newtonsoft.Json;

namespace CoffeeMachine.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly ILogger<WeatherService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _baseAddress;
        private readonly string _apiKey;
        private readonly string _lat;
        private readonly string _lon;

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _baseAddress = configuration["WeatherService:BaseAddress"] ?? "https://api.openweathermap.org";
            _apiKey = configuration["WeatherService:ApiKey"] ?? "default_api_key";
            _lat = configuration["WeatherService:Lat"] ?? "-37.7833";
            _lon = configuration["WeatherService:Lon"] ?? "175.2833";

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_baseAddress);
        }

        public async Task<double> GetTempAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("data/2.5/weather?lat={_lat}&lon={_lon}&appid={_apiKey}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var weather = JsonConvert.DeserializeObject<Weather>(content);

                return weather.WeatherMain.Temp;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("The weather service is currently unavailable. Default settings have been applied.");

                var weather = new Weather();
                return weather.WeatherMain.Temp;
            }
        }
    }
}
