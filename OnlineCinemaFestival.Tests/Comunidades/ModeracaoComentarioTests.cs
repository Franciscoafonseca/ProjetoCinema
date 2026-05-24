using System.Linq.Expressions;
using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Tests.Support.Builders;
using OnlineCinemaFestival.Tests.Support.Fakes;

namespace OnlineCinemaFestival.Tests.Comunidades;

public class ModeracaoComentarioTests
{
    [Fact]
    public async Task Moderar_PeloDono_OcultaComentario()
    {
        var dados = CriarContexto();
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
    public async Task Moderar_PorMembroNaoDono_Rejeita()
    {
        var dados = CriarContexto();
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
    public async Task ObterComentarios_AposRemocao_NaoMostraAUtilizadoresPublicos()
    {
        var dados = CriarContexto();
        var service = CriarComentarioService(dados);
        await service.ModerarComentarioAsync(
            dados.Comunidade.PublicId,
            dados.Comentario.Id,
            new ModerarComentarioDTO { Acao = AcaoModeracaoComentario.Remover },
            dados.Dono.Id
        );

        var publicos = await service.ObterComentariosPorComunidadeIdAsync(
            dados.Comunidade.PublicId,
            dados.Membro.Id
        );
        var doDono = await service.ObterComentariosPorComunidadeIdAsync(
            dados.Comunidade.PublicId,
            dados.Dono.Id
        );

        Assert.Empty(publicos);
        Assert.Single(doDono);
    }

    private static ComentarioService CriarComentarioService(ContextoComentario dados)
    {
        return new ComentarioService(
            new ComentarioRepositoryFalso(dados.Comentario),
            new UtilizadorRepositoryFalso(dados.Dono, dados.Membro),
            new ComunidadeRepositoryFalso(dados.Comunidade),
            new FilmeRepositoryVazioFalso(),
            Array.Empty<IComentarioObserver>()
        );
    }

    private static ContextoComentario CriarContexto()
    {
        var dono = new UtilizadorBuilder().ComId(1).ComNome("Dono").ComEmail("dono@teste.pt").Build();
        var membro = new UtilizadorBuilder().ComId(2).ComNome("Membro").ComEmail("membro@teste.pt").Build();
        var comunidade = new ComunidadeBuilder()
            .ComId(10)
            .ComNome("Clube")
            .CriadaPor(dono)
            .ComMembro(membro)
            .Build();

        var comentario = new ComentarioBuilder()
            .ComId(55)
            .ComTexto("Comentario publico da comunidade.")
            .DoUtilizador(membro)
            .NaComunidade(comunidade)
            .Build();
        comunidade.Comentarios.Add(comentario);

        return new ContextoComentario(dono, membro, comunidade, comentario);
    }

    private sealed record ContextoComentario(
        Utilizador Dono,
        Utilizador Membro,
        Comunidade Comunidade,
        Comentario Comentario
    );

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

        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    private sealed class FilmeRepositoryVazioFalso : IFilmeRepository
    {
        public Task<IEnumerable<Filme>> ObterTodosAsync() =>
            Task.FromResult<IEnumerable<Filme>>(Array.Empty<Filme>());

        public Task<Filme?> ObterPorIdAsync(int id) => Task.FromResult<Filme?>(null);
        public Task<Filme?> ObterDetalhePorIdAsync(int id) => Task.FromResult<Filme?>(null);
        public Task<Filme?> ObterPorTmdbIdAsync(int tmdbId) => Task.FromResult<Filme?>(null);
        public Task<List<Filme>> ObterPrincipaisAsync(int quantidade) => Task.FromResult(new List<Filme>());
        public Task<List<Festival>> ObterFestivaisDoFilmeAsync(int filmeId) => Task.FromResult(new List<Festival>());
        public Task<List<Sessao>> ObterSessoesDoFilmeAsync(int filmeId) => Task.FromResult(new List<Sessao>());
        public Task<Genero> ObterOuCriarGeneroAsync(string nome) => Task.FromResult(new Genero { Name = nome });
        public Task<Pessoa> ObterOuCriarPessoaAsync(int? tmdbPessoaId, string nome, string? imagemUrl) =>
            Task.FromResult(new Pessoa { Nome = nome, ImagemUrl = imagemUrl });
        public Task<bool> UtilizadorViuFilmeAsync(int utilizadorId, int filmeId) => Task.FromResult(false);
        public Task<Avaliacao?> ObterAvaliacaoAsync(int utilizadorId, int filmeId) => Task.FromResult<Avaliacao?>(null);
        public Task AddAvaliacaoAsync(Avaliacao avaliacao) => Task.CompletedTask;
        public Task AddAsync(Filme filme) => Task.CompletedTask;
        public void AtualizarVideo(Filme filme, string? provider, string? key, string? url) { }
        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
