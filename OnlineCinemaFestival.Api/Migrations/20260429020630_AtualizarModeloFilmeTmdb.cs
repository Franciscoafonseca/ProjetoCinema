using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarModeloFilmeTmdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassificacaoMedia",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "Popularidade",
                table: "Filmes");

            migrationBuilder.RenameColumn(
                name: "TrailerUrl",
                table: "Filmes",
                newName: "DataLancamento");

            migrationBuilder.RenameColumn(
                name: "Realizador",
                table: "Filmes",
                newName: "Classificacao");

            migrationBuilder.RenameColumn(
                name: "PosterUrl",
                table: "Filmes",
                newName: "CapaUrl");

            migrationBuilder.AddColumn<int>(
                name: "TmdbId",
                table: "Filmes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TmdbId",
                table: "Filmes");

            migrationBuilder.RenameColumn(
                name: "DataLancamento",
                table: "Filmes",
                newName: "TrailerUrl");

            migrationBuilder.RenameColumn(
                name: "Classificacao",
                table: "Filmes",
                newName: "Realizador");

            migrationBuilder.RenameColumn(
                name: "CapaUrl",
                table: "Filmes",
                newName: "PosterUrl");

            migrationBuilder.AddColumn<double>(
                name: "ClassificacaoMedia",
                table: "Filmes",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Popularidade",
                table: "Filmes",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
