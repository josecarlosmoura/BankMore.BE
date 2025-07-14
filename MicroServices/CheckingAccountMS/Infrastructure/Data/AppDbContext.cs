using CheckingAccountMS.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CheckingAccountMS.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CheckingAccount> CheckingAccounts => Set<CheckingAccount>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<Idempotency> Idempotencies => Set<Idempotency>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CheckingAccountConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new IdempotencyConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}