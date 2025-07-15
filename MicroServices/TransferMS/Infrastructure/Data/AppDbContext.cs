using Microsoft.EntityFrameworkCore;
using TransferMS.Infrastructure.Data.Configurations;
using TransferMS.Domain.Entities;


namespace TransferMS.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Transfer> Transfers => Set<Transfer>();        
        public DbSet<Idempotency> Idempotencies => Set<Idempotency>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TransferConfiguration());
            modelBuilder.ApplyConfiguration(new IdempotencyConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}