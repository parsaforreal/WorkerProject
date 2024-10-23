using IntervalWorkerService.Models;
using System.Net;
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
        private string _loginURL = "http://localhost:5163/Identity/Account/Login";
        private string _requestURL = "http://localhost:5163/Admin/Payment/Check";

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
                var response = await _httpClient.GetAsync(_requestURL);


                var responseJson = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<MessageCode>(responseJson);

                if (responseObject != null)
                {
                    _logger.LogInformation(@"Payment check: {0}", responseObject.ToString());
                }
                else
                {
                    _logger.LogInformation("Payment service is down!");
                }

                await Task.Delay(5000, stoppingToken); // Wait for 5 seconds
            }
        }


        private async Task LoginAndGetTokensAsync()
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "Input.Email", "parsa@parsa.com" },
                { "Input.Password", "P@rs6!" },
                { "Input.RememberMe", "false" }
            });
            var loginResponse = await _httpClient.PostAsync(_loginURL, content);

            if (loginResponse.StatusCode == HttpStatusCode.Found)
            {
                var loginJson = await loginResponse.Content.ReadAsStringAsync();

                try
                {
                    var loginResult = JsonSerializer.Deserialize<CookieResponse>(loginJson);
                    var cookie = loginResult?._RequestVerificationToken ?? "";
                    _logger.LogInformation(cookie);
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
        public class TokenResponse
        {
            [JsonPropertyName("accessToken")]
            public string AccessToken { get; set; }

            [JsonPropertyName("refreshToken")]
            public string RefreshToken { get; set; }
        }
    }    
}

