namespace HelpdeskApi.Models;

public class TicketUpdateRequest
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? StatusId { get; set; }

    public int? PriorityId { get; set; }
}