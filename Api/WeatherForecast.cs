using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Api
{
    public static class WeatherForecast
    {
        [FunctionName("WeatherForecast")]
        public static IActionResult Run(
                    [HttpTrigger(AuthorizationLevel.Function,
            "get",
            Route="weather-forecast/{daysToForecast=10}")]
            HttpRequest req, ILogger log, int daysToForecast)
        {
            return new OkObjectResult(GetWeather(daysToForecast));
        }

        private static dynamic[] GetWeather(int daysToForecast)
        {
            var enumerator = Enumerable.Range(1, daysToForecast);
            var result = new List<dynamic>();
            var rnd = new Random();

            foreach (var day in enumerator)
            {
                var temp = rnd.Next(25);
                var summary = GetSummary(temp);
                result.Add(new
                {
                    Date = DateTime.Now.AddDays(day),
                    Summary = summary,
                    TemperatureC = temp
                });
            }
            return result.ToArray();
        }
        private static object GetSummary(int temp)
        {
            return temp switch
            {
                int i when (i > 20) => "Hot!",
                int i when (i > 15) => "Warm",
                int i when (i > 10) => "Cool",
                int i when (i > 5) => "Cold!",
                _ => "Too Cold!",
            };
        }
    }
}

