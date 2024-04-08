using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis.Extensions.Core.Abstractions;
using TestWebApi.Contexts;
using TestWebApi.Models.Enities;
using TestWebApi.Models.Responses;

namespace TestWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IRedisDatabase _redisDb0;
    private const string RedisKey = "_weather_forecast_";

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        ApplicationDbContext context,
        IRedisClient redisClient)
    {
        _logger = logger;
        _context = context;
        _redisDb0 = redisClient.Db0;
    }

    [HttpGet]
    public async Task<ActionResult<WeatherForecastResponse>> GetAllAsync()
    {
        var hashKeys = new HashSet<string> { RedisKey };
        var cachedAllKeysData = await _redisDb0.GetAllAsync<List<WeatherForecast>>(hashKeys);
        var cachedData = await _redisDb0.GetAsync<List<WeatherForecast>>(RedisKey);
        if (cachedData != null)
        {
            _logger.LogInformation("Data retrieved from cache.");
            return Ok(new WeatherForecastResponse { Result = cachedData, Count = cachedData.Count });
        }

        List<WeatherForecast> weatherForecasts = _context.WeatherForecasts.ToList();
        var count = weatherForecasts.Count;

        return Ok(new WeatherForecastResponse { Result = weatherForecasts, Count = count });
    }

    [HttpPost]
    public async Task<ActionResult<WeatherForecastResponse>> CreateAsync()
    {
        var listWeatherForecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }).ToList();

        foreach (var weatherForecast in listWeatherForecasts)
        {
            _context.WeatherForecasts.Add(weatherForecast);
            // await _redisDb0.AddAsync($"{weatherForecast.Id}", weatherForecast, TimeSpan.FromMinutes(10));
        }

        await _redisDb0.AddAsync(RedisKey, listWeatherForecasts, TimeSpan.FromMinutes(10));
        _context.SaveChanges();

        return CreatedAtAction("GetAll", new WeatherForecastResponse { Result = listWeatherForecasts, Count = listWeatherForecasts.Count }, listWeatherForecasts);
    }
}
