using IntervalCalls.Server;
using IntervalWorkerService.Models;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntervalWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string _accessToken;
        private string _refreshToken;
        private string _email;
        private string _password;

        public Worker(ILogger<Worker> logger, HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration;
            _accessToken = _configuration["AccessToken"];
            _refreshToken = _configuration["RefreshToken"];
            _email = _configuration["Email"];
            _password = _configuration["Password"];
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (string.IsNullOrEmpty(_accessToken) || string.IsNullOrEmpty(_refreshToken))
                {
                    // If access or refresh tokens are null or empty, perform login
                    await LoginAndGetTokensAsync();
                }
                // Set the access token in the request header
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                var response = await _httpClient.GetAsync("http://localhost:5161/WeatherForecast");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // If Unauthorized response, try refreshing the tokens
                    _logger.LogInformation("Refreshing token!");
                    await RefreshTokenAsync();
                    response = await _httpClient.GetAsync("http://localhost:5161/WeatherForecast");

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        // If still Unauthorized, perform login again
                        await LoginAndGetTokensAsync();
                        response = await _httpClient.GetAsync("http://localhost:5161/WeatherForecast");
                    }
                }

                var weatherJson = await response.Content.ReadAsStringAsync();
                var weatherObject = JsonSerializer.Deserialize<WeatherForecast>(weatherJson);

                if (weatherObject != null)
                {
                    _logger.LogInformation("Weather: {0}", weatherObject.ToString());
                }
                else
                {
                    _logger.LogInformation("Weather service is down!");
                }

                await Task.Delay(50000, stoppingToken); // Wait for 5 seconds
            }
        }

        private async Task RefreshTokenAsync()
        {
            Debug.WriteLine("Refreshing token!");
            var refreshToken = _refreshToken;

            var refreshRequest = new
            {
                refreshToken
            };

            var jsonContent = JsonSerializer.Serialize(refreshRequest);

            var refreshResponse = await _httpClient.PostAsync("http://localhost:5161/refresh", new StringContent(jsonContent, Encoding.UTF8, "application/json"));

            if (refreshResponse.IsSuccessStatusCode)
            {
                var refreshJson = await refreshResponse.Content.ReadAsStringAsync();

                try
                {
                    var refreshResult = JsonSerializer.Deserialize<Response>(refreshJson);

                    _accessToken = refreshResult.accessToken;
                    _refreshToken = refreshResult.refreshToken;
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error deserializing JSON response in RefreshTokenAsync");
                }
            }
            else
            {
                _logger.LogError("Failed to refresh tokens. Status code: {0}", refreshResponse.StatusCode);
            }
        }

        private async Task LoginAndGetTokensAsync()
        {
            Debug.WriteLine("Logging In!");
            var loginRequest = new
            {
                email = _email,
                password = _password
            };

            var jsonContent = JsonSerializer.Serialize(loginRequest);

            var loginResponse = await _httpClient.PostAsync("http://localhost:5161/login", new StringContent(jsonContent, Encoding.UTF8, "application/json"));

            if (loginResponse.IsSuccessStatusCode)
            {
                var loginJson = await loginResponse.Content.ReadAsStringAsync();

                try
                {
                    var loginResult = JsonSerializer.Deserialize<Response>(loginJson);

                    _accessToken = loginResult.accessToken;
                    _refreshToken = loginResult.refreshToken;
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error deserializing JSON response in LoginAndGetTokensAsync");
                }
            }
            else
            {
                _logger.LogError("Failed to login. Status code: {0}", loginResponse.StatusCode);
            }
        }
    }

    public class TokenResponse
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
    }
}
