using Microsoft.EntityFrameworkCore;

namespace Peritus_BE.Context
{
    public class PeritusDbContext : DbContext
    {
        public PeritusDbContext(DbContextOptions<PeritusDbContext> options) : base(options)
        {
        }
        public DbSet<Quote> Quotes => Set<Quote>();
    }
}
