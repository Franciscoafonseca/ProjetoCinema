namespace OnlineCinemaFestival.Api.Mapping;

using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Domain;

// Para mapear os dados do filme para o DTO que será enviado para o cliente
public static class FilmeMapper
{
    // Mapear um filme para o DTO de leitura
    // BD -> API
    public static FilmeReadDTO MapToReadDTO(Filme f) =>
        new FilmeReadDTO
        {
            Id = f.Id,
            TmdbId = f.TmdbId,
            Titulo = f.Titulo,
            TituloOriginal = f.TituloOriginal,
            Sinopse = f.Sinopse,
            DataLancamento = f.DataLancamento,
            DuracaoMinutos = f.DuracaoMinutos,
            Genero = f.Genero,
            Generos = f.FilmeGeneros.Select(fg => fg.Genero.Name).OrderBy(g => g).ToList(),
            Classificacao = f.Classificacao,
            ClassificacaoTmdb = f.Classificacao,
            AvaliacaoTmdb = f.AvaliacaoTmdb,
            AvaliacaoInternaMedia = f.Avaliacoes.Count == 0
                ? null
                : f.Avaliacoes.Average(a => a.Pontuacao),
            AvaliacoesInternasTotal = f.Avaliacoes.Count,
            VisualizacoesTotal = f.Visualizacoes.Count,
            CapaUrl = f.CapaUrl,
            TrailerUrl = f.TrailerUrl,
            VideoProvider = f.VideoProvider,
            VideoKey = f.VideoKey,
            VideoUrl = f.VideoUrl,
            Popularidade = f.Popularidade,
            Realizador = f.Realizador,
            Atores = SepararLista(f.AtoresPrincipais),
            RealizadorDetalhe = MapPessoa(
                f.PessoasDoFilme.FirstOrDefault(p =>
                    p.Funcao == FuncaoPessoaFilme.Realizador
                )
            ),
            ProdutorDetalhe = MapPessoa(
                f.PessoasDoFilme.FirstOrDefault(p => p.Funcao == FuncaoPessoaFilme.Produtor)
            ),
            AtoresDetalhes = f
                .PessoasDoFilme.Where(p => p.Funcao == FuncaoPessoaFilme.Ator)
                .OrderBy(p => p.Ordem)
                .Select(MapPessoa)
                .OfType<PessoaFilmeDTO>()
                .ToList(),
            ReviewsTmdb = DesserializarReviews(f.TmdbReviewsJson),
            Premios = f.Premios,
            ReviewsAplicacao = f
                .Avaliacoes.Select(a => new AvaliacaoDTO
                {
                    Id = a.Id,
                    FilmeId = a.FilmeId,
                    TituloFilme = f.Titulo,
                    UsuarioId = a.UsuarioId,
                    NomeUsuario = a.Usuario?.Name ?? string.Empty,
                    Pontuacao = a.Pontuacao,
                    Texto = a.Texto,
                    Data = a.Data,
                })
                .OrderByDescending(a => a.Data)
                .ToList(),
            Festivais = f
                .FestivalFilmes.Select(ff => new FestivalResumoDTO
                {
                    Id = ff.FestivalId,
                    Name = ff.Festival?.Name ?? string.Empty,
                    StartDate = ff.Festival?.StartDate ?? default,
                    EndDate = ff.Festival?.EndDate ?? default,
                })
                .ToList(),
            Sessoes = f
                .Sessoes.Select(s => new SessaoResumoDTO
                {
                    Id = s.Id,
                    FestivalId = s.FestivalId,
                    FestivalName = s.Festival?.Name ?? string.Empty,
                    NomeFestival = s.Festival?.Name ?? string.Empty,
                    FilmeId = s.FilmeId,
                    TituloFilme = f.Titulo,
                    FilmeTitulo = f.Titulo,
                    Tipo = s.Tipo,
                    TipoNome = s.Tipo.ToString(),
                    Inicio = s.Inicio,
                    Fim = s.Fim,
                    Estado = SessaoMapper.ObterEstado(s.Inicio, s.Fim),
                    TemChatAoVivo = s.TemChatAoVivo,
                    PrecoBilhete = s
                        .Acessos.Where(a => a.IsAtivo && a.Tipo == TipoAcesso.BilheteSessao)
                        .OrderBy(a => a.Preco)
                        .Select(a => (decimal?)a.Preco)
                        .FirstOrDefault(),
                    Observacoes = s.Observacoes,
                })
                .OrderBy(s => s.Inicio)
                .ToList(),
            AcessosDisponiveis = f
                .Acessos.Where(a => a.IsAtivo)
                .Select(AcessoMapper.MapToReadDTO)
                .ToList(),
            ResultadosPremiosPublicados = f
                .ResultadosPremiosFestival.Where(r =>
                    r.PremioFestival.EstadoPremio == EstadoPremio.Publicado
                )
                .OrderByDescending(r => r.PublicadoEm)
                .Select(PremioFestivalMapper.MapResultadoToDTO)
                .ToList(),
        };

    // TMDB -> BD
    public static Filme MapFromTmdbDTO(TmdbFilmeDTO f) =>
        new Filme
        {
            TmdbId = f.TmdbId,
            Titulo = f.Titulo,
            TituloOriginal = f.TituloOriginal,
            Sinopse = f.Sinopse,
            DataLancamento = f.DataLancamento,
            DuracaoMinutos = f.DuracaoMinutos,
            Genero = f.Genero,
            AvaliacaoTmdb = f.AvaliacaoTmdb,
            Classificacao = f.Classificacao,
            CapaUrl = f.CapaUrl,
            TrailerUrl = f.TrailerUrl,
            VideoProvider = f.VideoProvider,
            VideoKey = f.VideoKey,
            VideoUrl = f.VideoUrl,
            Realizador = f.Realizador,
            AtoresPrincipais = string.Join(", ", f.Atores),
            TmdbReviewsJson = System.Text.Json.JsonSerializer.Serialize(f.Reviews),
        };

    // TMDB -> API (para resultados de busca, que são mais leves que os detalhes)
    public static FilmeReadDTO MapToReadDTOFromTmdb(TmdbFilmeDTO f) =>
        new FilmeReadDTO
        {
            TmdbId = f.TmdbId,
            Titulo = f.Titulo,
            TituloOriginal = f.TituloOriginal,
            Sinopse = f.Sinopse,
            DataLancamento = f.DataLancamento,
            Genero = f.Genero,
            Generos = f.Generos,
            Classificacao = f.Classificacao,
            ClassificacaoTmdb = f.Classificacao,
            AvaliacaoTmdb = f.AvaliacaoTmdb,
            CapaUrl = f.CapaUrl,
            TrailerUrl = f.TrailerUrl,
            VideoProvider = f.VideoProvider,
            VideoKey = f.VideoKey,
            VideoUrl = f.VideoUrl,
            DuracaoMinutos = f.DuracaoMinutos,
        };

    // Genero fica vazio na pesquisa; so e preenchido no detalhe do filme.
    public static TmdbFilmeDTO MapFromTmdbResult(TmdbFilmeResult r) =>
        new TmdbFilmeDTO
        {
            TmdbId = r.TmdbId,
            Titulo = r.Titulo,
            Sinopse = r.Sinopse,
            DataLancamento = DateTime.TryParse(r.DataLancamento, out var date)
                ? date
                : DateTime.MinValue,
            Classificacao = r.Classificacao?.ToString("0.0"),
            AvaliacaoTmdb = r.Classificacao,
            CapaUrl = string.IsNullOrWhiteSpace(r.CapaUrl)
                ? ""
                : $"https://image.tmdb.org/t/p/w500{r.CapaUrl}",
            Genero = "", // genero_ids precisa de tabela/lookup
        };

    private static PessoaFilmeDTO? MapPessoa(FilmePessoa? filmePessoa)
    {
        if (filmePessoa?.Pessoa == null)
            return null;

        return new PessoaFilmeDTO
        {
            Id = filmePessoa.Pessoa.Id,
            TmdbPessoaId = filmePessoa.Pessoa.TmdbPessoaId,
            Nome = filmePessoa.Pessoa.Nome,
            ImagemUrl = filmePessoa.Pessoa.ImagemUrl,
            Funcao = filmePessoa.Funcao.ToString(),
            Personagem = filmePessoa.Personagem,
            Ordem = filmePessoa.Ordem,
        };
    }

    private static List<string> SepararLista(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? new List<string>()
            : valor
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
    }

    private static List<TmdbReviewDTO> DesserializarReviews(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new List<TmdbReviewDTO>();

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<TmdbReviewDTO>>(json)
                ?? new List<TmdbReviewDTO>();
        }
        catch
        {
            return new List<TmdbReviewDTO>();
        }
    }
}
