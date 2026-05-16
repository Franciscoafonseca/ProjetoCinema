using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class BackendFuncionalidadesPrincipais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "HoraFim",
                table: "SessaoFilmes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HoraInicio",
                table: "SessaoFilmes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AtoresPrincipais",
                table: "Filmes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AvaliacaoTmdb",
                table: "Filmes",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DuracaoMinutos",
                table: "Filmes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Premios",
                table: "Filmes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Realizador",
                table: "Filmes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TituloOriginal",
                table: "Filmes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TmdbReviewsJson",
                table: "Filmes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Premios",
                table: "Festivals",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Texto",
                table: "Avaliacoes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "FilmeGeneros",
                columns: table => new
                {
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: false),
                    GeneroId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmeGeneros", x => new { x.FilmeId, x.GeneroId });
                    table.ForeignKey(
                        name: "FK_FilmeGeneros_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilmeGeneros_Generos_GeneroId",
                        column: x => x.GeneroId,
                        principalTable: "Generos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilmeGeneros_GeneroId",
                table: "FilmeGeneros",
                column: "GeneroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilmeGeneros");

            migrationBuilder.DropColumn(
                name: "HoraFim",
                table: "SessaoFilmes");

            migrationBuilder.DropColumn(
                name: "HoraInicio",
                table: "SessaoFilmes");

            migrationBuilder.DropColumn(
                name: "AtoresPrincipais",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "AvaliacaoTmdb",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "DuracaoMinutos",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "Premios",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "Realizador",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "TituloOriginal",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "TmdbReviewsJson",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "Premios",
                table: "Festivals");

            migrationBuilder.DropColumn(
                name: "Texto",
                table: "Avaliacoes");
        }
    }
}
