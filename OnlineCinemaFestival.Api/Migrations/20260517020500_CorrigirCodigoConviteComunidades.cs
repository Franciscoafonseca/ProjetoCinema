using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirCodigoConviteComunidades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoConvite",
                table: "Comunidades",
                type: "TEXT",
                nullable: true
            );

            migrationBuilder.Sql(
                """
                UPDATE "Comunidades"
                SET "CodigoConvite" = UPPER(HEX(RANDOMBLOB(4)))
                WHERE "CodigoConvite" IS NULL OR "CodigoConvite" = '';
                """
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "CodigoConvite", table: "Comunidades");
        }
    }
}
