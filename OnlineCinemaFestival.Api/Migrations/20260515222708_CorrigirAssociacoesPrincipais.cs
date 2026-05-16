using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirAssociacoesPrincipais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcessosUtilizador_Utilizadores_UtilizadorId",
                table: "AcessosUtilizador");

            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacoes_Filmes_FilmeId",
                table: "Avaliacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacoes_Utilizadores_UsuarioId",
                table: "Avaliacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Utilizadores_UsuarioId",
                table: "Comentarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Compras_Utilizadores_UtilizadorId",
                table: "Compras");

            migrationBuilder.DropForeignKey(
                name: "FK_ComunidadeMembros_Utilizadores_UtilizadorId",
                table: "ComunidadeMembros");

            migrationBuilder.DropForeignKey(
                name: "FK_ListaPessoalItems_Filmes_FilmeId",
                table: "ListaPessoalItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SessaoFilmes_Filmes_FilmeId",
                table: "SessaoFilmes");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessoes_Festivals_FestivalId",
                table: "Sessoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Visualizacoes_Utilizadores_UtilizadorId",
                table: "Visualizacoes");

            migrationBuilder.AlterColumn<double>(
                name: "Valor",
                table: "Pagamentos",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<double>(
                name: "Subtotal",
                table: "ItensCompra",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<double>(
                name: "PrecoUnitario",
                table: "ItensCompra",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<double>(
                name: "PrecoUnitario",
                table: "ItensCarrinho",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<double>(
                name: "ValorTotal",
                table: "Compras",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "ComunidadeId",
                table: "Comentarios",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "FilmeId",
                table: "Comentarios",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Preco",
                table: "Acessos",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.Sql(
                """
                Atualizar ListasPessoais SET Tipo = 1 WHERE Name = 'Quero ver' AND Tipo = 0;
                Atualizar ListasPessoais SET Tipo = 2 WHERE Name = 'Vistos' AND Tipo = 0;
                Atualizar ListasPessoais SET Tipo = 3 WHERE Name = 'Favoritos' AND Tipo = 0;
                """
            );

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_FilmeId",
                table: "Comentarios",
                column: "FilmeId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Comentarios_Alvo",
                table: "Comentarios",
                sql: "ComunidadeId IS NOT NULL OR FilmeId IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AcessosUtilizador_Utilizadores_UtilizadorId",
                table: "AcessosUtilizador",
                column: "UtilizadorId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacoes_Filmes_FilmeId",
                table: "Avaliacoes",
                column: "FilmeId",
                principalTable: "Filmes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacoes_Utilizadores_UsuarioId",
                table: "Avaliacoes",
                column: "UsuarioId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Filmes_FilmeId",
                table: "Comentarios",
                column: "FilmeId",
                principalTable: "Filmes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Utilizadores_UsuarioId",
                table: "Comentarios",
                column: "UsuarioId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_Utilizadores_UtilizadorId",
                table: "Compras",
                column: "UtilizadorId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ComunidadeMembros_Utilizadores_UtilizadorId",
                table: "ComunidadeMembros",
                column: "UtilizadorId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ListaPessoalItems_Filmes_FilmeId",
                table: "ListaPessoalItems",
                column: "FilmeId",
                principalTable: "Filmes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SessaoFilmes_Filmes_FilmeId",
                table: "SessaoFilmes",
                column: "FilmeId",
                principalTable: "Filmes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessoes_Festivals_FestivalId",
                table: "Sessoes",
                column: "FestivalId",
                principalTable: "Festivals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Visualizacoes_Utilizadores_UtilizadorId",
                table: "Visualizacoes",
                column: "UtilizadorId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcessosUtilizador_Utilizadores_UtilizadorId",
                table: "AcessosUtilizador");

            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacoes_Filmes_FilmeId",
                table: "Avaliacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacoes_Utilizadores_UsuarioId",
                table: "Avaliacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Filmes_FilmeId",
                table: "Comentarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Utilizadores_UsuarioId",
                table: "Comentarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Compras_Utilizadores_UtilizadorId",
                table: "Compras");

            migrationBuilder.DropForeignKey(
                name: "FK_ComunidadeMembros_Utilizadores_UtilizadorId",
                table: "ComunidadeMembros");

            migrationBuilder.DropForeignKey(
                name: "FK_ListaPessoalItems_Filmes_FilmeId",
                table: "ListaPessoalItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SessaoFilmes_Filmes_FilmeId",
                table: "SessaoFilmes");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessoes_Festivals_FestivalId",
                table: "Sessoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Visualizacoes_Utilizadores_UtilizadorId",
                table: "Visualizacoes");

            migrationBuilder.DropIndex(
                name: "IX_Comentarios_FilmeId",
                table: "Comentarios");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Comentarios_Alvo",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "FilmeId",
                table: "Comentarios");

            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "Pagamentos",
                type: "TEXT",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<decimal>(
                name: "Subtotal",
                table: "ItensCompra",
                type: "TEXT",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecoUnitario",
                table: "ItensCompra",
                type: "TEXT",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecoUnitario",
                table: "ItensCarrinho",
                type: "TEXT",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorTotal",
                table: "Compras",
                type: "TEXT",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<int>(
                name: "ComunidadeId",
                table: "Comentarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Preco",
                table: "Acessos",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AddForeignKey(
                name: "FK_AcessosUtilizador_Utilizadores_UtilizadorId",
                table: "AcessosUtilizador",
                column: "UtilizadorId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacoes_Filmes_FilmeId",
                table: "Avaliacoes",
                column: "FilmeId",
                principalTable: "Filmes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacoes_Utilizadores_UsuarioId",
                table: "Avaliacoes",
                column: "UsuarioId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Utilizadores_UsuarioId",
                table: "Comentarios",
                column: "UsuarioId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_Utilizadores_UtilizadorId",
                table: "Compras",
                column: "UtilizadorId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ComunidadeMembros_Utilizadores_UtilizadorId",
                table: "ComunidadeMembros",
                column: "UtilizadorId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ListaPessoalItems_Filmes_FilmeId",
                table: "ListaPessoalItems",
                column: "FilmeId",
                principalTable: "Filmes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SessaoFilmes_Filmes_FilmeId",
                table: "SessaoFilmes",
                column: "FilmeId",
                principalTable: "Filmes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessoes_Festivals_FestivalId",
                table: "Sessoes",
                column: "FestivalId",
                principalTable: "Festivals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Visualizacoes_Utilizadores_UtilizadorId",
                table: "Visualizacoes",
                column: "UtilizadorId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
