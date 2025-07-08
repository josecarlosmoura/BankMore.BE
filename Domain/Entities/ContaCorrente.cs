using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("ContaCorrente", Schema = "SYSTEM")]
    public class ContaCorrente
    {
        [Key]
        [Column("idContaCorrente", TypeName = "VARCHAR2(36)")]
        [Required]
        public string IdContaCorrente { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Column("numero", TypeName = "NUMBER(10)")]
        public long Numero { get; set; }

        [Required]
        [Column("cpf", TypeName = "NUMBER(11)")]
        public long Cpf { get; set; }

        [Required]
        [Column("nome", TypeName = "VARCHAR2(200)")]
        public string Nome { get; set; } = string.Empty;

        [Column("ativo", TypeName = "NUMBER(1)")]
        public bool Ativo { get; set; } = true;

        [Required]
        [Column("senha", TypeName = "VARCHAR2(512)")]
        public string Senha { get; set; } = string.Empty;

        [Required]
        [Column("salt", TypeName = "VARCHAR2(128)")]
        public string Salt { get; set; } = string.Empty;
    }
}
