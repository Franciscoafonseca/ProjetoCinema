using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class RecriarModeloAposMerge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""ComprasItens"";");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""ef_temp_RewardsTransacoes"";");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""ef_temp_Rewards"";");

            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""RewardsTransacoes"";");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""Rewards"";");

            migrationBuilder.CreateTable(
                name: "Rewards",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Pontos = table.Column<int>(type: "INTEGER", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rewards", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "RewardsTransacoes",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Pontos = table.Column<int>(type: "INTEGER", nullable: false),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Motivo = table.Column<string>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardsTransacoes", x => x.Id);
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""RewardsTransacoes"";");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""Rewards"";");

            migrationBuilder.CreateTable(
                name: "Rewards",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UtilizadorId = table.Column<string>(type: "TEXT", nullable: false),
                    Pontos = table.Column<int>(type: "INTEGER", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rewards", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "RewardsTransacoes",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UtilizadorId = table.Column<string>(type: "TEXT", nullable: false),
                    Pontos = table.Column<int>(type: "INTEGER", nullable: false),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Motivo = table.Column<string>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardsTransacoes", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "ComprasItens",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompraId = table.Column<int>(type: "INTEGER", nullable: false),
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: true),
                    PrecoPago = table.Column<decimal>(type: "TEXT", nullable: false),
                    SessaoId = table.Column<int>(type: "INTEGER", nullable: true),
                    TipoAcesso = table.Column<int>(type: "INTEGER", nullable: false),
                    Validade = table.Column<DateTime>(type: "TEXT", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprasItens", x => x.Id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_ComprasItens_CompraId",
                table: "ComprasItens",
                column: "CompraId"
            );
        }
    }
}
