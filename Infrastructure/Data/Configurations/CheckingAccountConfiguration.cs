using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class CheckingAccountConfiguration : IEntityTypeConfiguration<CheckingAccount>
    {
        public void Configure(EntityTypeBuilder<CheckingAccount> entity)
        {
            entity.ToTable("ContaCorrente");

            entity.HasKey(c => c.CheckingAccountId);

            entity.Property(c => c.CheckingAccountId)
                  .HasColumnName("idContaCorrente")
                  .HasColumnType("RAW(36)");

            entity.Property(c => c.AccountNumber)
                  .HasColumnName("numero")
                  .HasColumnType("NUMBER(10)")
                  .IsRequired();

            entity.Property(c => c.Cpf)
                    .HasColumnName("cpf")
                    .HasColumnType("VARCHAR2(11)")
                    .IsRequired();

            entity.Property(c => c.FullName)
                  .HasColumnName("nome")
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(c => c.Password)
                  .HasColumnName("senha")
                  .HasMaxLength(512)
                  .IsRequired();

            entity.Property(c => c.Salt)
                  .HasColumnName("salt")
                  .HasMaxLength(128)
                  .IsRequired();

            entity.Property(c => c.IsActive)
                  .HasColumnName("ativo")
                  .HasConversion<int>() // Oracle não suporta boolean: armazena como 0/1
                  .IsRequired();
        }
    }
}
