using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCamposComunidade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoConvite",
                table: "Comunidades",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "Comunidades",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Comunidades_CodigoConvite",
                table: "Comunidades",
                column: "CodigoConvite",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comunidades_PublicId",
                table: "Comunidades",
                column: "PublicId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comunidades_CodigoConvite",
                table: "Comunidades");

            migrationBuilder.DropIndex(
                name: "IX_Comunidades_PublicId",
                table: "Comunidades");

            migrationBuilder.DropColumn(
                name: "CodigoConvite",
                table: "Comunidades");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Comunidades");
        }
    }
}
