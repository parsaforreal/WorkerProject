using System.Text.Json;
using IntervalWorkerService.Models;

namespace IntervalWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly HttpClient _httpClient;

        public Worker(ILogger<Worker> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Fetch a random cat fact from the API
                var response = await _httpClient.GetAsync("https://catfact.ninja/fact");

                var factJson = await response.Content.ReadAsStringAsync();
                var factObject = JsonSerializer.Deserialize<CatFactObject>(factJson);

                //Log the cat fact to the console
                if (factObject != null)
                {
                    // Log the cat fact to the console
                    _logger.LogInformation("Cat Fact: {fact}", factObject.fact);
                }
                else
                {
                    // Log a message if no cat fact was retrieved
                    _logger.LogInformation("No cat fact was able to be retrieved at this interval!");
                }

                await Task.Delay(2000, stoppingToken); // Wait for 5 seconds
            }
        }
    }
}
