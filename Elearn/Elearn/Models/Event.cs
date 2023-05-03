namespace Elearn.Models
{
    public class Event:BaseEntity
    {
        public string? Header { get; set; }

        public string? Location { get; set; }

        public DateTime PublishedDate { get; set; }

    }
}
