using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Newtonsoft;
using TestWebApi.Contexts;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis.Extensions.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Add services to the container.
var assemblyName = typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name;
builder.Services.AddDbContext<ApplicationDbContext>(
c => c.UseSqlServer(
         builder.Configuration.GetConnectionString("ApplicationConnection"),
         o => o.MigrationsAssembly(assemblyName)));

var settings = new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() };
var serializer = new NewtonsoftSerializer(settings);

builder.Services.AddSingleton(builder.Configuration.GetSection("Redis")
    .Get<RedisConfiguration>(о => о.BindNonPublicProperties = true));

builder.Services.AddSingleton<IRedisClient, RedisClient>();
builder.Services.AddSingleton<IRedisConnectionPoolManager, RedisConnectionPoolManager>();
builder.Services.AddSingleton<ISerializer>(serializer);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
