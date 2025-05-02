using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Portfolio.Data.Database;
using Portfolio.Data.Repository;
using Portfolio.Data.Repository.Interfaces;
using Portfolio.Services;
using Portfolio.Services.Interfaces;
using Shared.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers().AddApplicationPart(typeof(Program).Assembly);

// Configure DbContext
builder.Services.AddDbContext<PostgresContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// Register services
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddScoped<IPortfolioService, PortfolioService>();
builder.Services.AddScoped<IPortfolioFactory, PortfolioFactory>();
builder.Services.AddScoped<IMarketDataClient, MarketDataClient>();

builder.Services.AddHttpClient<MarketDataClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetConnectionString("MarketDataService"));
});

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Portfolio API",
        Version = "v1",
        Description = "API for managing investment portfolios"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Portfolio API v1"));
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// This line keeps the application running
app.Run();