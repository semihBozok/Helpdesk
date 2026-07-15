namespace HelpdeskApi.Models

{
    public class TicketCreateRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
    }
}