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


app.MapGet("/ticketshigh",() =>
{
    
var hightickets = new List<Ticket>(); 
    foreach (var t in tickets)
    {
    
    if (t.Priority == "High")
    hightickets.Add(t);

    } 

return hightickets; 
});


app.MapGet("/", () => "Helpdesk API läuft");

app.Run();


