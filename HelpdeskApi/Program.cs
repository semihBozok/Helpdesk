using HelpdeskApi.Models;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
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


var tickets = new List<Ticket>
{
    new Ticket
    {
        Id = 1,
        Title = "Drucker funktioniert nicht",
        Description = "Der Drucker im 2. Stock reagiert nicht",
        Status = "Open",
        Priority = "High",
        CreatedBy = "Semih",
        CreatedAt = DateTime.Now
    },
    new Ticket
    {
        Id = 2,
        Title = "Passwort vergessen",
        Description = "User kann sich nicht anmelden",
        Status = "In Progress",
        Priority = "Medium",
        CreatedBy = "IT Support",
        CreatedAt = DateTime.Now
    }
};
app.MapGet("/tickets", () =>
{
    return tickets;
});

app.MapGet("/", () => "Helpdesk API läuft");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
