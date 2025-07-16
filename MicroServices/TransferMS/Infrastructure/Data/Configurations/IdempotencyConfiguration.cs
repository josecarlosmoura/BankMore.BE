using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransferMS.Domain.Entities;

namespace TransferMS.Infrastructure.Data.Configurations
{
    public class IdempotencyConfiguration : IEntityTypeConfiguration<Idempotency>
    {
        public void Configure(EntityTypeBuilder<Idempotency> entity)
        {
            entity.ToTable("Idempotencia");

            entity.HasKey(e => e.IdempotencyKey);

            entity.Property(e => e.IdempotencyKey)
                  .HasColumnName("chaveIdempotencia")
                  .HasColumnType("RAW(36)");

            entity.Property(e => e.RequestId)
                  .HasColumnName("requisicao")
                  .HasColumnType("RAW(36)")
                  .IsRequired();

            entity.Property(e => e.Result)
                  .HasColumnName("resultado")
                  .HasColumnType("CLOB") //CLOB (Character Large Object) - Pode ser VARCHAR2(4000) se preferir
                  .IsRequired();
        }
    }
}
