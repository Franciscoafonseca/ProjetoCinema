using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarPremiosFestival : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PremiosFestival",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FestivalId = table.Column<int>(type: "INTEGER", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    DataAberturaVotacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataFechoVotacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EstadoPremio = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremiosFestival", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PremiosFestival_Festivals_FestivalId",
                        column: x => x.FestivalId,
                        principalTable: "Festivals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResultadosPremiosFestival",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PremioFestivalId = table.Column<int>(type: "INTEGER", nullable: false),
                    FilmeIdVencedor = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalVotos = table.Column<int>(type: "INTEGER", nullable: false),
                    PublicadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PublicadoPorUtilizadorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultadosPremiosFestival", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultadosPremiosFestival_Filmes_FilmeIdVencedor",
                        column: x => x.FilmeIdVencedor,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultadosPremiosFestival_PremiosFestival_PremioFestivalId",
                        column: x => x.PremioFestivalId,
                        principalTable: "PremiosFestival",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResultadosPremiosFestival_Utilizadores_PublicadoPorUtilizadorId",
                        column: x => x.PublicadoPorUtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VotosPremiosFestival",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PremioFestivalId = table.Column<int>(type: "INTEGER", nullable: false),
                    FestivalId = table.Column<int>(type: "INTEGER", nullable: false),
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: false),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataVoto = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotosPremiosFestival", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VotosPremiosFestival_Festivals_FestivalId",
                        column: x => x.FestivalId,
                        principalTable: "Festivals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VotosPremiosFestival_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VotosPremiosFestival_PremiosFestival_PremioFestivalId",
                        column: x => x.PremioFestivalId,
                        principalTable: "PremiosFestival",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VotosPremiosFestival_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PremiosFestival_FestivalId",
                table: "PremiosFestival",
                column: "FestivalId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultadosPremiosFestival_FilmeIdVencedor",
                table: "ResultadosPremiosFestival",
                column: "FilmeIdVencedor");

            migrationBuilder.CreateIndex(
                name: "IX_ResultadosPremiosFestival_PremioFestivalId",
                table: "ResultadosPremiosFestival",
                column: "PremioFestivalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResultadosPremiosFestival_PublicadoPorUtilizadorId",
                table: "ResultadosPremiosFestival",
                column: "PublicadoPorUtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_VotosPremiosFestival_FestivalId",
                table: "VotosPremiosFestival",
                column: "FestivalId");

            migrationBuilder.CreateIndex(
                name: "IX_VotosPremiosFestival_FilmeId",
                table: "VotosPremiosFestival",
                column: "FilmeId");

            migrationBuilder.CreateIndex(
                name: "IX_VotosPremiosFestival_PremioFestivalId_UtilizadorId",
                table: "VotosPremiosFestival",
                columns: new[] { "PremioFestivalId", "UtilizadorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VotosPremiosFestival_UtilizadorId",
                table: "VotosPremiosFestival",
                column: "UtilizadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResultadosPremiosFestival");

            migrationBuilder.DropTable(
                name: "VotosPremiosFestival");

            migrationBuilder.DropTable(
                name: "PremiosFestival");
        }
    }
}
