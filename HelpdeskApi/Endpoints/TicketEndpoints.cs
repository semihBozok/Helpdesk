using HelpdeskApi.Data;
using HelpdeskApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskApi.Endpoints;

public static class TicketEndpoints
{
    private const int OpenStatusId = 1;
    private const int HighPriorityId = 3;
    private const int CriticalPriorityId = 4;

    public static void MapTicketEndpoints(this WebApplication app)
    {
        var ticketsGroup = app
            .MapGroup("/tickets")
            .WithTags("Tickets");


        // GET /tickets
        ticketsGroup.MapGet("", async (HelpdeskDbContext db) =>
        {
            var tickets = await db.Tickets
                .AsNoTracking()
                .Include(ticket => ticket.Status)
                .Include(ticket => ticket.Priority)
                .OrderByDescending(ticket => ticket.CreatedAt)
                .ToListAsync();

            return Results.Ok(tickets);
        });

        // GET /tickets/statuses
        ticketsGroup.MapGet("/statuses", async (
            HelpdeskDbContext db) =>
        {
            var statuses = await db.TicketStatuses
                .AsNoTracking()
                .OrderBy(status => status.Id)
                .ToListAsync();

            return Results.Ok(statuses);
        });

        // GET /tickets/priorities
        ticketsGroup.MapGet("/priorities", async (
                HelpdeskDbContext db) =>
{
    var priorities = await db.TicketPriorities
     .AsNoTracking()
     .OrderBy(priority => priority.Id)
     .ToListAsync();

    return Results.Ok(priorities);
});


        ticketsGroup.MapGet("/{id:int}", async (
            int id,
            HelpdeskDbContext db) =>
        {
            var ticket = await db.Tickets
                .AsNoTracking()
                .Include(ticket => ticket.Status)
                .Include(ticket => ticket.Priority)
                .FirstOrDefaultAsync(ticket => ticket.Id == id);

            if (ticket is null)
            {
                return Results.NotFound(new
                {
                    message = $"Ticket mit ID {id} wurde nicht gefunden."
                });
            }

            return Results.Ok(ticket);
        });

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "healthy",
        timestamp = DateTime.UtcNow
    });
});
        // GET /tickets/high
        ticketsGroup.MapGet("/high", async (HelpdeskDbContext db) =>
        {
            var tickets = await db.Tickets
                .AsNoTracking()
                .Include(ticket => ticket.Status)
                .Include(ticket => ticket.Priority)
                .Where(ticket => ticket.PriorityId == HighPriorityId)
                .OrderByDescending(ticket => ticket.CreatedAt)
                .ToListAsync();

            return Results.Ok(tickets);
        });


        // GET /tickets/open
        ticketsGroup.MapGet("/open", async (HelpdeskDbContext db) =>
        {
            var tickets = await db.Tickets
                .AsNoTracking()
                .Include(ticket => ticket.Status)
                .Include(ticket => ticket.Priority)
                .Where(ticket => ticket.StatusId == OpenStatusId)
                .OrderByDescending(ticket => ticket.CreatedAt)
                .ToListAsync();

            return Results.Ok(tickets);
        });


        // GET /tickets/critical
        ticketsGroup.MapGet("/critical", async (HelpdeskDbContext db) =>
        {
            var tickets = await db.Tickets
                .AsNoTracking()
                .Include(ticket => ticket.Status)
                .Include(ticket => ticket.Priority)
                .Where(ticket => ticket.PriorityId == CriticalPriorityId)
                .OrderByDescending(ticket => ticket.CreatedAt)
                .ToListAsync();

            return Results.Ok(tickets);
        });


        ticketsGroup.MapPost("", async (
            TicketCreateRequest request,
            HelpdeskDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Results.BadRequest(new
                {
                    message = "Der Titel darf nicht leer sein."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                return Results.BadRequest(new
                {
                    message = "Die Beschreibung darf nicht leer sein."
                });
            }

            if (string.IsNullOrWhiteSpace(request.CreatedBy))
            {
                return Results.BadRequest(new
                {
                    message = "CreatedBy darf nicht leer sein."
                });
            }

            var priorityExists = await db.TicketPriorities
                .AnyAsync(priority =>
                    priority.Id == request.PriorityId);

            if (!priorityExists)
            {
                return Results.BadRequest(new
                {
                    message =
                        $"PriorityId {request.PriorityId} existiert nicht."
                });
            }

            var newTicket = new Ticket
            {
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),

                // Jedes neue Ticket startet als Open.
                StatusId = OpenStatusId,

                PriorityId = request.PriorityId,
                CreatedBy = request.CreatedBy.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            db.Tickets.Add(newTicket);
            await db.SaveChangesAsync();

            // Das Ticket noch einmal inklusive Beziehungen laden.
            var createdTicket = await db.Tickets
                .AsNoTracking()
                .Include(ticket => ticket.Status)
                .Include(ticket => ticket.Priority)
                .SingleAsync(ticket =>
                    ticket.Id == newTicket.Id);

            return Results.Created(
                $"/tickets/{createdTicket.Id}",
                createdTicket);
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

            if (request.Title is not null)
            {
                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    return Results.BadRequest(new
                    {
                        message = "Der Titel darf nicht leer sein."
                    });
                }

                ticket.Title = request.Title.Trim();
            }

            if (request.Description is not null)
            {
                if (string.IsNullOrWhiteSpace(request.Description))
                {
                    return Results.BadRequest(new
                    {
                        message =
                            "Die Beschreibung darf nicht leer sein."
                    });
                }

                ticket.Description = request.Description.Trim();
            }

            if (request.StatusId.HasValue)
            {
                var statusExists = await db.TicketStatuses
                    .AnyAsync(status =>
                        status.Id == request.StatusId.Value);

                if (!statusExists)
                {
                    return Results.BadRequest(new
                    {
                        message =
                            $"StatusId {request.StatusId.Value} existiert nicht."
                    });
                }

                ticket.StatusId = request.StatusId.Value;
            }

            if (request.PriorityId.HasValue)
            {
                var priorityExists = await db.TicketPriorities
                    .AnyAsync(priority =>
                        priority.Id == request.PriorityId.Value);

                if (!priorityExists)
                {
                    return Results.BadRequest(new
                    {
                        message =
                            $"PriorityId {request.PriorityId.Value} existiert nicht."
                    });
                }

                ticket.PriorityId = request.PriorityId.Value;
            }

            ticket.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            var updatedTicket = await db.Tickets
                .AsNoTracking()
                .Include(currentTicket => currentTicket.Status)
                .Include(currentTicket => currentTicket.Priority)
                .SingleAsync(currentTicket =>
                    currentTicket.Id == ticket.Id);

            return Results.Ok(updatedTicket);
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