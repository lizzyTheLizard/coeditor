using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Shared;
using System.Net;

namespace Api
{
    public class WeatherForecastFunction
    {
        private readonly ILogger<WeatherForecastFunction> _logger;

        public WeatherForecastFunction(ILogger<WeatherForecastFunction> logger)
        {
            _logger = logger;
        }

        [Function("WeatherForecast")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var forecast = new WeatherForecast[] { new() { Summary = "Dark" } };
            return new OkObjectResult(forecast);
        }
    }
}
