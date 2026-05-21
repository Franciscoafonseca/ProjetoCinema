using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirBackendSessaoFilmeVideoPremios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConteudoLocalPath",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "DuracaoVideoSegundos",
                table: "Filmes");

            migrationBuilder.AlterColumn<int>(
                name: "PublicadoPorUtilizadorId",
                table: "ResultadosPremiosFestival",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PublicadoPorUtilizadorId",
                table: "ResultadosPremiosFestival",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConteudoLocalPath",
                table: "Filmes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DuracaoVideoSegundos",
                table: "Filmes",
                type: "INTEGER",
                nullable: true);
        }
    }
}
