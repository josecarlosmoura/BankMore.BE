using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransferMS.Domain.Entities;

namespace TransferMS.Infrastructure.Data.Configurations
{
    public class TransferConfiguration : IEntityTypeConfiguration<Transfer>
    {
        public void Configure(EntityTypeBuilder<Transfer> entity)
        {
            entity.ToTable("Transferencia");

            entity.HasKey(t => t.TransferId);

            entity.Property(t => t.TransferId)
                  .HasColumnName("idTransferencia")
                  .HasColumnType("VARCHAR2(36)")
                  .IsRequired();

            entity.Property(t => t.IdCheckingAccountFrom)
                  .HasColumnName("idContaCorrenteOrigem")
                  .HasColumnType("VARCHAR2(36)")
                  .IsRequired();

            entity.Property(t => t.IdCheckingAccountTo)
                  .HasColumnName("idContaCorrenteDestino")
                  .HasColumnType("VARCHAR2(36)")
                  .IsRequired();

            entity.Property(t => t.TransferDate)
                  .HasColumnName("dataTransferencia")
                  .HasColumnType("TIMESTAMP")
                  .IsRequired();

            entity.Property(t => t.Amount)
                  .HasColumnName("valor")
                  .HasPrecision(18, 2)
                  .IsRequired();
        }
    }
}
