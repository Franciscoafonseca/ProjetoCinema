using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirPublicIdComunidades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "Comunidades",
                type: "TEXT",
                nullable: false,
                defaultValue: Guid.Empty
            );

            migrationBuilder.Sql(
                """
                UPDATE "Comunidades"
                SET "PublicId" =
                    lower(hex(randomblob(4))) || '-' ||
                    lower(hex(randomblob(2))) || '-' ||
                    lower(hex(randomblob(2))) || '-' ||
                    lower(hex(randomblob(2))) || '-' ||
                    lower(hex(randomblob(6)))
                WHERE "PublicId" = '00000000-0000-0000-0000-000000000000';
                """
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "PublicId", table: "Comunidades");
        }
    }
}
