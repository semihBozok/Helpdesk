namespace HelpdeskApi.Models;

public class TicketCreateRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int PriorityId { get; set; }

    public string CreatedBy { get; set; } = string.Empty;
}