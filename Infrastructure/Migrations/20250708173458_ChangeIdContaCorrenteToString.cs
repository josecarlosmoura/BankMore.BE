using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIdContaCorrenteToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "idContaCorrente",
                table: "ContaCorrente",
                type: "VARCHAR2(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "RAW(16)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "idContaCorrente",
                table: "ContaCorrente",
                type: "RAW(16)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR2(36)");
        }
    }
}
