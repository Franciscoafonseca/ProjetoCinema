using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirLigacoesFestivalFilmeSessao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "FestivalFilmes",
                type: "TEXT",
                maxLength: 150,
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAdicao",
                table: "FestivalFilmes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 17, 0, 0, 0, DateTimeKind.Utc)
            );

            migrationBuilder.AddColumn<bool>(
                name: "ElegivelPremiosPublico",
                table: "FestivalFilmes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<string>(
                name: "Secao",
                table: "FestivalFilmes",
                type: "TEXT",
                maxLength: 150,
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "DuracaoSegundos",
                table: "SessaoFilmes",
                type: "INTEGER",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "InicioOffsetSegundos",
                table: "SessaoFilmes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "IntervaloAposSegundos",
                table: "SessaoFilmes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.CreateIndex(
                name: "IX_SessaoFilmes_SessaoId_Ordem",
                table: "SessaoFilmes",
                columns: new[] { "SessaoId", "Ordem" },
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SessaoFilmes_SessaoId_Ordem",
                table: "SessaoFilmes"
            );

            migrationBuilder.DropColumn(name: "Categoria", table: "FestivalFilmes");

            migrationBuilder.DropColumn(name: "DataAdicao", table: "FestivalFilmes");

            migrationBuilder.DropColumn(name: "ElegivelPremiosPublico", table: "FestivalFilmes");

            migrationBuilder.DropColumn(name: "Secao", table: "FestivalFilmes");

            migrationBuilder.DropColumn(name: "DuracaoSegundos", table: "SessaoFilmes");

            migrationBuilder.DropColumn(name: "InicioOffsetSegundos", table: "SessaoFilmes");

            migrationBuilder.DropColumn(name: "IntervaloAposSegundos", table: "SessaoFilmes");
        }
    }
}
