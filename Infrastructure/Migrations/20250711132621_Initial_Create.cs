using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Create : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContaCorrente",
                columns: table => new
                {
                    idContaCorrente = table.Column<byte[]>(type: "RAW(36)", nullable: false),
                    numero = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    cpf = table.Column<string>(type: "VARCHAR2(11)", nullable: false),
                    nome = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    ativo = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    senha = table.Column<string>(type: "NVARCHAR2(512)", maxLength: 512, nullable: false),
                    salt = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContaCorrente", x => x.idContaCorrente);
                });

            migrationBuilder.CreateTable(
                name: "Movimento",
                columns: table => new
                {
                    idMovimento = table.Column<byte[]>(type: "RAW(36)", nullable: false),
                    idContaCorrente = table.Column<byte[]>(type: "RAW(36)", nullable: false),
                    tipoMovimento = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    valor = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    CheckingAccountId = table.Column<byte[]>(type: "RAW(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimento", x => x.idMovimento);
                    table.ForeignKey(
                        name: "FK_Movimento_ContaCorrente_CheckingAccountId",
                        column: x => x.CheckingAccountId,
                        principalTable: "ContaCorrente",
                        principalColumn: "idContaCorrente");
                    table.ForeignKey(
                        name: "FK_Transaction_ContaCorrente",
                        column: x => x.idContaCorrente,
                        principalTable: "ContaCorrente",
                        principalColumn: "idContaCorrente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movimento_CheckingAccountId",
                table: "Movimento",
                column: "CheckingAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimento_idContaCorrente",
                table: "Movimento",
                column: "idContaCorrente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movimento");

            migrationBuilder.DropTable(
                name: "ContaCorrente");
        }
    }
}
