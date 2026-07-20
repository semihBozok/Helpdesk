using HelpdeskApi.Data;
using HelpdeskApi.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("HelpdeskDb")
    ?? throw new InvalidOperationException(
        "Connection String 'HelpdeskDb' wurde nicht gefunden.");

builder.Services.AddDbContext<HelpdeskDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddOpenApi();

var app = builder.Build();


// Migrationen ausführen und Mockdaten anlegen
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<HelpdeskDbContext>();

    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Registriert alle Ticket-Routen
app.MapTicketEndpoints();

app.MapGet("/", () => "Helpdesk API läuft");

app.Run();