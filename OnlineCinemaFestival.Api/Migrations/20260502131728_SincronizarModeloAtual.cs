using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class SincronizarModeloAtual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Filmes_FilmeId",
                table: "Comentarios"
            );

            migrationBuilder.RenameColumn(
                name: "FilmeId",
                table: "Comentarios",
                newName: "ComunidadeId"
            );

            migrationBuilder.RenameColumn(name: "Data", table: "Comentarios", newName: "CriadoEm");

            migrationBuilder.RenameIndex(
                name: "IX_Comentarios_FilmeId",
                table: "Comentarios",
                newName: "IX_Comentarios_ComunidadeId"
            );

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Comentarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true
            );

            migrationBuilder.CreateTable(
                name: "Generos",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generos", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Utilizadores",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Nationality = table.Column<string>(
                        type: "TEXT",
                        maxLength: 80,
                        nullable: false
                    ),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizadores", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Comunidades",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comunidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comunidades_Utilizadores_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ListasPessoais",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Description = table.Column<string>(
                        type: "TEXT",
                        maxLength: 500,
                        nullable: false
                    ),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasPessoais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListasPessoais_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "PerfisUtilizador",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Bio = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ProfileImageUrl = table.Column<string>(
                        type: "TEXT",
                        maxLength: 300,
                        nullable: false
                    ),
                    Location = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfisUtilizador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerfisUtilizador_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "UtilizadoresGenerosFavoritos",
                columns: table => new
                {
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    GeneroId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_UtilizadoresGenerosFavoritos",
                        x => new { x.UtilizadorId, x.GeneroId }
                    );
                    table.ForeignKey(
                        name: "FK_UtilizadoresGenerosFavoritos_Generos_GeneroId",
                        column: x => x.GeneroId,
                        principalTable: "Generos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_UtilizadoresGenerosFavoritos_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ComunidadeMembros",
                columns: table => new
                {
                    ComunidadeId = table.Column<int>(type: "INTEGER", nullable: false),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ComunidadeMembros",
                        x => new { x.ComunidadeId, x.UtilizadorId }
                    );
                    table.ForeignKey(
                        name: "FK_ComunidadeMembros_Comunidades_ComunidadeId",
                        column: x => x.ComunidadeId,
                        principalTable: "Comunidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_ComunidadeMembros_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ListaPessoalItems",
                columns: table => new
                {
                    ListaPessoalId = table.Column<int>(type: "INTEGER", nullable: false),
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ListaPessoalItems",
                        x => new { x.ListaPessoalId, x.FilmeId }
                    );
                    table.ForeignKey(
                        name: "FK_ListaPessoalItems_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_ListaPessoalItems_ListasPessoais_ListaPessoalId",
                        column: x => x.ListaPessoalId,
                        principalTable: "ListasPessoais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_UsuarioId",
                table: "Comentarios",
                column: "UsuarioId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacoes_UsuarioId_FilmeId",
                table: "Avaliacoes",
                columns: new[] { "UsuarioId", "FilmeId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_ComunidadeMembros_UtilizadorId",
                table: "ComunidadeMembros",
                column: "UtilizadorId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Comunidades_CreatedByUserId",
                table: "Comunidades",
                column: "CreatedByUserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Generos_Name",
                table: "Generos",
                column: "Name",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_ListaPessoalItems_FilmeId",
                table: "ListaPessoalItems",
                column: "FilmeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ListasPessoais_UtilizadorId",
                table: "ListasPessoais",
                column: "UtilizadorId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PerfisUtilizador_UtilizadorId",
                table: "PerfisUtilizador",
                column: "UtilizadorId",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Utilizadores_Email",
                table: "Utilizadores",
                column: "Email",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_UtilizadoresGenerosFavoritos_GeneroId",
                table: "UtilizadoresGenerosFavoritos",
                column: "GeneroId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacoes_Utilizadores_UsuarioId",
                table: "Avaliacoes",
                column: "UsuarioId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Comunidades_ComunidadeId",
                table: "Comentarios",
                column: "ComunidadeId",
                principalTable: "Comunidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Utilizadores_UsuarioId",
                table: "Comentarios",
                column: "UsuarioId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacoes_Utilizadores_UsuarioId",
                table: "Avaliacoes"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Comunidades_ComunidadeId",
                table: "Comentarios"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Utilizadores_UsuarioId",
                table: "Comentarios"
            );

            migrationBuilder.DropTable(name: "ComunidadeMembros");

            migrationBuilder.DropTable(name: "ListaPessoalItems");

            migrationBuilder.DropTable(name: "PerfisUtilizador");

            migrationBuilder.DropTable(name: "UtilizadoresGenerosFavoritos");

            migrationBuilder.DropTable(name: "Comunidades");

            migrationBuilder.DropTable(name: "ListasPessoais");

            migrationBuilder.DropTable(name: "Generos");

            migrationBuilder.DropTable(name: "Utilizadores");

            migrationBuilder.DropIndex(name: "IX_Comentarios_UsuarioId", table: "Comentarios");

            migrationBuilder.DropIndex(
                name: "IX_Avaliacoes_UsuarioId_FilmeId",
                table: "Avaliacoes"
            );

            migrationBuilder.RenameColumn(name: "CriadoEm", table: "Comentarios", newName: "Data");

            migrationBuilder.RenameColumn(
                name: "ComunidadeId",
                table: "Comentarios",
                newName: "FilmeId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_Comentarios_ComunidadeId",
                table: "Comentarios",
                newName: "IX_Comentarios_FilmeId"
            );

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Comentarios",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Filmes_FilmeId",
                table: "Comentarios",
                column: "FilmeId",
                principalTable: "Filmes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
