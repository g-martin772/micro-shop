using AuthService.API.Data;
using AuthService.API.Models;
using Microsoft.AspNetCore.Identity;
using ProductService.Api.Data;
using DbInitializer = ProductService.Api.Data.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<ProductServiceContext>("ProductDb");

var identityUrl = "http://localhost:5001";

builder.Services.AddAuthentication()
    .AddJwtBearer(o =>
    {
        o.Authority = identityUrl;
        o.RequireHttpsMetadata = false;
        o.Audience = "catalog";
        o.TokenValidationParameters.ValidIssuers = [identityUrl];
        o.TokenValidationParameters.ValidateIssuer = false;
        o.TokenValidationParameters.ValidateAudience = false;
        o.TokenValidationParameters.ValidateIssuerSigningKey = false;
    });

builder.Services.AddSingleton<DbInitializer>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<DbInitializer>());

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapDefaultEndpoints();
app.MapOpenApi();

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}