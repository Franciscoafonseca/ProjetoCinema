using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Tests;

public class SocialComponentTests
{
    [Fact]
    public async Task PerfilPublico_NaoMostraEmailOuTelefone()
    {
        var utilizador = CriarUtilizador(2, "Publico", "publico@teste.pt", "910000000");
        var service = CriarPerfilService(utilizador);

        var perfil = await service.ObterPerfilPublicoAsync(utilizador.Id);

        Assert.Equal("Publico", perfil.Name);
        Assert.Null(typeof(PerfilPublicoDTO).GetProperty("Email"));
        Assert.Null(typeof(PerfilPublicoDTO).GetProperty("PhoneNumber"));
    }

    [Fact]
    public async Task PerfilPrivado_MostraDadosEditaveis()
    {
        var utilizador = CriarUtilizador(7, "Privado", "privado@teste.pt", "920000000");
        var service = CriarPerfilService(utilizador);

        var perfil = await service.ObterMeuPerfilAsync(utilizador.Id);

        Assert.Equal("privado@teste.pt", perfil.Email);
        Assert.Equal("920000000", perfil.PhoneNumber);
        Assert.True(typeof(PerfilPrivadoDTO).GetProperty("Email") != null);
        Assert.True(typeof(PerfilPrivadoDTO).GetProperty("PhoneNumber") != null);
    }

    [Fact]
    public async Task Utilizador_ReportaOutroUtilizador()
    {
        var reportado = CriarUtilizador(2, "Reportado", "reportado@teste.pt", "910000000");
        var reporter = CriarUtilizador(3, "Reporter", "reporter@teste.pt", "920000000");
        var repo = new ReporteUtilizadorRepositoryFalso();
        var service = new ReporteUtilizadorService(
            repo,
            new UtilizadorRepositoryFalso(reportado, reporter)
        );

        var reporte = await service.ReportarAsync(
            reportado.Id,
            reporter.Id,
            new CriarReporteUtilizadorDTO { Motivo = "Comportamento abusivo no perfil publico." }
        );

        Assert.Equal(EstadoReporteUtilizador.Pendente, reporte.Estado);
        Assert.Equal(reportado.Id, reporte.UtilizadorReportadoId);
        Assert.Single(repo.Reportes);
    }

    [Fact]
    public async Task Utilizador_NaoReportaMesmoPerfilDuasVezes()
    {
        var reportado = CriarUtilizador(2, "Reportado", "reportado@teste.pt", "910000000");
        var reporter = CriarUtilizador(3, "Reporter", "reporter@teste.pt", "920000000");
        var service = new ReporteUtilizadorService(
            new ReporteUtilizadorRepositoryFalso(),
            new UtilizadorRepositoryFalso(reportado, reporter)
        );

        await service.ReportarAsync(
            reportado.Id,
            reporter.Id,
            new CriarReporteUtilizadorDTO { Motivo = "Comportamento abusivo no perfil publico." }
        );

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ReportarAsync(
                reportado.Id,
                reporter.Id,
                new CriarReporteUtilizadorDTO { Motivo = "Novo reporte repetido do mesmo perfil." }
            )
        );
    }

    [Fact]
    public async Task Admin_ListaUtilizadoresReportados()
    {
        var reportado = CriarUtilizador(2, "Reportado", "reportado@teste.pt", "910000000");
        var reporter = CriarUtilizador(3, "Reporter", "reporter@teste.pt", "920000000");
        var repo = new ReporteUtilizadorRepositoryFalso();
        var service = new ReporteUtilizadorService(
            repo,
            new UtilizadorRepositoryFalso(reportado, reporter)
        );
        await service.ReportarAsync(
            reportado.Id,
            reporter.Id,
            new CriarReporteUtilizadorDTO { Motivo = "Comportamento abusivo no perfil publico." }
        );

        var reportes = await service.ObterTodosAsync();

        var reporte = Assert.Single(reportes);
        Assert.Equal("Reportado", reporte.UtilizadorReportadoNome);
        Assert.Equal("Reporter", reporte.ReportadoPorUtilizadorNome);
    }

    [Fact]
    public async Task Admin_AlteraEstadoDoReport()
    {
        var reportado = CriarUtilizador(2, "Reportado", "reportado@teste.pt", "910000000");
        var reporter = CriarUtilizador(3, "Reporter", "reporter@teste.pt", "920000000");
        var repo = new ReporteUtilizadorRepositoryFalso();
        var service = new ReporteUtilizadorService(
            repo,
            new UtilizadorRepositoryFalso(reportado, reporter)
        );
        var criado = await service.ReportarAsync(
            reportado.Id,
            reporter.Id,
            new CriarReporteUtilizadorDTO { Motivo = "Comportamento abusivo no perfil publico." }
        );

        var atualizado = await service.AtualizarEstadoAsync(
            criado.Id,
            EstadoReporteUtilizador.Rejeitado
        );

        Assert.Equal(EstadoReporteUtilizador.Rejeitado, atualizado.Estado);
    }

    [Fact]
    public async Task DonoDaComunidade_ModeraComentario()
    {
        var dados = CriarContextoComentario();
        var service = CriarComentarioService(dados);

        var comentario = await service.ModerarComentarioAsync(
            dados.Comunidade.PublicId,
            dados.Comentario.Id,
            new ModerarComentarioDTO { Acao = AcaoModeracaoComentario.Ocultar },
            dados.Dono.Id
        );

        Assert.False(comentario.Visivel);
        Assert.Equal(EstadoModeracaoComentario.Oculto, comentario.EstadoModeracao);
    }

    [Fact]
    public async Task NaoDono_NaoModeraComentario()
    {
        var dados = CriarContextoComentario();
        var service = CriarComentarioService(dados);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.ModerarComentarioAsync(
                dados.Comunidade.PublicId,
                dados.Comentario.Id,
                new ModerarComentarioDTO { Acao = AcaoModeracaoComentario.Remover },
                dados.Membro.Id
            )
        );
    }

    [Fact]
    public async Task ComentarioRemovidoOuOculto_NaoAparecePublicamente()
    {
        var dados = CriarContextoComentario();
        var service = CriarComentarioService(dados);
        await service.ModerarComentarioAsync(
            dados.Comunidade.PublicId,
            dados.Comentario.Id,
            new ModerarComentarioDTO { Acao = AcaoModeracaoComentario.Remover },
            dados.Dono.Id
        );

        var comentariosPublicos = await service.ObterComentariosPorComunidadeIdAsync(
            dados.Comunidade.PublicId,
            dados.Membro.Id
        );
        var comentariosDono = await service.ObterComentariosPorComunidadeIdAsync(
            dados.Comunidade.PublicId,
            dados.Dono.Id
        );

        Assert.Empty(comentariosPublicos);
        Assert.Single(comentariosDono);
    }

    private static PerfilUtilizadorService CriarPerfilService(Utilizador utilizador)
    {
        return new PerfilUtilizadorService(
            new UtilizadorRepositoryFalso(utilizador),
            new GeneroRepositoryFalso(),
            new PerfilFotoUploadServiceFalso()
        );
    }

    private static ComentarioService CriarComentarioService(ContextoComentario dados)
    {
        return new ComentarioService(
            new ComentarioRepositoryFalso(dados.Comentario),
            new UtilizadorRepositoryFalso(dados.Dono, dados.Membro),
            new ComunidadeRepositoryFalso(dados.Comunidade),
            new FilmeRepositoryFalso(),
            Array.Empty<IComentarioObserver>()
        );
    }

    private static ContextoComentario CriarContextoComentario()
    {
        var dono = CriarUtilizador(1, "Dono", "dono@teste.pt", "910000000");
        var membro = CriarUtilizador(2, "Membro", "membro@teste.pt", "920000000");
        var comunidade = new Comunidade
        {
            Id = 10,
            PublicId = Guid.NewGuid(),
            Name = "Clube",
            IsPublic = true,
            CreatedByUserId = dono.Id,
            CreatedByUser = dono,
        };
        comunidade.Members.Add(
            new ComunidadeMembro
            {
                ComunidadeId = comunidade.Id,
                Comunidade = comunidade,
                UtilizadorId = dono.Id,
                Utilizador = dono,
                Role = PapelMembroComunidade.Proprietario,
            }
        );
        comunidade.Members.Add(
            new ComunidadeMembro
            {
                ComunidadeId = comunidade.Id,
                Comunidade = comunidade,
                UtilizadorId = membro.Id,
                Utilizador = membro,
                Role = PapelMembroComunidade.Membro,
            }
        );

        var comentario = new Comentario
        {
            Id = 55,
            ComunidadeId = comunidade.Id,
            Comunidade = comunidade,
            UsuarioId = membro.Id,
            Usuario = membro,
            Texto = "Comentario publico da comunidade.",
            Visivel = true,
            EstadoModeracao = EstadoModeracaoComentario.Visivel,
        };
        comunidade.Comentarios.Add(comentario);

        return new ContextoComentario(dono, membro, comunidade, comentario);
    }

    private static Utilizador CriarUtilizador(int id, string nome, string email, string telefone)
    {
        return new Utilizador
        {
            Id = id,
            Name = nome,
            Email = email,
            PhoneNumber = telefone,
            Nationality = "PT",
            Perfil = new PerfilUtilizador
            {
                UtilizadorId = id,
                Bio = "Bio publica",
                CountryCode = "PT",
                Nationality = "Portugal",
                Location = "Lisboa",
                IsPublic = true,
            },
        };
    }

    private sealed record ContextoComentario(
        Utilizador Dono,
        Utilizador Membro,
        Comunidade Comunidade,
        Comentario Comentario
    );

    private sealed class UtilizadorRepositoryFalso : IUtilizadorRepository
    {
        private readonly Dictionary<int, Utilizador> _utilizadores;

        public UtilizadorRepositoryFalso(params Utilizador[] utilizadores)
        {
            _utilizadores = utilizadores.ToDictionary(u => u.Id);
        }

        public Task<Utilizador?> ObterPorIdAsync(int id) =>
            Task.FromResult(_utilizadores.GetValueOrDefault(id));

        public Task<Utilizador?> ObterPorEmailAsync(string email) =>
            Task.FromResult(_utilizadores.Values.FirstOrDefault(u => u.Email == email));

        public Task<Utilizador?> ObterPorTelefoneAsync(string telefone) =>
            Task.FromResult(_utilizadores.Values.FirstOrDefault(u => u.PhoneNumber == telefone));

        public Task<Utilizador?> ObterComPerfilAsync(int id) =>
            Task.FromResult(_utilizadores.GetValueOrDefault(id));

        public Task<List<Utilizador>> ObterPerfisPublicosAsync() =>
            Task.FromResult(_utilizadores.Values.Where(u => u.Perfil?.IsPublic == true).ToList());

        public Task AddAsync(Utilizador utilizador)
        {
            _utilizadores[utilizador.Id] = utilizador;
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    private sealed class GeneroRepositoryFalso : IGeneroRepository
    {
        public Task<IEnumerable<Genero>> ObterTodosAsync() =>
            Task.FromResult<IEnumerable<Genero>>(Array.Empty<Genero>());

        public Task<Genero?> ObterPorIdAsync(int id) => Task.FromResult<Genero?>(null);

        public Task AddAsync(Genero genero) => Task.CompletedTask;

        public Task SaveChangesAsync() => Task.CompletedTask;

        public Task<List<Genero>> ObterPorIdsAsync(IEnumerable<int> ids) =>
            Task.FromResult(new List<Genero>());
    }

    private sealed class PerfilFotoUploadServiceFalso : IPerfilFotoUploadService
    {
        public Task<string> GuardarAsync(IFormFile ficheiro) => Task.FromResult("/foto.png");
    }

    private sealed class ReporteUtilizadorRepositoryFalso : IReporteUtilizadorRepository
    {
        private int _sequencia;

        public List<ReporteUtilizador> Reportes { get; } = new();

        public Task<ReporteUtilizador?> ObterPorIdAsync(int id) =>
            Task.FromResult(Reportes.FirstOrDefault(r => r.Id == id));

        public Task<bool> ExisteAsync(int utilizadorReportadoId, int reportadoPorUtilizadorId) =>
            Task.FromResult(
                Reportes.Any(r =>
                    r.UtilizadorReportadoId == utilizadorReportadoId
                    && r.ReportadoPorUtilizadorId == reportadoPorUtilizadorId
                )
            );

        public Task<List<ReporteUtilizador>> ObterTodosAsync() =>
            Task.FromResult(Reportes.ToList());

        public Task AddAsync(ReporteUtilizador reporte)
        {
            reporte.Id = ++_sequencia;
            Reportes.Add(reporte);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    private sealed class ComentarioRepositoryFalso : IComentarioRepository
    {
        private readonly List<Comentario> _comentarios;

        public ComentarioRepositoryFalso(params Comentario[] comentarios)
        {
            _comentarios = comentarios.ToList();
        }

        public Task<Comentario> AddAsync(Comentario comentario)
        {
            _comentarios.Add(comentario);
            return Task.FromResult(comentario);
        }

        public Task<IEnumerable<Comentario>> ObterPorComunidadeIdAsync(int comunidadeId) =>
            ObterPorComunidadeIdAsync(comunidadeId, incluirModerados: false);

        public Task<IEnumerable<Comentario>> ObterPorComunidadeIdAsync(
            int comunidadeId,
            bool incluirModerados
        ) =>
            Task.FromResult<IEnumerable<Comentario>>(
                _comentarios.Where(c =>
                    c.ComunidadeId == comunidadeId && (incluirModerados || c.Visivel)
                )
            );

        public Task<IEnumerable<Comentario>> ObterPorFilmeIdAsync(int filmeId) =>
            Task.FromResult<IEnumerable<Comentario>>(
                _comentarios.Where(c => c.FilmeId == filmeId && c.Visivel)
            );

        public Task<Comentario?> GetByIdAsync(int comentarioId) =>
            Task.FromResult(_comentarios.FirstOrDefault(c => c.Id == comentarioId));

        public Task<IEnumerable<Comentario>> ObterReportadosPorComunidadeIdAsync(int comunidadeId) =>
            Task.FromResult<IEnumerable<Comentario>>(
                _comentarios.Where(c => c.ComunidadeId == comunidadeId && c.Reportado)
            );

        public Task UpdateAsync(Comentario comentario) => Task.CompletedTask;

        public Task<Comentario?> ObterPorIdAsync(int id) =>
            Task.FromResult(_comentarios.FirstOrDefault(c => c.Id == id));

        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    private sealed class ComunidadeRepositoryFalso : IComunidadeRepository
    {
        private readonly Comunidade _comunidade;

        public ComunidadeRepositoryFalso(Comunidade comunidade)
        {
            _comunidade = comunidade;
        }

        public Task<Comunidade> AddComunidadeAsync(Comunidade comunidade) =>
            Task.FromResult(comunidade);

        public Task<Comunidade?> GetComunidadeByPublicIdAsync(Guid publicId) =>
            Task.FromResult(_comunidade.PublicId == publicId ? _comunidade : null);

        public Task<IEnumerable<Comunidade>> FindComunidadesAsync(
            Expression<Func<Comunidade, bool>> predicate
        ) =>
            Task.FromResult<IEnumerable<Comunidade>>(
                new[] { _comunidade }.AsQueryable().Where(predicate)
            );

        public Task<bool> IsMembroAsync(int comunidadeId, int utilizadorId) =>
            Task.FromResult(
                _comunidade.Id == comunidadeId
                && _comunidade.Members.Any(m => m.UtilizadorId == utilizadorId)
            );

        public Task<bool> IsProprietarioAsync(int comunidadeId, int utilizadorId) =>
            Task.FromResult(
                _comunidade.Id == comunidadeId
                && _comunidade.Members.Any(m =>
                    m.UtilizadorId == utilizadorId && m.Role == PapelMembroComunidade.Proprietario
                )
            );

        public Task<Comunidade?> GetComunidadeByConviteAsync(string codigoConvite) =>
            Task.FromResult<Comunidade?>(null);

        public Task<ComunidadeMembro> AdicionarMembroAsync(ComunidadeMembro membro) =>
            Task.FromResult(membro);

        public Task ApagarComunidadeAsync(Comunidade comunidade) => Task.CompletedTask;

        public Task RemoverMembroAsync(ComunidadeMembro membro) => Task.CompletedTask;
    }

    private sealed class FilmeRepositoryFalso : IFilmeRepository
    {
        public Task<IEnumerable<Filme>> ObterTodosAsync() =>
            Task.FromResult<IEnumerable<Filme>>(Array.Empty<Filme>());

        public Task<Filme?> ObterPorIdAsync(int id) => Task.FromResult<Filme?>(null);

        public Task<Filme?> ObterDetalhePorIdAsync(int id) => Task.FromResult<Filme?>(null);

        public Task<Filme?> ObterPorTmdbIdAsync(int tmdbId) => Task.FromResult<Filme?>(null);

        public Task<List<Filme>> ObterPrincipaisAsync(int quantidade) =>
            Task.FromResult(new List<Filme>());

        public Task<List<Festival>> ObterFestivaisDoFilmeAsync(int filmeId) =>
            Task.FromResult(new List<Festival>());

        public Task<List<Sessao>> ObterSessoesDoFilmeAsync(int filmeId) =>
            Task.FromResult(new List<Sessao>());

        public Task<Genero> ObterOuCriarGeneroAsync(string nome) =>
            Task.FromResult(new Genero { Name = nome });

        public Task<Pessoa> ObterOuCriarPessoaAsync(
            int? tmdbPessoaId,
            string nome,
            string? imagemUrl
        ) =>
            Task.FromResult(new Pessoa { Nome = nome, ImagemUrl = imagemUrl });

        public Task<bool> UtilizadorViuFilmeAsync(int utilizadorId, int filmeId) =>
            Task.FromResult(false);

        public Task<Avaliacao?> ObterAvaliacaoAsync(int utilizadorId, int filmeId) =>
            Task.FromResult<Avaliacao?>(null);

        public Task AddAvaliacaoAsync(Avaliacao avaliacao) => Task.CompletedTask;

        public Task AddAsync(Filme filme) => Task.CompletedTask;

        public void AtualizarVideo(Filme filme, string? provider, string? key, string? url) { }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
