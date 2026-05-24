using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class RewardsRecomendacoesCatalogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChaveAcao",
                table: "RewardsTransacoes",
                type: "TEXT",
                maxLength: 160,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                "UPDATE RewardsTransacoes SET ChaveAcao = 'historico:' || Id WHERE ChaveAcao = ''"
            );

            migrationBuilder.CreateIndex(
                name: "IX_RewardsTransacoes_UtilizadorId_ChaveAcao",
                table: "RewardsTransacoes",
                columns: new[] { "UtilizadorId", "ChaveAcao" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RewardsTransacoes_UtilizadorId_ChaveAcao",
                table: "RewardsTransacoes");

            migrationBuilder.DropColumn(
                name: "ChaveAcao",
                table: "RewardsTransacoes");
        }
    }
}
