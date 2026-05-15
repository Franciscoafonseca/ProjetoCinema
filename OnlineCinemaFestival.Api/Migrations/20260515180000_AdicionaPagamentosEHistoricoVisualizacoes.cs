using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaPagamentosEHistoricoVisualizacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConteudoLocalPath",
                table: "Filmes",
                type: "TEXT",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "TrailerUrl",
                table: "Filmes",
                type: "TEXT",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "Filmes",
                type: "TEXT",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "Pagamentos",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompraId = table.Column<int>(type: "INTEGER", nullable: false),
                    Referencia = table.Column<string>(type: "TEXT", nullable: false),
                    Valor = table.Column<decimal>(
                        type: "TEXT",
                        precision: 10,
                        scale: 2,
                        nullable: false
                    ),
                    Metodo = table.Column<string>(type: "TEXT", nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProcessadoEm = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Mensagem = table.Column<string>(type: "TEXT", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagamentos_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Visualizacoes",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: false),
                    SessaoId = table.Column<int>(type: "INTEGER", nullable: true),
                    FestivalId = table.Column<int>(type: "INTEGER", nullable: true),
                    TipoConteudo = table.Column<string>(type: "TEXT", nullable: false),
                    UrlVisualizacao = table.Column<string>(type: "TEXT", nullable: true),
                    VisualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visualizacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Visualizacoes_Festivals_FestivalId",
                        column: x => x.FestivalId,
                        principalTable: "Festivals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_Visualizacoes_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_Visualizacoes_Sessoes_SessaoId",
                        column: x => x.SessaoId,
                        principalTable: "Sessoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_Visualizacoes_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_CompraId",
                table: "Pagamentos",
                column: "CompraId",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_Referencia",
                table: "Pagamentos",
                column: "Referencia",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Visualizacoes_FestivalId",
                table: "Visualizacoes",
                column: "FestivalId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Visualizacoes_FilmeId",
                table: "Visualizacoes",
                column: "FilmeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Visualizacoes_SessaoId",
                table: "Visualizacoes",
                column: "SessaoId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Visualizacoes_UtilizadorId_VisualizadoEm",
                table: "Visualizacoes",
                columns: new[] { "UtilizadorId", "VisualizadoEm" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Pagamentos");

            migrationBuilder.DropTable(name: "Visualizacoes");

            migrationBuilder.DropColumn(name: "ConteudoLocalPath", table: "Filmes");

            migrationBuilder.DropColumn(name: "TrailerUrl", table: "Filmes");

            migrationBuilder.DropColumn(name: "VideoUrl", table: "Filmes");
        }
    }
}
