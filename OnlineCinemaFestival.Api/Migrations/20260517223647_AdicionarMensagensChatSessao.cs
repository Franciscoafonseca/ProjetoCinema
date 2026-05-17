using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarMensagensChatSessao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MensagensChatSessao",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    SessaoId = table.Column<int>(type: "INTEGER", nullable: false),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Texto = table.Column<string>(type: "TEXT", maxLength: 600, nullable: false),
                    EnviadaEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Removida = table.Column<bool>(type: "INTEGER", nullable: false),
                    RemovidaPorModeracao = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensagensChatSessao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensagensChatSessao_Sessoes_SessaoId",
                        column: x => x.SessaoId,
                        principalTable: "Sessoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MensagensChatSessao_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MensagensChatSessao_SessaoId_EnviadaEm",
                table: "MensagensChatSessao",
                columns: new[] { "SessaoId", "EnviadaEm" });

            migrationBuilder.CreateIndex(
                name: "IX_MensagensChatSessao_UtilizadorId",
                table: "MensagensChatSessao",
                column: "UtilizadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MensagensChatSessao");
        }
    }
}
