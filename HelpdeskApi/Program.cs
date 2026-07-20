using HelpdeskApi.Models;
using System.Collections.Generic;
using System.Linq;
using HelpdeskApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


var connectionString =
    builder.Configuration.GetConnectionString("HelpdeskDb")
    ?? throw new InvalidOperationException(
        "Connection String 'HelpdeskDb' wurde nicht gefunden.");

builder.Services.AddDbContext<HelpdeskDbContext>(options =>
    options.UseNpgsql(connectionString));


builder.Services.AddOpenApi();

var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<HelpdeskDbContext>();

    await db.Database.MigrateAsync();
     await DbSeeder.SeedAsync(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.MapGet("/high",async (HelpdeskDbContext db) =>
{
    var tickets = await db.Tickets.AsNoTracking().Where(t => t.Priority == "High").ToListAsync();

    if (tickets.Count == 0)
    {
        return Results.NotFound(new
        {
            message = $"Keine Tickets mit Hoher Prioritaet."
        });
    }

    return Results.Ok(tickets);
});



app.MapGet("/tickets",async (HelpdeskDbContext db) =>
{
    var tickets = await db.Tickets.AsNoTracking().ToListAsync();
    return Results.Ok(tickets);
});


app.MapGet("/tickets/{id}",async (int id, HelpdeskDbContext db) =>
{

    var ticket = await db.Tickets.AsNoTracking().FirstOrDefaultAsync(t=> t.Id == id);

    if (ticket is null)
    {
        return Results.NotFound(new
        {
            message = $"Ticket mit ID {id} wurde nicht gefunden."
        });
    }

    return Results.Ok(ticket);
});

app.MapGet("/open",async (HelpdeskDbContext db)=>
{
    var openticket = await db.Tickets.AsNoTracking().Where(t => t.Status == "Open").ToListAsync();
// trying Any() operator instead of count 
//  Count() => counting all records      Any() => return if you find First Record 
    if (!openticket.Any())
    {
        return Results.NotFound(new
        {
            message = $"Keine Offenen Tickets."
        });
    }

    return Results.Ok(openticket);
});


app.MapGet("/tickets/critical",async(HelpdeskDbContext db ) =>
{

    var crittickets = await db.Tickets.AsNoTracking().Where(t => t.Priority =="Critical").ToListAsync();

        if(crittickets.Count() == 0)

        {
               return Results.NotFound(new
                {
                    message = $"Keine Offenen Tickets."
                });
        }    
        
    return Results.Ok(crittickets);

});

app.MapPost("/tickets/set", async 
    (TicketCreateRequest request, HelpdeskDbContext db) =>
{
    var newTicket = new Ticket
    {
        Title = request.Title,
        Description = request.Description,
        Priority = request.Priority,
        Status = "Open",
        CreatedBy = request.CreatedBy,
        CreatedAt = DateTime.Now
    };

    db.Tickets.Add(newTicket);

    await db.SaveChangesAsync();

    return Results.Created($"/tickets/{newTicket.Id}", newTicket);
});


app.MapPut("/tickets/{id}", async (int id, TicketUpdateRequest request,HelpdeskDbContext db) =>
{
    var ticket = await db.Tickets.FindAsync(id);

    if (ticket is null)
    {
        return Results.NotFound(new
        {
            message = $"Ticket mit ID {id} wurde nicht gefunden."
        });
    }

    ticket.Title = request.Title ?? ticket.Title;
    ticket.Description = request.Description ?? ticket.Description;
    ticket.Status = request.Status ?? ticket.Status;
    ticket.Priority = request.Priority ?? ticket.Priority;
    await db.SaveChangesAsync();



    return Results.Ok( ticket);
});

app.MapDelete("/tickets/{id:int}", async (int id, HelpdeskDbContext  db) =>
{

    var ticket = await db.Tickets.FindAsync(id);


    if (ticket is null)
    {
        return Results.NotFound(new
        {
            message = $"Ticket mit ID {id} wurde nicht gefunden."
        });
    }

    db.Tickets.Remove(ticket);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapGet("/", () => "Helpdesk API läuft");

app.Run();


