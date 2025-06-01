using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlGastosApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTipoAndDescripcionFromFondos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "FondosMonetarios");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "FondosMonetarios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "FondosMonetarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "FondosMonetarios",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
