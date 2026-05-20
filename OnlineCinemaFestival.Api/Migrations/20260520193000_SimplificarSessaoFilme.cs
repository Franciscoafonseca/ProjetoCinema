using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class SimplificarSessaoFilme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FilmeId",
                table: "Sessoes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                """
                UPDATE Sessoes
                SET FilmeId = (
                    SELECT FilmeId
                    FROM SessaoFilmes
                    WHERE SessaoFilmes.SessaoId = Sessoes.Id
                    ORDER BY Ordem, FilmeId
                    LIMIT 1
                )
                WHERE EXISTS (
                    SELECT 1
                    FROM SessaoFilmes
                    WHERE SessaoFilmes.SessaoId = Sessoes.Id
                );
                """
            );

            migrationBuilder.Sql(
                """
                UPDATE Sessoes
                SET FilmeId = (SELECT Id FROM Filmes ORDER BY Id LIMIT 1)
                WHERE FilmeId = 0
                  AND EXISTS (SELECT 1 FROM Filmes);
                """
            );

            migrationBuilder.AddColumn<string>(
                name: "Entidade",
                table: "Pagamentos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessoes_FilmeId",
                table: "Sessoes",
                column: "FilmeId");

            migrationBuilder.DropTable(name: "SessaoFilmes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SessaoFilmes",
                columns: table => new
                {
                    SessaoId = table.Column<int>(type: "INTEGER", nullable: false),
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Ordem = table.Column<int>(type: "INTEGER", nullable: false),
                    DuracaoSegundos = table.Column<int>(type: "INTEGER", nullable: true),
                    InicioOffsetSegundos = table.Column<int>(
                        type: "INTEGER",
                        nullable: false,
                        defaultValue: 0),
                    IntervaloAposSegundos = table.Column<int>(
                        type: "INTEGER",
                        nullable: false,
                        defaultValue: 0),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessaoFilmes", x => new { x.SessaoId, x.FilmeId });
                    table.ForeignKey(
                        name: "FK_SessaoFilmes_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessaoFilmes_Sessoes_SessaoId",
                        column: x => x.SessaoId,
                        principalTable: "Sessoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(
                """
                INSERT INTO SessaoFilmes (
                    SessaoId,
                    FilmeId,
                    Ordem,
                    InicioOffsetSegundos,
                    IntervaloAposSegundos
                )
                SELECT Id, FilmeId, 1, 0, 0
                FROM Sessoes
                WHERE FilmeId IS NOT NULL;
                """
            );

            migrationBuilder.CreateIndex(
                name: "IX_SessaoFilmes_FilmeId",
                table: "SessaoFilmes",
                column: "FilmeId");

            migrationBuilder.CreateIndex(
                name: "IX_SessaoFilmes_SessaoId_Ordem",
                table: "SessaoFilmes",
                columns: new[] { "SessaoId", "Ordem" },
                unique: true);

            migrationBuilder.DropIndex(name: "IX_Sessoes_FilmeId", table: "Sessoes");

            migrationBuilder.DropColumn(name: "FilmeId", table: "Sessoes");

            migrationBuilder.DropColumn(name: "Entidade", table: "Pagamentos");
        }
    }
}
