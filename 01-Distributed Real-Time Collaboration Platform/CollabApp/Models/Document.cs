namespace CollabApp.Server.Models
{
    public class Document
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<DocumentVersion> Versions { get; set; }
    }
}
