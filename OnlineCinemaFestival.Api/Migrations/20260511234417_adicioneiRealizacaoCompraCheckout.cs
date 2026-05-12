using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class adicioneiRealizacaoCompraCheckout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Referencia = table.Column<string>(type: "TEXT", nullable: false),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    CriadaEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PagaEm = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Compras_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcessosUtilizador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    AcessoId = table.Column<int>(type: "INTEGER", nullable: false),
                    CompraId = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoAcesso = table.Column<int>(type: "INTEGER", nullable: false),
                    SessaoId = table.Column<int>(type: "INTEGER", nullable: true),
                    FestivalId = table.Column<int>(type: "INTEGER", nullable: true),
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: true),
                    InicioValidade = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FimValidade = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcessosUtilizador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcessosUtilizador_Acessos_AcessoId",
                        column: x => x.AcessoId,
                        principalTable: "Acessos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcessosUtilizador_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcessosUtilizador_Festivals_FestivalId",
                        column: x => x.FestivalId,
                        principalTable: "Festivals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcessosUtilizador_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcessosUtilizador_Sessoes_SessaoId",
                        column: x => x.SessaoId,
                        principalTable: "Sessoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcessosUtilizador_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItensCompra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompraId = table.Column<int>(type: "INTEGER", nullable: false),
                    AcessoId = table.Column<int>(type: "INTEGER", nullable: false),
                    NomeAcesso = table.Column<string>(type: "TEXT", nullable: false),
                    TipoAcesso = table.Column<int>(type: "INTEGER", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    Subtotal = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensCompra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensCompra_Acessos_AcessoId",
                        column: x => x.AcessoId,
                        principalTable: "Acessos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItensCompra_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcessosUtilizador_AcessoId",
                table: "AcessosUtilizador",
                column: "AcessoId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessosUtilizador_CompraId",
                table: "AcessosUtilizador",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessosUtilizador_FestivalId",
                table: "AcessosUtilizador",
                column: "FestivalId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessosUtilizador_FilmeId",
                table: "AcessosUtilizador",
                column: "FilmeId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessosUtilizador_SessaoId",
                table: "AcessosUtilizador",
                column: "SessaoId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessosUtilizador_UtilizadorId_AcessoId",
                table: "AcessosUtilizador",
                columns: new[] { "UtilizadorId", "AcessoId" });

            migrationBuilder.CreateIndex(
                name: "IX_Compras_Referencia",
                table: "Compras",
                column: "Referencia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Compras_UtilizadorId",
                table: "Compras",
                column: "UtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensCompra_AcessoId",
                table: "ItensCompra",
                column: "AcessoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensCompra_CompraId",
                table: "ItensCompra",
                column: "CompraId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcessosUtilizador");

            migrationBuilder.DropTable(
                name: "ItensCompra");

            migrationBuilder.DropTable(
                name: "Compras");
        }
    }
}
