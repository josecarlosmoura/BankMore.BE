using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Create_Table_Idempotencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Idempotencia",
                columns: table => new
                {
                    chaveIdempotencia = table.Column<byte[]>(type: "RAW(36)", nullable: false),
                    requisicao = table.Column<byte[]>(type: "RAW(36)", nullable: false),
                    resultado = table.Column<string>(type: "CLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Idempotencia", x => x.chaveIdempotencia);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Idempotencia");
        }
    }
}
