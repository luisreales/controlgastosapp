using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlGastosApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddDescripcionToTipoGasto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "TiposGasto",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "TiposGasto");
        }
    }
}
