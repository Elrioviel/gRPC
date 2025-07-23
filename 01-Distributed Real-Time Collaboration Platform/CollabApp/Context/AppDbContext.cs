using CollabApp.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.Server.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentVersion> Versions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}
