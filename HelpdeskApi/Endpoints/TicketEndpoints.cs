using HelpdeskApi.Data;
using HelpdeskApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskApi.Endpoints;

public static class TicketEndpoints
{
    public static void MapTicketEndpoints(this WebApplication app)
    {
        var ticketsGroup = app.MapGroup("/tickets");

        ticketsGroup.MapGet("", async (HelpdeskDbContext db) =>
        {
            var tickets = await db.Tickets
                .AsNoTracking()
                .ToListAsync();

            return Results.Ok(tickets);
        });

        ticketsGroup.MapGet("/{id:int}", async (
            int id,
            HelpdeskDbContext db) =>
        {
            var ticket = await db.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket is null)
            {
                return Results.NotFound(new
                {
                    message = $"Ticket mit ID {id} wurde nicht gefunden."
                });
            }

            return Results.Ok(ticket);
        });

        ticketsGroup.MapGet("/high", async (HelpdeskDbContext db) =>
        {
            var tickets = await db.Tickets
                .AsNoTracking()
                .Where(t => t.Priority == "High")
                .ToListAsync();

            return Results.Ok(tickets);
        });

        ticketsGroup.MapGet("/open", async (HelpdeskDbContext db) =>
        {
            var tickets = await db.Tickets
                .AsNoTracking()
                .Where(t => t.Status == "Open")
                .ToListAsync();

            return Results.Ok(tickets);
        });

        ticketsGroup.MapGet("/critical", async (HelpdeskDbContext db) =>
        {
            var tickets = await db.Tickets
                .AsNoTracking()
                .Where(t => t.Priority == "Critical")
                .ToListAsync();

            return Results.Ok(tickets);
        });

        ticketsGroup.MapPost("", async (
            TicketCreateRequest request,
            HelpdeskDbContext db) =>
        {
            var newTicket = new Ticket
            {
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                Status = "Open",
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            db.Tickets.Add(newTicket);
            await db.SaveChangesAsync();

            return Results.Created(
                $"/tickets/{newTicket.Id}",
                newTicket);
        });

        ticketsGroup.MapPut("/{id:int}", async (
            int id,
            TicketUpdateRequest request,
            HelpdeskDbContext db) =>
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
            ticket.Description =
                request.Description ?? ticket.Description;
            ticket.Status = request.Status ?? ticket.Status;
            ticket.Priority = request.Priority ?? ticket.Priority;

            await db.SaveChangesAsync();

            return Results.Ok(ticket);
        });

        ticketsGroup.MapDelete("/{id:int}", async (
            int id,
            HelpdeskDbContext db) =>
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
    }
}