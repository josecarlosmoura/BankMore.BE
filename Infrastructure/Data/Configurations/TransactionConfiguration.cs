using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> entity)
        {
            entity.ToTable("Movimento");

            entity.HasKey(t => t.TransactionId);

            entity.Property(t => t.TransactionId)
                  .HasColumnName("idMovimento")
                  .HasColumnType("RAW(36)");

            entity.Property(t => t.AccountId)
                  .HasColumnName("idContaCorrente")
                  .HasColumnType("RAW(36)")
                  .IsRequired();

            entity.Property(t => t.TransactionType)
                  .HasColumnName("tipoMovimento")
                  .HasConversion<string>()
                  .HasMaxLength(10)
                  .IsRequired();

            entity.Property(t => t.Amount)
                  .HasColumnName("valor")
                  .HasPrecision(18, 2)
                  .IsRequired();

            entity.HasOne(t => t.Account)
                  .WithMany()
                  .HasForeignKey(t => t.AccountId)
                  .HasConstraintName("FK_Transaction_ContaCorrente");
        }
    }
}
