using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    /// <inheritdoc />
    public partial class FinalSchemaCinemaFestival : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Festivals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Premios = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Festivals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Filmes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbId = table.Column<int>(type: "INTEGER", nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", nullable: false),
                    TituloOriginal = table.Column<string>(type: "TEXT", nullable: true),
                    Sinopse = table.Column<string>(type: "TEXT", nullable: true),
                    DataLancamento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DuracaoMinutos = table.Column<int>(type: "INTEGER", nullable: true),
                    Genero = table.Column<string>(type: "TEXT", nullable: true),
                    Classificacao = table.Column<string>(type: "TEXT", nullable: true),
                    AvaliacaoTmdb = table.Column<double>(type: "REAL", nullable: true),
                    CapaUrl = table.Column<string>(type: "TEXT", nullable: false),
                    TrailerUrl = table.Column<string>(type: "TEXT", nullable: true),
                    VideoProvider = table.Column<string>(type: "TEXT", nullable: true),
                    VideoKey = table.Column<string>(type: "TEXT", nullable: true),
                    VideoUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Realizador = table.Column<string>(type: "TEXT", nullable: true),
                    AtoresPrincipais = table.Column<string>(type: "TEXT", nullable: true),
                    TmdbReviewsJson = table.Column<string>(type: "TEXT", nullable: true),
                    Premios = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filmes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Generos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pessoas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TmdbPessoaId = table.Column<int>(type: "INTEGER", nullable: true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    ImagemUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pessoas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rewards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Pontos = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rewards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RewardsTransacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Pontos = table.Column<int>(type: "INTEGER", nullable: false),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Motivo = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    ChaveAcao = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardsTransacoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utilizadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Nationality = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizadores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PremiosFestival",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FestivalId = table.Column<int>(type: "INTEGER", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    DataAberturaVotacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataFechoVotacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EstadoPremio = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremiosFestival", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PremiosFestival_Festivals_FestivalId",
                        column: x => x.FestivalId,
                        principalTable: "Festivals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FestivalFilmes",
                columns: table => new
                {
                    FestivalId = table.Column<int>(type: "INTEGER", nullable: false),
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ElegivelPremiosPublico = table.Column<bool>(type: "INTEGER", nullable: false),
                    Secao = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    Categoria = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    DataAdicao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FestivalFilmes", x => new { x.FestivalId, x.FilmeId });
                    table.ForeignKey(
                        name: "FK_FestivalFilmes_Festivals_FestivalId",
                        column: x => x.FestivalId,
                        principalTable: "Festivals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FestivalFilmes_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FestivalId = table.Column<int>(type: "INTEGER", nullable: false),
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Inicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Fim = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TemChatAoVivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    Observacoes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessoes_Festivals_FestivalId",
                        column: x => x.FestivalId,
                        principalTable: "Festivals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sessoes_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FilmeGeneros",
                columns: table => new
                {
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: false),
                    GeneroId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmeGeneros", x => new { x.FilmeId, x.GeneroId });
                    table.ForeignKey(
                        name: "FK_FilmeGeneros_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilmeGeneros_Generos_GeneroId",
                        column: x => x.GeneroId,
                        principalTable: "Generos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FilmePessoas",
                columns: table => new
                {
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: false),
                    PessoaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Funcao = table.Column<int>(type: "INTEGER", nullable: false),
                    Personagem = table.Column<string>(type: "TEXT", nullable: true),
                    Ordem = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmePessoas", x => new { x.FilmeId, x.PessoaId, x.Funcao });
                    table.ForeignKey(
                        name: "FK_FilmePessoas_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilmePessoas_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Avaliacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    Pontuacao = table.Column<int>(type: "INTEGER", nullable: false),
                    Texto = table.Column<string>(type: "TEXT", nullable: false),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avaliacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Avaliacoes_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Avaliacoes_Utilizadores_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Carrinhos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carrinhos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carrinhos_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Referencia = table.Column<string>(type: "TEXT", nullable: false),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    ValorTotal = table.Column<double>(type: "REAL", nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    CriadaEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PagaEm = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Compras_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comunidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PublicId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CodigoConvite = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comunidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comunidades_Utilizadores_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ListasPessoais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasPessoais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListasPessoais_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerfisUtilizador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Bio = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Nationality = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    CountryCode = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfisUtilizador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerfisUtilizador_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportesUtilizadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UtilizadorReportadoId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReportadoPorUtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Motivo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportesUtilizadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportesUtilizadores_Utilizadores_ReportadoPorUtilizadorId",
                        column: x => x.ReportadoPorUtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportesUtilizadores_Utilizadores_UtilizadorReportadoId",
                        column: x => x.UtilizadorReportadoId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UtilizadoresGenerosFavoritos",
                columns: table => new
                {
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    GeneroId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilizadoresGenerosFavoritos", x => new { x.UtilizadorId, x.GeneroId });
                    table.ForeignKey(
                        name: "FK_UtilizadoresGenerosFavoritos_Generos_GeneroId",
                        column: x => x.GeneroId,
                        principalTable: "Generos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UtilizadoresGenerosFavoritos_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResultadosPremiosFestival",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PremioFestivalId = table.Column<int>(type: "INTEGER", nullable: false),
                    FilmeIdVencedor = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalVotos = table.Column<int>(type: "INTEGER", nullable: false),
                    PublicadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PublicadoPorUtilizadorId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultadosPremiosFestival", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultadosPremiosFestival_Filmes_FilmeIdVencedor",
                        column: x => x.FilmeIdVencedor,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultadosPremiosFestival_PremiosFestival_PremioFestivalId",
                        column: x => x.PremioFestivalId,
                        principalTable: "PremiosFestival",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResultadosPremiosFestival_Utilizadores_PublicadoPorUtilizadorId",
                        column: x => x.PublicadoPorUtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VotosPremiosFestival",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PremioFestivalId = table.Column<int>(type: "INTEGER", nullable: false),
                    FestivalId = table.Column<int>(type: "INTEGER", nullable: false),
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: false),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataVoto = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotosPremiosFestival", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VotosPremiosFestival_Festivals_FestivalId",
                        column: x => x.FestivalId,
                        principalTable: "Festivals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VotosPremiosFestival_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VotosPremiosFestival_PremiosFestival_PremioFestivalId",
                        column: x => x.PremioFestivalId,
                        principalTable: "PremiosFestival",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VotosPremiosFestival_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Acessos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UtilizadorId = table.Column<string>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Preco = table.Column<double>(type: "REAL", nullable: false),
                    PrecoPago = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsAtivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Validade = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SessaoId = table.Column<int>(type: "INTEGER", nullable: true),
                    FestivalId = table.Column<int>(type: "INTEGER", nullable: true),
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: true),
                    DataAcesso = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DuracaoHoras = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acessos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Acessos_Festivals_FestivalId",
                        column: x => x.FestivalId,
                        principalTable: "Festivals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Acessos_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Acessos_Sessoes_SessaoId",
                        column: x => x.SessaoId,
                        principalTable: "Sessoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MensagensChatSessao",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    SessaoId = table.Column<int>(type: "INTEGER", nullable: false),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Texto = table.Column<string>(type: "TEXT", maxLength: 600, nullable: false),
                    EnviadaEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Removida = table.Column<bool>(type: "INTEGER", nullable: false),
                    RemovidaPorModeracao = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensagensChatSessao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensagensChatSessao_Sessoes_SessaoId",
                        column: x => x.SessaoId,
                        principalTable: "Sessoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MensagensChatSessao_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Visualizacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: false),
                    SessaoId = table.Column<int>(type: "INTEGER", nullable: true),
                    FestivalId = table.Column<int>(type: "INTEGER", nullable: true),
                    TipoConteudo = table.Column<string>(type: "TEXT", nullable: false),
                    TipoAcessoUsado = table.Column<int>(type: "INTEGER", nullable: true),
                    UrlVisualizacao = table.Column<string>(type: "TEXT", nullable: true),
                    VisualizadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visualizacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Visualizacoes_Festivals_FestivalId",
                        column: x => x.FestivalId,
                        principalTable: "Festivals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Visualizacoes_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Visualizacoes_Sessoes_SessaoId",
                        column: x => x.SessaoId,
                        principalTable: "Sessoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Visualizacoes_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pagamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompraId = table.Column<int>(type: "INTEGER", nullable: false),
                    Referencia = table.Column<string>(type: "TEXT", nullable: false),
                    Entidade = table.Column<string>(type: "TEXT", nullable: true),
                    Valor = table.Column<double>(type: "REAL", nullable: false),
                    Metodo = table.Column<string>(type: "TEXT", nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProcessadoEm = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Mensagem = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagamentos_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comentarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Texto = table.Column<string>(type: "TEXT", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Reportado = table.Column<bool>(type: "INTEGER", nullable: false),
                    Visivel = table.Column<bool>(type: "INTEGER", nullable: false),
                    EstadoModeracao = table.Column<int>(type: "INTEGER", nullable: false),
                    ModeradoPorUtilizadorId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModeradoEm = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    ComunidadeId = table.Column<int>(type: "INTEGER", nullable: true),
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarios", x => x.Id);
                    table.CheckConstraint("CK_Comentarios_Alvo", "ComunidadeId IS NOT NULL OR FilmeId IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_Comentarios_Comunidades_ComunidadeId",
                        column: x => x.ComunidadeId,
                        principalTable: "Comunidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comentarios_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comentarios_Utilizadores_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComunidadeMembros",
                columns: table => new
                {
                    ComunidadeId = table.Column<int>(type: "INTEGER", nullable: false),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComunidadeMembros", x => new { x.ComunidadeId, x.UtilizadorId });
                    table.ForeignKey(
                        name: "FK_ComunidadeMembros_Comunidades_ComunidadeId",
                        column: x => x.ComunidadeId,
                        principalTable: "Comunidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComunidadeMembros_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ListaPessoalItems",
                columns: table => new
                {
                    ListaPessoalId = table.Column<int>(type: "INTEGER", nullable: false),
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListaPessoalItems", x => new { x.ListaPessoalId, x.FilmeId });
                    table.ForeignKey(
                        name: "FK_ListaPessoalItems_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListaPessoalItems_ListasPessoais_ListaPessoalId",
                        column: x => x.ListaPessoalId,
                        principalTable: "ListasPessoais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcessosUtilizador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UtilizadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    AcessoId = table.Column<int>(type: "INTEGER", nullable: false),
                    CompraId = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoAcesso = table.Column<int>(type: "INTEGER", nullable: false),
                    SessaoId = table.Column<int>(type: "INTEGER", nullable: true),
                    FestivalId = table.Column<int>(type: "INTEGER", nullable: true),
                    FilmeId = table.Column<int>(type: "INTEGER", nullable: true),
                    InicioValidade = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FimValidade = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcessosUtilizador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcessosUtilizador_Acessos_AcessoId",
                        column: x => x.AcessoId,
                        principalTable: "Acessos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcessosUtilizador_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcessosUtilizador_Festivals_FestivalId",
                        column: x => x.FestivalId,
                        principalTable: "Festivals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcessosUtilizador_Filmes_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcessosUtilizador_Sessoes_SessaoId",
                        column: x => x.SessaoId,
                        principalTable: "Sessoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcessosUtilizador_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItensCarrinho",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CarrinhoId = table.Column<int>(type: "INTEGER", nullable: false),
                    AcessoId = table.Column<int>(type: "INTEGER", nullable: false),
                    PrecoUnitario = table.Column<double>(type: "REAL", nullable: false),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    DataAdicao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensCarrinho", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensCarrinho_Acessos_AcessoId",
                        column: x => x.AcessoId,
                        principalTable: "Acessos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItensCarrinho_Carrinhos_CarrinhoId",
                        column: x => x.CarrinhoId,
                        principalTable: "Carrinhos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItensCompra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompraId = table.Column<int>(type: "INTEGER", nullable: false),
                    AcessoId = table.Column<int>(type: "INTEGER", nullable: false),
                    NomeAcesso = table.Column<string>(type: "TEXT", nullable: false),
                    TipoAcesso = table.Column<int>(type: "INTEGER", nullable: false),
                    PrecoUnitario = table.Column<double>(type: "REAL", nullable: false),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    Subtotal = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensCompra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensCompra_Acessos_AcessoId",
                        column: x => x.AcessoId,
                        principalTable: "Acessos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItensCompra_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Acessos_FestivalId",
                table: "Acessos",
                column: "FestivalId");

            migrationBuilder.CreateIndex(
                name: "IX_Acessos_FilmeId",
                table: "Acessos",
                column: "FilmeId");

            migrationBuilder.CreateIndex(
                name: "IX_Acessos_SessaoId",
                table: "Acessos",
                column: "SessaoId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessosUtilizador_AcessoId",
                table: "AcessosUtilizador",
                column: "AcessoId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessosUtilizador_CompraId",
                table: "AcessosUtilizador",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessosUtilizador_FestivalId",
                table: "AcessosUtilizador",
                column: "FestivalId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessosUtilizador_FilmeId",
                table: "AcessosUtilizador",
                column: "FilmeId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessosUtilizador_SessaoId",
                table: "AcessosUtilizador",
                column: "SessaoId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessosUtilizador_UtilizadorId_AcessoId",
                table: "AcessosUtilizador",
                columns: new[] { "UtilizadorId", "AcessoId" });

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacoes_FilmeId",
                table: "Avaliacoes",
                column: "FilmeId");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacoes_UsuarioId_FilmeId",
                table: "Avaliacoes",
                columns: new[] { "UsuarioId", "FilmeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carrinhos_UtilizadorId",
                table: "Carrinhos",
                column: "UtilizadorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_ComunidadeId",
                table: "Comentarios",
                column: "ComunidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_FilmeId",
                table: "Comentarios",
                column: "FilmeId");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_UsuarioId",
                table: "Comentarios",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_Referencia",
                table: "Compras",
                column: "Referencia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Compras_UtilizadorId",
                table: "Compras",
                column: "UtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_ComunidadeMembros_UtilizadorId",
                table: "ComunidadeMembros",
                column: "UtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comunidades_CodigoConvite",
                table: "Comunidades",
                column: "CodigoConvite",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comunidades_CreatedByUserId",
                table: "Comunidades",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comunidades_PublicId",
                table: "Comunidades",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FestivalFilmes_FilmeId",
                table: "FestivalFilmes",
                column: "FilmeId");

            migrationBuilder.CreateIndex(
                name: "IX_FilmeGeneros_GeneroId",
                table: "FilmeGeneros",
                column: "GeneroId");

            migrationBuilder.CreateIndex(
                name: "IX_FilmePessoas_PessoaId",
                table: "FilmePessoas",
                column: "PessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_Filmes_TmdbId",
                table: "Filmes",
                column: "TmdbId");

            migrationBuilder.CreateIndex(
                name: "IX_Generos_Name",
                table: "Generos",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItensCarrinho_AcessoId",
                table: "ItensCarrinho",
                column: "AcessoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensCarrinho_CarrinhoId_AcessoId",
                table: "ItensCarrinho",
                columns: new[] { "CarrinhoId", "AcessoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItensCompra_AcessoId",
                table: "ItensCompra",
                column: "AcessoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensCompra_CompraId",
                table: "ItensCompra",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_ListaPessoalItems_FilmeId",
                table: "ListaPessoalItems",
                column: "FilmeId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasPessoais_UtilizadorId",
                table: "ListasPessoais",
                column: "UtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_MensagensChatSessao_SessaoId_EnviadaEm",
                table: "MensagensChatSessao",
                columns: new[] { "SessaoId", "EnviadaEm" });

            migrationBuilder.CreateIndex(
                name: "IX_MensagensChatSessao_UtilizadorId",
                table: "MensagensChatSessao",
                column: "UtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_CompraId",
                table: "Pagamentos",
                column: "CompraId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_Referencia",
                table: "Pagamentos",
                column: "Referencia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerfisUtilizador_UtilizadorId",
                table: "PerfisUtilizador",
                column: "UtilizadorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_Nome",
                table: "Pessoas",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_TmdbPessoaId",
                table: "Pessoas",
                column: "TmdbPessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_PremiosFestival_FestivalId",
                table: "PremiosFestival",
                column: "FestivalId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportesUtilizadores_ReportadoPorUtilizadorId",
                table: "ReportesUtilizadores",
                column: "ReportadoPorUtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportesUtilizadores_UtilizadorReportadoId_ReportadoPorUtilizadorId",
                table: "ReportesUtilizadores",
                columns: new[] { "UtilizadorReportadoId", "ReportadoPorUtilizadorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResultadosPremiosFestival_FilmeIdVencedor",
                table: "ResultadosPremiosFestival",
                column: "FilmeIdVencedor");

            migrationBuilder.CreateIndex(
                name: "IX_ResultadosPremiosFestival_PremioFestivalId",
                table: "ResultadosPremiosFestival",
                column: "PremioFestivalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResultadosPremiosFestival_PublicadoPorUtilizadorId",
                table: "ResultadosPremiosFestival",
                column: "PublicadoPorUtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_RewardsTransacoes_UtilizadorId_ChaveAcao",
                table: "RewardsTransacoes",
                columns: new[] { "UtilizadorId", "ChaveAcao" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessoes_FestivalId",
                table: "Sessoes",
                column: "FestivalId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessoes_FilmeId",
                table: "Sessoes",
                column: "FilmeId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilizadores_Email",
                table: "Utilizadores",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Utilizadores_PhoneNumber",
                table: "Utilizadores",
                column: "PhoneNumber",
                unique: true,
                filter: "\"PhoneNumber\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_UtilizadoresGenerosFavoritos_GeneroId",
                table: "UtilizadoresGenerosFavoritos",
                column: "GeneroId");

            migrationBuilder.CreateIndex(
                name: "IX_Visualizacoes_FestivalId",
                table: "Visualizacoes",
                column: "FestivalId");

            migrationBuilder.CreateIndex(
                name: "IX_Visualizacoes_FilmeId",
                table: "Visualizacoes",
                column: "FilmeId");

            migrationBuilder.CreateIndex(
                name: "IX_Visualizacoes_SessaoId",
                table: "Visualizacoes",
                column: "SessaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Visualizacoes_UtilizadorId_VisualizadoEm",
                table: "Visualizacoes",
                columns: new[] { "UtilizadorId", "VisualizadoEm" });

            migrationBuilder.CreateIndex(
                name: "IX_VotosPremiosFestival_FestivalId",
                table: "VotosPremiosFestival",
                column: "FestivalId");

            migrationBuilder.CreateIndex(
                name: "IX_VotosPremiosFestival_FilmeId",
                table: "VotosPremiosFestival",
                column: "FilmeId");

            migrationBuilder.CreateIndex(
                name: "IX_VotosPremiosFestival_PremioFestivalId_UtilizadorId",
                table: "VotosPremiosFestival",
                columns: new[] { "PremioFestivalId", "UtilizadorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VotosPremiosFestival_UtilizadorId",
                table: "VotosPremiosFestival",
                column: "UtilizadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcessosUtilizador");

            migrationBuilder.DropTable(
                name: "Avaliacoes");

            migrationBuilder.DropTable(
                name: "Comentarios");

            migrationBuilder.DropTable(
                name: "ComunidadeMembros");

            migrationBuilder.DropTable(
                name: "FestivalFilmes");

            migrationBuilder.DropTable(
                name: "FilmeGeneros");

            migrationBuilder.DropTable(
                name: "FilmePessoas");

            migrationBuilder.DropTable(
                name: "ItensCarrinho");

            migrationBuilder.DropTable(
                name: "ItensCompra");

            migrationBuilder.DropTable(
                name: "ListaPessoalItems");

            migrationBuilder.DropTable(
                name: "MensagensChatSessao");

            migrationBuilder.DropTable(
                name: "Pagamentos");

            migrationBuilder.DropTable(
                name: "PerfisUtilizador");

            migrationBuilder.DropTable(
                name: "ReportesUtilizadores");

            migrationBuilder.DropTable(
                name: "ResultadosPremiosFestival");

            migrationBuilder.DropTable(
                name: "Rewards");

            migrationBuilder.DropTable(
                name: "RewardsTransacoes");

            migrationBuilder.DropTable(
                name: "UtilizadoresGenerosFavoritos");

            migrationBuilder.DropTable(
                name: "Visualizacoes");

            migrationBuilder.DropTable(
                name: "VotosPremiosFestival");

            migrationBuilder.DropTable(
                name: "Comunidades");

            migrationBuilder.DropTable(
                name: "Pessoas");

            migrationBuilder.DropTable(
                name: "Carrinhos");

            migrationBuilder.DropTable(
                name: "Acessos");

            migrationBuilder.DropTable(
                name: "ListasPessoais");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "Generos");

            migrationBuilder.DropTable(
                name: "PremiosFestival");

            migrationBuilder.DropTable(
                name: "Sessoes");

            migrationBuilder.DropTable(
                name: "Utilizadores");

            migrationBuilder.DropTable(
                name: "Festivals");

            migrationBuilder.DropTable(
                name: "Filmes");
        }
    }
}
