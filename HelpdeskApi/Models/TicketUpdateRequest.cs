namespace HelpdeskApi.Models
{
public class TicketUpdateRequest
{
    
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status{get; set;}
        public string Priority { get; set; }
        public string CreatedBy { get; set; }

}
}