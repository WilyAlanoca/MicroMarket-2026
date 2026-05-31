using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroMarket.Migrations
{
    /// <inheritdoc />
    public partial class DetalleVentas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleVenta_Productos_ProductoId",
                table: "DetalleVenta");

            migrationBuilder.DropForeignKey(
                name: "FK_DetalleVenta_Ventas_VentaId",
                table: "DetalleVenta");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Productos_ProductoId",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_ProductoId",
                table: "Ventas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DetalleVenta",
                table: "DetalleVenta");

            migrationBuilder.DropColumn(
                name: "ProductoId",
                table: "Ventas");

            migrationBuilder.RenameTable(
                name: "DetalleVenta",
                newName: "DetalleVentas");

            migrationBuilder.RenameIndex(
                name: "IX_DetalleVenta_VentaId",
                table: "DetalleVentas",
                newName: "IX_DetalleVentas_VentaId");

            migrationBuilder.RenameIndex(
                name: "IX_DetalleVenta_ProductoId",
                table: "DetalleVentas",
                newName: "IX_DetalleVentas_ProductoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DetalleVentas",
                table: "DetalleVentas",
                column: "DetalleVentaId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleVentas_Productos_ProductoId",
                table: "DetalleVentas",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleVentas_Ventas_VentaId",
                table: "DetalleVentas",
                column: "VentaId",
                principalTable: "Ventas",
                principalColumn: "VentaId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleVentas_Productos_ProductoId",
                table: "DetalleVentas");

            migrationBuilder.DropForeignKey(
                name: "FK_DetalleVentas_Ventas_VentaId",
                table: "DetalleVentas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DetalleVentas",
                table: "DetalleVentas");

            migrationBuilder.RenameTable(
                name: "DetalleVentas",
                newName: "DetalleVenta");

            migrationBuilder.RenameIndex(
                name: "IX_DetalleVentas_VentaId",
                table: "DetalleVenta",
                newName: "IX_DetalleVenta_VentaId");

            migrationBuilder.RenameIndex(
                name: "IX_DetalleVentas_ProductoId",
                table: "DetalleVenta",
                newName: "IX_DetalleVenta_ProductoId");

            migrationBuilder.AddColumn<int>(
                name: "ProductoId",
                table: "Ventas",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DetalleVenta",
                table: "DetalleVenta",
                column: "DetalleVentaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_ProductoId",
                table: "Ventas",
                column: "ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleVenta_Productos_ProductoId",
                table: "DetalleVenta",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleVenta_Ventas_VentaId",
                table: "DetalleVenta",
                column: "VentaId",
                principalTable: "Ventas",
                principalColumn: "VentaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Productos_ProductoId",
                table: "Ventas",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "ProductoId");
        }
    }
}
