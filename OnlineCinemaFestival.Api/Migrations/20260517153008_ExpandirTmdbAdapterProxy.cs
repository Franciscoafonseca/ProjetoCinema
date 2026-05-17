using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class ExpandirTmdbAdapterProxy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DuracaoVideoSegundos",
                table: "Filmes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoKey",
                table: "Filmes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoProvider",
                table: "Filmes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Pessoas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbPessoaId = table.Column<int>(type: "INTEGER", nullable: true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    ImagemUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pessoas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FilmePessoas",
                columns: table => new
                {
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: false),
                    PessoaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Funcao = table.Column<int>(type: "INTEGER", nullable: false),
                    Personagem = table.Column<string>(type: "TEXT", nullable: true),
                    Ordem = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmePessoas", x => new { x.FilmeId, x.PessoaId, x.Funcao });
                    table.ForeignKey(
                        name: "FK_FilmePessoas_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilmePessoas_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Filmes_TmdbId",
                table: "Filmes",
                column: "TmdbId");

            migrationBuilder.CreateIndex(
                name: "IX_FilmePessoas_PessoaId",
                table: "FilmePessoas",
                column: "PessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_Nome",
                table: "Pessoas",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_TmdbPessoaId",
                table: "Pessoas",
                column: "TmdbPessoaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilmePessoas");

            migrationBuilder.DropTable(
                name: "Pessoas");

            migrationBuilder.DropIndex(
                name: "IX_Filmes_TmdbId",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "DuracaoVideoSegundos",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "VideoKey",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "VideoProvider",
                table: "Filmes");
        }
    }
}
