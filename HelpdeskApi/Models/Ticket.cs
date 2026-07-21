namespace HelpdeskApi.Models;

public class Ticket
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;


    // fk table TicketStatus
    public int StatusId { get; set; }


    public TicketStatus Status { get; set; } = null!;

    // fk table TicketPriority  
    public int PriorityId { get; set; }

  
    public TicketPriority Priority { get; set; } = null!;


    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}