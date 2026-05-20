using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class ExpandirPerfilAutenticacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Utilizadores",
                type: "TEXT",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "PerfisUtilizador",
                type: "TEXT",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "PerfisUtilizador",
                type: "TEXT",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Utilizadores_PhoneNumber",
                table: "Utilizadores",
                column: "PhoneNumber",
                unique: true,
                filter: "\"PhoneNumber\" <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Utilizadores_PhoneNumber",
                table: "Utilizadores");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Utilizadores");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "PerfisUtilizador");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "PerfisUtilizador");
        }
    }
}
