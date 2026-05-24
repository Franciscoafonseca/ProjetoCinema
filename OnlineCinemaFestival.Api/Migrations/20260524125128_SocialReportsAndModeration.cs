using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class SocialReportsAndModeration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstadoModeracao",
                table: "Comentarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModeradoEm",
                table: "Comentarios",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModeradoPorUtilizadorId",
                table: "Comentarios",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReportesUtilizadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UtilizadorReportadoId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReportadoPorUtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Motivo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportesUtilizadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportesUtilizadores_Utilizadores_ReportadoPorUtilizadorId",
                        column: x => x.ReportadoPorUtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportesUtilizadores_Utilizadores_UtilizadorReportadoId",
                        column: x => x.UtilizadorReportadoId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportesUtilizadores_ReportadoPorUtilizadorId",
                table: "ReportesUtilizadores",
                column: "ReportadoPorUtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportesUtilizadores_UtilizadorReportadoId_ReportadoPorUtilizadorId",
                table: "ReportesUtilizadores",
                columns: new[] { "UtilizadorReportadoId", "ReportadoPorUtilizadorId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportesUtilizadores");

            migrationBuilder.DropColumn(
                name: "EstadoModeracao",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "ModeradoEm",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "ModeradoPorUtilizadorId",
                table: "Comentarios");
        }
    }
}
