using Microsoft.EntityFrameworkCore;
using TestWebApi.Models.Enities;

namespace TestWebApi.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<WeatherForecast> WeatherForecasts { get; set; }
}
