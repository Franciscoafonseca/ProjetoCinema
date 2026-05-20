using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class FilmeService : IFilmeService
{
    private readonly IFilmeRepository _filmeRepository;
    private readonly IAcessoRepository _acessoRepository;
    private readonly ITmdbService _tmdbService;
    private readonly IValidacaoAcessoService _validacaoAcessoService;

    public FilmeService(
        IFilmeRepository filmeRepository,
        IAcessoRepository acessoRepository,
        ITmdbService tmdbService,
        IValidacaoAcessoService validacaoAcessoService
    )
    {
        _filmeRepository = filmeRepository;
        _acessoRepository = acessoRepository;
        _tmdbService = tmdbService;
        _validacaoAcessoService = validacaoAcessoService;
    }

    public async Task<IEnumerable<FilmeReadDTO>> ObterTodosFilmesAsync()
    {
        var filmes = await _filmeRepository.ObterTodosAsync();
        return filmes.Select(FilmeMapper.MapToReadDTO);
    }

    public async Task<IEnumerable<FilmeReadDTO>> SearchFilmesTmdbAsync(string query)
    {
        var filmesTmdb = await _tmdbService.SearchFilmesTmdbAsync(query);
        return filmesTmdb.Select(FilmeMapper.MapToReadDTOFromTmdb);
    }

    public async Task<IEnumerable<FilmeReadDTO>> ObterFilmesIniciaisTmdbAsync()
    {
        var filmesTmdb = await _tmdbService.ObterFilmesIniciaisAsync();
        return filmesTmdb.Select(FilmeMapper.MapToReadDTOFromTmdb);
    }

    public async Task<FilmeReadDTO> ImportFilmeFromTmdbAsync(int tmdbId)
    {
        var filmeExistente = await _filmeRepository.ObterPorTmdbIdAsync(tmdbId);

        if (filmeExistente != null)
            return FilmeMapper.MapToReadDTO(filmeExistente);

        var filmeTmdb = await _tmdbService.ObterFilmePorTmdbIdAsync(tmdbId);

        if (filmeTmdb == null)
            throw new KeyNotFoundException($"Filme com TMDb ID {tmdbId} nao encontrado.");

        var novoFilme = FilmeMapper.MapFromTmdbDTO(filmeTmdb);

        foreach (var nomeGenero in filmeTmdb.Generos.Where(g => !string.IsNullOrWhiteSpace(g)).Distinct())
        {
            var genero = await _filmeRepository.ObterOuCriarGeneroAsync(nomeGenero);
            novoFilme.FilmeGeneros.Add(new FilmeGenero { Filme = novoFilme, Genero = genero });
        }

        await AdicionarPessoaAsync(novoFilme, filmeTmdb.RealizadorDetalhe, FuncaoPessoaFilme.Realizador, 0);
        await AdicionarPessoaAsync(novoFilme, filmeTmdb.ProdutorDetalhe, FuncaoPessoaFilme.Produtor, 0);

        foreach (var ator in filmeTmdb.AtoresDetalhes)
            await AdicionarPessoaAsync(novoFilme, ator, FuncaoPessoaFilme.Ator, ator.Ordem);

        await _filmeRepository.AddAsync(novoFilme);
        await _filmeRepository.SaveChangesAsync();

        await GarantirAcessosAutomaticosAsync(novoFilme.Id);

        return FilmeMapper.MapToReadDTO(novoFilme);
    }

    public async Task<FilmeReadDTO> AtualizarVideoAsync(int filmeId, AtualizarVideoFilmeDTO dto)
    {
        var filme = await _filmeRepository.ObterDetalhePorIdAsync(filmeId);

        if (filme == null)
            throw new KeyNotFoundException("Filme nao encontrado.");

        var videoUrl = string.IsNullOrWhiteSpace(dto.VideoUrl)
            ? CriarVideoUrl(dto.VideoProvider, dto.VideoKey)
            : dto.VideoUrl.Trim();

        _filmeRepository.AtualizarVideo(
            filme,
            NormalizarValor(dto.VideoProvider),
            NormalizarValor(dto.VideoKey),
            NormalizarValor(videoUrl),
            filme.DuracaoVideoSegundos
        );

        await _filmeRepository.SaveChangesAsync();
        return FilmeMapper.MapToReadDTO(filme);
    }

    public async Task<FilmeDetalheDTO?> ObterDetalheAsync(int filmeId, int? utilizadorId)
    {
        var filme = await _filmeRepository.ObterDetalhePorIdAsync(filmeId);

        if (filme == null)
            return null;

        var dto = FilmeMapper.MapToReadDTO(filme);

        if (utilizadorId.HasValue)
        {
            dto.PodeAvaliar = await _filmeRepository.UtilizadorViuFilmeAsync(
                utilizadorId.Value,
                filmeId
            );
            dto.PodeVer =
                await _validacaoAcessoService.ObterAcessoValidoParaFilmeAsync(
                    utilizadorId.Value,
                    filme,
                    null
                ) != null;
        }

        return dto;
    }

    public async Task<AvaliacaoDTO> CriarReviewAsync(
        int utilizadorId,
        int filmeId,
        CriarAvaliacaoDTO dto
    )
    {
        var filme = await _filmeRepository.ObterDetalhePorIdAsync(filmeId);

        if (filme == null)
            throw new KeyNotFoundException("Filme nao encontrado.");

        if (!await _filmeRepository.UtilizadorViuFilmeAsync(utilizadorId, filmeId))
            throw new UnauthorizedAccessException("So podes avaliar depois de ver o filme.");

        if (await _filmeRepository.ObterAvaliacaoAsync(utilizadorId, filmeId) != null)
            throw new InvalidOperationException("Ja existe uma review tua para este filme.");

        ValidarReview(dto);

        var avaliacao = new Avaliacao
        {
            FilmeId = filmeId,
            UsuarioId = utilizadorId,
            Pontuacao = dto.Pontuacao,
            Texto = dto.Texto.Trim(),
            Data = DateTime.UtcNow,
        };

        await _filmeRepository.AddAvaliacaoAsync(avaliacao);
        await _filmeRepository.SaveChangesAsync();

        return new AvaliacaoDTO
        {
            Id = avaliacao.Id,
            FilmeId = filmeId,
            TituloFilme = filme.Titulo,
            UsuarioId = utilizadorId,
            Pontuacao = avaliacao.Pontuacao,
            Texto = avaliacao.Texto,
            Data = avaliacao.Data,
        };
    }

    private async Task GarantirAcessosAutomaticosAsync(int filmeId)
    {
        var acessos = new List<Acesso>();

        if (
            await _acessoRepository.GetAtivoParaCarrinhoAsync(
                TipoAcesso.AluguerDigital,
                null,
                filmeId,
                null,
                null
            ) == null
        )
        {
            acessos.Add(
                new Acesso
                {
                    Nome = $"Aluguer Digital - Filme {filmeId}",
                    Descricao = "Aluguer individual do filme durante 48 horas.",
                    Tipo = TipoAcesso.AluguerDigital,
                    Preco = 3.99m,
                    FilmeId = filmeId,
                    DuracaoHoras = 48,
                    IsAtivo = true,
                    CriadoEm = DateTime.UtcNow,
                }
            );
        }

        var festivais = await _filmeRepository.ObterFestivaisDoFilmeAsync(filmeId);

        foreach (var festival in festivais)
        {
            if (
                await _acessoRepository.GetAtivoParaCarrinhoAsync(
                    TipoAcesso.PasseCompleto,
                    festival.Id,
                    null,
                    null,
                    null
                ) == null
            )
            {
                acessos.Add(
                    new Acesso
                    {
                        Nome = $"Passe Completo - {festival.Name}",
                        Descricao = "Passe valido para todo o festival.",
                        Tipo = TipoAcesso.PasseCompleto,
                        Preco = 24.99m,
                        FestivalId = festival.Id,
                        IsAtivo = true,
                        CriadoEm = DateTime.UtcNow,
                    }
                );
            }

            var totalDias = Math.Max(1, (festival.EndDate.Date - festival.StartDate.Date).Days + 1);
            for (var i = 0; i < totalDias; i++)
            {
                var dia = festival.StartDate.Date.AddDays(i);

                if (
                    await _acessoRepository.GetAtivoParaCarrinhoAsync(
                        TipoAcesso.PasseDiario,
                        festival.Id,
                        null,
                        null,
                        dia
                    ) != null
                )
                    continue;

                acessos.Add(
                    new Acesso
                    {
                        Nome = $"Passe Diario - {festival.Name} - {dia:dd/MM/yyyy}",
                        Descricao = "Passe valido para todas as sessoes de um dia do festival.",
                        Tipo = TipoAcesso.PasseDiario,
                        Preco = 9.99m,
                        FestivalId = festival.Id,
                        DataAcesso = dia,
                        IsAtivo = true,
                        CriadoEm = DateTime.UtcNow,
                    }
                );
            }
        }

        var sessoes = await _filmeRepository.ObterSessoesDoFilmeAsync(filmeId);

        foreach (var sessao in sessoes)
        {
            if (
                await _acessoRepository.GetAtivoParaCarrinhoAsync(
                    TipoAcesso.BilheteSessao,
                    null,
                    null,
                    sessao.Id,
                    null
                ) != null
            )
                continue;

            acessos.Add(
                new Acesso
                {
                    Nome = $"Bilhete - Sessao {sessao.Id}",
                    Descricao = "Bilhete valido para uma sessao especifica.",
                    Tipo = TipoAcesso.BilheteSessao,
                    Preco = sessao.TemChatAoVivo ? 5.99m : 4.99m,
                    SessaoId = sessao.Id,
                    FilmeId = sessao.FilmeId,
                    IsAtivo = true,
                    CriadoEm = DateTime.UtcNow,
                }
            );
        }

        if (acessos.Count > 0)
        {
            await _acessoRepository.AddManyAsync(acessos);
            await _acessoRepository.SaveChangesAsync();
        }
    }

    private static void ValidarReview(CriarAvaliacaoDTO dto)
    {
        if (dto.Pontuacao is < 1 or > 10)
            throw new ArgumentException("A pontuacao deve estar entre 1 e 10 estrelas.");

        var texto = dto.Texto?.Trim() ?? string.Empty;

        if (texto.Length < 20)
            throw new ArgumentException("A review deve ter pelo menos 20 caracteres.");

        if (texto.Length > 1000)
            throw new ArgumentException("A review nao pode exceder 1000 caracteres.");
    }

    private async Task AdicionarPessoaAsync(
        Filme filme,
        TmdbPessoaDTO? pessoaTmdb,
        FuncaoPessoaFilme funcao,
        int ordem
    )
    {
        if (pessoaTmdb == null || string.IsNullOrWhiteSpace(pessoaTmdb.Nome))
            return;

        var pessoa = await _filmeRepository.ObterOuCriarPessoaAsync(
            pessoaTmdb.TmdbPessoaId,
            pessoaTmdb.Nome,
            pessoaTmdb.ImagemUrl
        );

        if (
            filme.PessoasDoFilme.Any(fp =>
                fp.Pessoa == pessoa && fp.Funcao == funcao
            )
        )
            return;

        filme.PessoasDoFilme.Add(
            new FilmePessoa
            {
                Filme = filme,
                Pessoa = pessoa,
                Funcao = funcao,
                Personagem = pessoaTmdb.Personagem,
                Ordem = ordem,
            }
        );
    }

    private static string? CriarVideoUrl(string? provider, string? key)
    {
        if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(key))
            return null;

        return provider.Equals("YouTube", StringComparison.OrdinalIgnoreCase)
            ? $"https://www.youtube.com/embed/{key.Trim()}"
            : null;
    }

    private static string? NormalizarValor(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }
}
