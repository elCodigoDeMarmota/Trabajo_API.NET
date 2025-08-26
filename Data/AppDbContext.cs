using Microsoft.EntityFrameworkCore;
using Trabajo_API_NET.Models;

namespace Trabajo_API_NET.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Snippet> Snippets { get; set; }
    }
}
