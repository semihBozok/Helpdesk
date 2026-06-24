namespace HelpdeskApi.Models
{
    public class Ticket
    {
        private int _id;
        private string _title;
        private string _description;
        private string _status;
        private string _priority;
        private string _createdBy;
        private DateTime _createdAt;
        private DateTime? _updatedAt;

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string Title
        {
            get => _title;
            set => _title = value;
        }

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        public string Status
        {
            get => _status;
            set => _status = value;
        }

        public string Priority
        {
            get => _priority;
            set => _priority = value;
        }

        public string CreatedBy
        {
            get => _createdBy;
            set => _createdBy = value;
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set => _createdAt = value;
        }

        public DateTime? UpdatedAt
        {
            get => _updatedAt;
            set => _updatedAt = value;
        }
    }
}