using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlGastosApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFondoIdFromDepositos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Depositos_FondosMonetarios_FondoMonetarioId",
                table: "Depositos");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosGastos_FondosMonetarios_FondoId",
                table: "RegistrosGastos");

            migrationBuilder.DropColumn(
                name: "FondoId",
                table: "Depositos");

            migrationBuilder.RenameColumn(
                name: "FondoId",
                table: "RegistrosGastos",
                newName: "FondoMonetarioId");

            migrationBuilder.RenameIndex(
                name: "IX_RegistrosGastos_FondoId",
                table: "RegistrosGastos",
                newName: "IX_RegistrosGastos_FondoMonetarioId");

            migrationBuilder.AlterColumn<int>(
                name: "FondoMonetarioId",
                table: "Depositos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Depositos_FondosMonetarios_FondoMonetarioId",
                table: "Depositos",
                column: "FondoMonetarioId",
                principalTable: "FondosMonetarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosGastos_FondosMonetarios_FondoMonetarioId",
                table: "RegistrosGastos",
                column: "FondoMonetarioId",
                principalTable: "FondosMonetarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Depositos_FondosMonetarios_FondoMonetarioId",
                table: "Depositos");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosGastos_FondosMonetarios_FondoMonetarioId",
                table: "RegistrosGastos");

            migrationBuilder.RenameColumn(
                name: "FondoMonetarioId",
                table: "RegistrosGastos",
                newName: "FondoId");

            migrationBuilder.RenameIndex(
                name: "IX_RegistrosGastos_FondoMonetarioId",
                table: "RegistrosGastos",
                newName: "IX_RegistrosGastos_FondoId");

            migrationBuilder.AlterColumn<int>(
                name: "FondoMonetarioId",
                table: "Depositos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "FondoId",
                table: "Depositos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Depositos_FondosMonetarios_FondoMonetarioId",
                table: "Depositos",
                column: "FondoMonetarioId",
                principalTable: "FondosMonetarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosGastos_FondosMonetarios_FondoId",
                table: "RegistrosGastos",
                column: "FondoId",
                principalTable: "FondosMonetarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
