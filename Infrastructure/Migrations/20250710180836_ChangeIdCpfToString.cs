using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIdCpfToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "cpf",
                table: "ContaCorrente",
                type: "VARCHAR2(11)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "NUMBER(11)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "cpf",
                table: "ContaCorrente",
                type: "NUMBER(11)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR2(11)");
        }
    }
}
