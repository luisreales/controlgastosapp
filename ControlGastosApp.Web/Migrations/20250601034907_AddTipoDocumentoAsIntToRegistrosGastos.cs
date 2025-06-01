using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlGastosApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoDocumentoAsIntToRegistrosGastos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add a new temporary column TipoDocumentoTemp of type int
            migrationBuilder.AddColumn<int>(
                name: "TipoDocumentoTemp",
                table: "RegistrosGastos",
                type: "int",
                nullable: true); // Allow nulls temporarily

            // 2. Copy data from the string column to the new int column with conversion
            migrationBuilder.Sql(
                "UPDATE RegistrosGastos SET TipoDocumentoTemp = CASE " +
                "WHEN TipoDocumento = 'Factura' THEN 1 " +
                "WHEN TipoDocumento = 'Recibo' THEN 2 " +
                "WHEN TipoDocumento = 'Ticket' THEN 3 " +
                "WHEN TipoDocumento = 'Comprobante' THEN 4 " +
                "WHEN TipoDocumento = 'Otro' THEN 5 " +
                "ELSE 5 END"); // Default to Otro if unknown

            // 3. Drop the original string column
            migrationBuilder.DropColumn(
                name: "TipoDocumento",
                table: "RegistrosGastos");

            // 4. Rename the new int column to TipoDocumento
            migrationBuilder.RenameColumn(
                name: "TipoDocumentoTemp",
                table: "RegistrosGastos",
                newName: "TipoDocumento");

            // 5. Make the column non-nullable (if needed, although AddColumn defaults to nullable=true)
            // The final AlterColumn<int> will handle making it non-nullable as in the original migration.
             migrationBuilder.AlterColumn<int>(
                name: "TipoDocumento",
                table: "RegistrosGastos",
                type: "int",
                nullable: false, // Set to false as required
                oldClrType: typeof(int), // old type is now int after rename
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert logic in Down method (simplified as we are focusing on Up)
            migrationBuilder.AddColumn<string>(
                name: "TipoDocumentoTemp",
                table: "RegistrosGastos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE RegistrosGastos SET TipoDocumentoTemp = CASE " +
                "WHEN TipoDocumento = 1 THEN 'Factura' " +
                "WHEN TipoDocumento = 2 THEN 'Recibo' " +
                "WHEN TipoDocumento = 3 THEN 'Ticket' " +
                "WHEN TipoDocumento = 4 THEN 'Comprobante' " +
                "WHEN TipoDocumento = 5 THEN 'Otro' " +
                "ELSE 'Otro' END");

            migrationBuilder.DropColumn(
                name: "TipoDocumento",
                table: "RegistrosGastos");

            migrationBuilder.RenameColumn(
                name: "TipoDocumentoTemp",
                table: "RegistrosGastos",
                newName: "TipoDocumento");

             migrationBuilder.AlterColumn<string>(
                name: "TipoDocumento",
                table: "RegistrosGastos",
                type: "nvarchar(max)",
                nullable: false, // Set to false as required
                oldClrType: typeof(string), // old type is now string after rename
                oldType: "nvarchar(max)");
        }
    }
}
