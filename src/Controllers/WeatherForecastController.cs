using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new string[]
        {
           "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private static readonly Random _random = new Random();

        [HttpGet("/weatherforecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                _random.Next(-20, 55),
                Summaries[_random.Next(Summaries.Length)]
            )).ToArray();
        }

        [HttpPost("/weatherforecast/add")]
        public IActionResult AddSummary([FromBody] string summary)
        {
            if (summary == null || Summaries.Length >= 50)
           {
               return BadRequest("Summary cannot be null and maximum 50 items can be in summaries.");
           }
           Summaries = Summaries.Concat(new[] { summary }).ToArray();
           return Ok(summary);
        }

        public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary);
    }
}
