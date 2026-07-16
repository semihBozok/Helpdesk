using HelpdeskApi.Models;
using System.Collections.Generic;
using System.Linq;

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
    new Ticket { Id = 1, Title = "Drucker funktioniert nicht", Description = "Drucker im 2. Stock reagiert nicht.", Status = "Open", Priority = "High", CreatedBy = "Semih", CreatedAt = DateTime.Now },

    new Ticket { Id = 2, Title = "Passwort vergessen", Description = "Benutzer kann sich nicht anmelden.", Status = "In Progress", Priority = "Medium", CreatedBy = "IT Support", CreatedAt = DateTime.Now },

    new Ticket { Id = 3, Title = "Outlook startet nicht", Description = "Outlook beendet sich sofort.", Status = "Open", Priority = "High", CreatedBy = "Anna", CreatedAt = DateTime.Now },

    new Ticket { Id = 4, Title = "VPN Verbindung fehlgeschlagen", Description = "VPN baut keine Verbindung auf.", Status = "Resolved", Priority = "Critical", CreatedBy = "Max", CreatedAt = DateTime.Now },

    new Ticket { Id = 5, Title = "Monitor flackert", Description = "Monitor zeigt Bildfehler.", Status = "Closed", Priority = "Low", CreatedBy = "Lisa", CreatedAt = DateTime.Now },

    new Ticket { Id = 6, Title = "Teams Mikrofon defekt", Description = "Kein Ton bei Meetings.", Status = "Open", Priority = "Medium", CreatedBy = "Kevin", CreatedAt = DateTime.Now },

    new Ticket { Id = 7, Title = "SAP Anmeldung fehlgeschlagen", Description = "SAP Login funktioniert nicht.", Status = "Open", Priority = "Critical", CreatedBy = "Sarah", CreatedAt = DateTime.Now },

    new Ticket { Id = 8, Title = "Scanner nicht erreichbar", Description = "Scanner wird im Netzwerk nicht gefunden.", Status = "In Progress", Priority = "Medium", CreatedBy = "Daniel", CreatedAt = DateTime.Now },

    new Ticket { Id = 9, Title = "Kein Internet", Description = "Netzwerkverbindung unterbrochen.", Status = "Open", Priority = "Critical", CreatedBy = "Julia", CreatedAt = DateTime.Now },

    new Ticket { Id = 10, Title = "Excel stürzt ab", Description = "Excel beendet sich beim Öffnen.", Status = "Resolved", Priority = "High", CreatedBy = "Thomas", CreatedAt = DateTime.Now },

    new Ticket { Id = 11, Title = "Druckauftrag hängt", Description = "Druckaufträge bleiben in Warteschlange.", Status = "Open", Priority = "Low", CreatedBy = "Michael", CreatedAt = DateTime.Now },

    new Ticket { Id = 12, Title = "MFA funktioniert nicht", Description = "Authentifizierung schlägt fehl.", Status = "In Progress", Priority = "Critical", CreatedBy = "Laura", CreatedAt = DateTime.Now },

    new Ticket { Id = 13, Title = "Dateifreigabe fehlt", Description = "Netzlaufwerk nicht sichtbar.", Status = "Open", Priority = "Medium", CreatedBy = "Markus", CreatedAt = DateTime.Now },

    new Ticket { Id = 14, Title = "PC startet langsam", Description = "Windows benötigt mehrere Minuten.", Status = "Closed", Priority = "Low", CreatedBy = "Stefan", CreatedAt = DateTime.Now },

    new Ticket { Id = 15, Title = "USB Gerät wird nicht erkannt", Description = "Scanner wird nicht erkannt.", Status = "Open", Priority = "Medium", CreatedBy = "Sandra", CreatedAt = DateTime.Now },

    new Ticket { Id = 16, Title = "E-Mail Versand fehlgeschlagen", Description = "Outlook sendet keine E-Mails.", Status = "Resolved", Priority = "High", CreatedBy = "Patrick", CreatedAt = DateTime.Now },

    new Ticket { Id = 17, Title = "Active Directory gesperrt", Description = "Benutzerkonto wurde gesperrt.", Status = "Open", Priority = "Critical", CreatedBy = "Nicole", CreatedAt = DateTime.Now },

    new Ticket { Id = 18, Title = "WLAN instabil", Description = "Verbindung bricht regelmäßig ab.", Status = "In Progress", Priority = "Medium", CreatedBy = "David", CreatedAt = DateTime.Now },

    new Ticket { Id = 19, Title = "Windows Update hängt", Description = "Update bleibt bei 30% stehen.", Status = "Open", Priority = "High", CreatedBy = "Tim", CreatedAt = DateTime.Now },

    new Ticket { Id = 20, Title = "Browser öffnet keine Seiten", Description = "Chrome lädt keine Webseiten.", Status = "Resolved", Priority = "Medium", CreatedBy = "Sophie", CreatedAt = DateTime.Now }
};


app.MapGet("/high",() =>
{
    
var hightickets = new List<Ticket>(); 
    foreach (var t in tickets)
    {
    
    if (t.Priority == "High")
    hightickets.Add(t);

    } 

return hightickets; 
});


app.MapGet("/tickets",() =>
{
    return tickets; 
});


app.MapGet("/tickets/{id}", (int id) =>
{
    var ticket = tickets.FirstOrDefault(t => t.Id == id);
    return ticket is null ? Results.NotFound(new { message = "Ticket nicht gefunden" }) : Results.Ok(ticket);
});

app.MapGet("/open",()=>
{
    var open = new List<Ticket>();
    foreach(var t in tickets)
    {
        if(t.Status=="Open")
        open.Add(t);
    }
    return open; 
});


app.MapGet("tickets/critical",() =>
{
    var crit = new List<Ticket>(); 
    foreach (var t in tickets)
    {
    
        if (t.Priority == "Critical")
        crit.Add(t);

    } 
    return crit;
});
app.MapPost("/tickets/set", (TicketCreateRequest request) =>
{
    var newTicket = new Ticket
    {
        Id = tickets.Count + 1,
        Title = request.Title,
        Description = request.Description,
        Priority = request.Priority,
        Status = "Open",
        CreatedBy = request.CreatedBy,
        CreatedAt = DateTime.Now
    };

    tickets.Add(newTicket);

    return Results.Ok(newTicket);
});


app.MapPut("/tickets/{id}", (int id, TicketUpdateRequest request) =>
{
    var ticket = tickets.FirstOrDefault(t => t.Id == id);
    if (ticket is null)
    {
        return Results.NotFound(new { message = "Ticket nicht gefunden" });
    }

    ticket.Title = request.Title ?? ticket.Title;
    ticket.Description = request.Description ?? ticket.Description;
    ticket.Status = request.Status ?? ticket.Status;
    ticket.Priority = request.Priority ?? ticket.Priority;

    return Results.Ok(ticket);
});

app.MapDelete("/tickets/{id:int}", (int id) =>
{
    var ticket = tickets.FirstOrDefault(t => t.Id == id); ; 

    if (ticket is null)
    {
        return Results.NotFound(new
        {
            message = $"Ticket mit ID {id} wurde nicht gefunden."
        });
    }

    tickets.Remove(ticket);

     return Results.NotFound(new { message = $"Ticket mit der  {id} wurde entfernt"  });
});

app.MapGet("/", () => "Helpdesk API läuft");

app.Run();


