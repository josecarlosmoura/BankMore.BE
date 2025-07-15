using System;
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
                name: "Idempotencia",
                columns: table => new
                {
                    chaveIdempotencia = table.Column<byte[]>(type: "RAW(36)", nullable: false),
                    requisicao = table.Column<byte[]>(type: "RAW(36)", nullable: false),
                    resultado = table.Column<string>(type: "CLOB", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Idempotencia", x => x.chaveIdempotencia);
                });

            migrationBuilder.CreateTable(
                name: "Transferencia",
                columns: table => new
                {
                    idTransferencia = table.Column<string>(type: "VARCHAR2(36)", nullable: false),
                    idContaCorrenteOrigem = table.Column<string>(type: "VARCHAR2(36)", nullable: false),
                    idContaCorrenteDestino = table.Column<string>(type: "VARCHAR2(36)", nullable: false),
                    dataTransferencia = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    valor = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transferencia", x => x.idTransferencia);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Idempotencia");

            migrationBuilder.DropTable(
                name: "Transferencia");
        }
    }
}
