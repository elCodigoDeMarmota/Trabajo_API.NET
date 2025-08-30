using Microsoft.EntityFrameworkCore;
using Trabajo_API_NET.Models;

namespace Trabajo_API_NET.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Snippet> Snippets => Set<Snippet>();
        public DbSet<BuscarCode> BuscarCodes => Set<BuscarCode>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BuscarCode>()
                .HasNoKey()
                .ToView(null);
        }
    }
}
