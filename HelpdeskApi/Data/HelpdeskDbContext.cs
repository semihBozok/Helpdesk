using HelpdeskApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskApi.Data;

public class HelpdeskDbContext : DbContext
{
    public HelpdeskDbContext(
        DbContextOptions<HelpdeskDbContext> options)
        : base(options)
    {
    }

    public DbSet<Ticket> Tickets => Set<Ticket>();

    public DbSet<TicketStatus> TicketStatuses =>
        Set<TicketStatus>();

    public DbSet<TicketPriority> TicketPriorities =>
        Set<TicketPriority>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TicketStatus>()
            .HasKey(status => status.Id);

        modelBuilder.Entity<TicketPriority>()
            .HasKey(priority => priority.Id);

        modelBuilder.Entity<Ticket>()
            .HasKey(ticket => ticket.Id);

        modelBuilder.Entity<Ticket>()
            .HasOne(ticket => ticket.Status)
            .WithMany()
            .HasForeignKey(ticket => ticket.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne(ticket => ticket.Priority)
            .WithMany()
            .HasForeignKey(ticket => ticket.PriorityId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TicketStatus>()
            .HasIndex(status => status.Name)
            .IsUnique();

        modelBuilder.Entity<TicketPriority>()
            .HasIndex(priority => priority.Name)
            .IsUnique();
    }
}