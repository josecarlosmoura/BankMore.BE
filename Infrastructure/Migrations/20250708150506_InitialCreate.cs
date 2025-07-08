using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContaCorrente",
                columns: table => new
                {
                    idContaCorrente = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    numero = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    cpf = table.Column<long>(type: "NUMBER(11)", nullable: false),
                    nome = table.Column<string>(type: "VARCHAR2(200)", nullable: false),
                    ativo = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    senha = table.Column<string>(type: "VARCHAR2(512)", nullable: false),
                    salt = table.Column<string>(type: "VARCHAR2(128)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContaCorrente", x => x.idContaCorrente);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContaCorrente");
        }
    }
}
