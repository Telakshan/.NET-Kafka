using CQRS.Core.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    { 
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")] 
    public IEnumerable<string> Get()
    {
        var handlers = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            .Where(x => typeof(BaseCommand).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .Select(x => x.AssemblyQualifiedName).ToList();

        return handlers;

        //return Enumerable.Range(1, handlers.Count - 1).Select(index =>  (string)handlers[index]);

        /*return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            Test = Environment.GetEnvironmentVariable("KAFKA_TOPIC")
        })
        .ToArray();*/
    }
}
