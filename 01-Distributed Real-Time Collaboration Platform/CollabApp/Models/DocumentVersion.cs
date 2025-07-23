namespace CollabApp.Server.Models
{
    public class DocumentVersion
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public string ContentSnapshot { get; set; }
        public DateTime Timestamp { get; set; }

        public Document Document { get; set; }
    }
}
