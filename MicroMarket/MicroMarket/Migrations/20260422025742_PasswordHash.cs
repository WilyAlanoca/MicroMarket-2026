using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroMarket.Migrations
{
    /// <inheritdoc />
    public partial class PasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contraseña",
                table: "Vendedores");

            migrationBuilder.RenameColumn(
                name: "Celular",
                table: "Vendedores",
                newName: "Telefono");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Vendedores",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Vendedores");

            migrationBuilder.RenameColumn(
                name: "Telefono",
                table: "Vendedores",
                newName: "Celular");

            migrationBuilder.AddColumn<string>(
                name: "Contraseña",
                table: "Vendedores",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
