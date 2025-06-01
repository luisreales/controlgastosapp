using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlGastosApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FondosMonetarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Saldo = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FondosMonetarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposGasto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposGasto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Depositos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FondoId = table.Column<int>(type: "int", nullable: false),
                    FondoMonetarioId = table.Column<int>(type: "int", nullable: true),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Depositos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Depositos_FondosMonetarios_FondoMonetarioId",
                        column: x => x.FondoMonetarioId,
                        principalTable: "FondosMonetarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RegistrosGastos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FondoId = table.Column<int>(type: "int", nullable: false),
                    Comercio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoDocumento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosGastos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosGastos_FondosMonetarios_FondoId",
                        column: x => x.FondoId,
                        principalTable: "FondosMonetarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gasto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoGastoId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gasto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gasto_TiposGasto_TipoGastoId",
                        column: x => x.TipoGastoId,
                        principalTable: "TiposGasto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Presupuestos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoGastoId = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presupuestos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Presupuestos_TiposGasto_TipoGastoId",
                        column: x => x.TipoGastoId,
                        principalTable: "TiposGasto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesGastos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistroGastoId = table.Column<int>(type: "int", nullable: false),
                    TipoGastoId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesGastos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesGastos_RegistrosGastos_RegistroGastoId",
                        column: x => x.RegistroGastoId,
                        principalTable: "RegistrosGastos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesGastos_TiposGasto_TipoGastoId",
                        column: x => x.TipoGastoId,
                        principalTable: "TiposGasto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Depositos_FondoMonetarioId",
                table: "Depositos",
                column: "FondoMonetarioId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesGastos_RegistroGastoId",
                table: "DetallesGastos",
                column: "RegistroGastoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesGastos_TipoGastoId",
                table: "DetallesGastos",
                column: "TipoGastoId");

            migrationBuilder.CreateIndex(
                name: "IX_Gasto_TipoGastoId",
                table: "Gasto",
                column: "TipoGastoId");

            migrationBuilder.CreateIndex(
                name: "IX_Presupuestos_TipoGastoId",
                table: "Presupuestos",
                column: "TipoGastoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosGastos_FondoId",
                table: "RegistrosGastos",
                column: "FondoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Depositos");

            migrationBuilder.DropTable(
                name: "DetallesGastos");

            migrationBuilder.DropTable(
                name: "Gasto");

            migrationBuilder.DropTable(
                name: "Presupuestos");

            migrationBuilder.DropTable(
                name: "RegistrosGastos");

            migrationBuilder.DropTable(
                name: "TiposGasto");

            migrationBuilder.DropTable(
                name: "FondosMonetarios");
        }
    }
}
