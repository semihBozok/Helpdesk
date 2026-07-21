using HelpdeskApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskApi.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(HelpdeskDbContext db)
    {
        // Wenn bereits mindestens ein Ticket existiert,
        // werden keine weiteren Mockdaten angelegt.
        if (await db.Tickets.AnyAsync())
        {
            return;
        }

        var now = DateTime.UtcNow;

        var statuses = new List<TicketStatus>
{
    new() { Id = 1, Name = "Open" },
    new() { Id = 2, Name = "In Progress" },
    new() { Id = 3, Name = "Resolved" },
    new() { Id = 4, Name = "Closed" }
};

        var priorities = new List<TicketPriority>
{
    new() { Id = 1, Name = "Low" },
    new() { Id = 2, Name = "Medium" },
    new() { Id = 3, Name = "High" },
    new() { Id = 4, Name = "Critical" }
};

        db.TicketStatuses.AddRange(statuses);
        db.TicketPriorities.AddRange(priorities);

        await db.SaveChangesAsync();

        var tickets = new List<Ticket>
        {
            new()
            {
                Title = "VPN funktioniert nicht",
                Description = "Die Verbindung zum Firmennetzwerk schlägt fehl.",
                StatusId = 1,
                PriorityId = 3,
                CreatedBy = "Semih",
                CreatedAt = now
            },
            new()
            {
                Title = "Drucker im zweiten Stock offline",
                Description = "Der Netzwerkdrucker reagiert nicht auf Druckaufträge.",
                StatusId = 1,
                PriorityId = 2,
                CreatedBy = "Anna",
                CreatedAt = now.AddHours(-2)
            },
            new()
            {
                Title = "Passwort zurücksetzen",
                Description = "Der Benutzer hat sein Windows-Passwort vergessen.",
                StatusId = 2,
                PriorityId = 1,
                CreatedBy = "Max",
                CreatedAt = now.AddHours(-5)
            },
            new()
            {
                Title = "Outlook startet nicht",
                Description = "Outlook bleibt beim Ladebildschirm hängen.",
                StatusId = 1,
                PriorityId = 3,
                CreatedBy = "Lisa",
                CreatedAt = now.AddHours(-7)
            },
            new()
            {
                Title = "Laptop ist sehr langsam",
                Description = "Das Gerät reagiert seit dem letzten Update sehr langsam.",
                StatusId = 1,
                PriorityId = 2,
                CreatedBy = "Thomas",
                CreatedAt = now.AddHours(-10)
            },
            new()
            {
                Title = "MFA-Code wird nicht akzeptiert",
                Description = "Die Anmeldung schlägt trotz korrektem Authenticator-Code fehl.",
                StatusId = 1,
                PriorityId = 4,
                CreatedBy = "Sarah",
                CreatedAt = now.AddHours(-12)
            },
            new()
            {
                Title = "Teams-Mikrofon funktioniert nicht",
                Description = "In Microsoft Teams wird kein Audiosignal erkannt.",
                StatusId = 1,
                PriorityId = 2,
                CreatedBy = "Kevin",
                CreatedAt = now.AddHours(-15)
            },
            new()
            {
                Title = "SAP-Anmeldung fehlgeschlagen",
                Description = "Der Benutzer kann sich nicht am SAP-System anmelden.",
                StatusId = 1,
                PriorityId = 4,
                CreatedBy = "Daniel",
                CreatedAt = now.AddDays(-1)
            },
            new()
            {
                Title = "Scanner nicht erreichbar",
                Description = "Der Scanner wird im Firmennetzwerk nicht gefunden.",
                StatusId = 1,
                PriorityId = 2,
                CreatedBy = "Julia",
                CreatedAt = now.AddDays(-1).AddHours(-2)
            },
            new()
            {
                Title = "Keine Internetverbindung",
                Description = "Der Arbeitsplatz hat keine Verbindung zum Internet.",
                StatusId = 2,
                PriorityId = 4,
                CreatedBy = "Michael",
                CreatedAt = now.AddDays(-1).AddHours(-5)
            },
            new()
            {
                Title = "Excel stürzt beim Öffnen ab",
                Description = "Microsoft Excel beendet sich beim Öffnen einer Datei.",
                StatusId = 1,
                PriorityId = 3,
                CreatedBy = "Laura",
                CreatedAt = now.AddDays(-2)
            },
            new()
            {
                Title = "Druckauftrag hängt",
                Description = "Mehrere Druckaufträge bleiben in der Warteschlange.",
                StatusId = 1,
                PriorityId = 1,
                CreatedBy = "Markus",
                CreatedAt = now.AddDays(-2).AddHours(-3)
            },
            new()
            {
                Title = "Netzlaufwerk fehlt",
                Description = "Das Abteilungslaufwerk wird im Explorer nicht angezeigt.",
                StatusId = 1,
                PriorityId = 3,
                CreatedBy = "Stefan",
                CreatedAt = now.AddDays(-2).AddHours(-7)
            },
            new()
            {
                Title = "PC startet sehr langsam",
                Description = "Der Windows-Start dauert länger als zehn Minuten.",
                StatusId = 2,
                PriorityId = 1,
                CreatedBy = "Sandra",
                CreatedAt = now.AddDays(-3)
            },
            new()
            {
                Title = "USB-Gerät wird nicht erkannt",
                Description = "Ein angeschlossenes USB-Gerät erscheint nicht im System.",
                StatusId = 1,
                PriorityId = 2,
                CreatedBy = "Patrick",
                CreatedAt = now.AddDays(-3).AddHours(-4)
            },
            new()
            {
                Title = "E-Mail-Versand fehlgeschlagen",
                Description = "Outlook kann keine externen E-Mails versenden.",
                StatusId = 2,
                PriorityId = 3,
                CreatedBy = "Nicole",
                CreatedAt = now.AddDays(-4)
            },
            new()
            {
                Title = "Benutzerkonto gesperrt",
                Description = "Das Active-Directory-Konto wurde nach Fehlversuchen gesperrt.",
                StatusId = 1,
                PriorityId = 4,
                CreatedBy = "David",
                CreatedAt = now.AddDays(-4).AddHours(-5)
            },
            new()
            {
                Title = "WLAN-Verbindung instabil",
                Description = "Die WLAN-Verbindung bricht regelmäßig ab.",
                StatusId = 1,
                PriorityId = 2,
                CreatedBy = "Tim",
                CreatedAt = now.AddDays(-5)
            },
            new()
            {
                Title = "Windows Update hängt",
                Description = "Die Installation bleibt dauerhaft bei 30 Prozent stehen.",
                StatusId = 1,
                PriorityId = 3,
                CreatedBy = "Sophie",
                CreatedAt = now.AddDays(-5).AddHours(-6)
            },
            new()
            {
                Title = "Browser lädt keine Webseiten",
                Description = "Chrome zeigt bei allen Webseiten einen Netzwerkfehler.",
                StatusId = 2,
                PriorityId = 2,
                CreatedBy = "Felix",
                CreatedAt = now.AddDays(-6)
            },
            new()
            {
                Title = "Monitor bleibt schwarz",
                Description = "Der externe Monitor erhält kein Bildsignal.",
                StatusId = 1,
                PriorityId = 3,
                CreatedBy = "Nina",
                CreatedAt = now.AddDays(-6).AddHours(-4)
            },
            new()
            {
                Title = "Softwareinstallation benötigt",
                Description = "Für ein Projekt wird Visual Studio Code benötigt.",
                StatusId = 1,
                PriorityId = 1,
                CreatedBy = "Leon",
                CreatedAt = now.AddDays(-7)
            },
            new()
            {
                Title = "Citrix-Sitzung startet nicht",
                Description = "Die virtuelle Arbeitsumgebung lässt sich nicht öffnen.",
                StatusId = 1,
                PriorityId = 4,
                CreatedBy = "Marie",
                CreatedAt = now.AddDays(-8)
            },
            new()
            {
                Title = "Kalenderfreigabe fehlt",
                Description = "Der Teamkalender kann in Outlook nicht geöffnet werden.",
                StatusId = 1,
                PriorityId = 2,
                CreatedBy = "Jonas",
                CreatedAt = now.AddDays(-9)
            },
            new()
            {
                Title = "Headset wird nicht erkannt",
                Description = "Das USB-Headset erscheint nicht in den Audiogeräten.",
                StatusId = 2,
                PriorityId = 1,
                CreatedBy = "Lea",
                CreatedAt = now.AddDays(-10)
            }
        };

        db.Tickets.AddRange(tickets);

        await db.SaveChangesAsync();
    }
}