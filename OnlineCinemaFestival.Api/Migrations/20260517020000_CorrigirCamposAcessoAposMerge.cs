using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirCamposAcessoAposMerge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrecoPago",
                table: "Acessos",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m
            );

            migrationBuilder.AddColumn<string>(
                name: "UtilizadorId",
                table: "Acessos",
                type: "TEXT",
                nullable: false,
                defaultValue: string.Empty
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "Validade",
                table: "Acessos",
                type: "TEXT",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "PrecoPago", table: "Acessos");
            migrationBuilder.DropColumn(name: "UtilizadorId", table: "Acessos");
            migrationBuilder.DropColumn(name: "Validade", table: "Acessos");
        }
    }
}
