using Microsoft.EntityFrameworkCore;

namespace ProductGrpc.Server.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<ProductEntities> Products { get; set; } = null!;
        public DbSet<LinkEntities> Links { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options) { }

        protected override void OnModelCreating (ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LinkEntities> ()
                .HasOne(l => l.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(l => l.ParentId)
                .OnDelete (DeleteBehavior.Restrict);

            modelBuilder.Entity<LinkEntities> ()
                .HasOne(l => l.Child)
                .WithMany(p => p.Parents)
                .HasForeignKey (l => l.ChildId)
                .OnDelete (DeleteBehavior.Restrict);
        }
    }
}
