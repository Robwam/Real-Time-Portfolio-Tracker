using MarketData.Application.Configuration;
using MarketData.Application.Services;
using MarketData.Infrastructure.Cache;
using MarketData.Application.Interfaces;
using Shared.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MarketDataSettings>(
    builder.Configuration.GetSection("MarketData"));

// Register services
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddSingleton<IExternalMarketDataClientFactory, ExternalMarketDataClientFactory>();
builder.Services.AddSingleton<CacheKeyGenerator>();
builder.Services.AddScoped<IMarketDataService, MarketDataService>();
//builder.Services.AddScoped<IMarketDataClient, MarketDataClient>();

// Register Redis connection
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = builder.Configuration.GetConnectionString("Redis");
    return ConnectionMultiplexer.Connect(config);
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();