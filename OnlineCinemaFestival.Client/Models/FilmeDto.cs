namespace OnlineCinemaFestival.Client.Models;

public class FilmeDTO
{
    public int Id { get; set; }

    public int TmdbId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string? TituloOriginal { get; set; }

    public string? Sinopse { get; set; }

    public DateTime DataLancamento { get; set; }

    public int? DuracaoMinutos { get; set; }

    public string? Genero { get; set; }

    public List<string> Generos { get; set; } = new();

    public string? Classificacao { get; set; }

    public double? AvaliacaoTmdb { get; set; }

    public string CapaUrl { get; set; } = string.Empty;

    public string? TrailerUrl { get; set; }

    public string? VideoProvider { get; set; }

    public string? VideoKey { get; set; }

    public string? VideoUrl { get; set; }

    public int? DuracaoVideoSegundos { get; set; }

    public string? ConteudoLocalPath { get; set; }

    public int Popularidade { get; set; }

    public string? Realizador { get; set; }

    public List<string> Atores { get; set; } = new();

    public PessoaFilmeDTO? RealizadorDetalhe { get; set; }

    public PessoaFilmeDTO? ProdutorDetalhe { get; set; }

    public List<PessoaFilmeDTO> AtoresDetalhes { get; set; } = new();

    public List<TmdbReviewDTO> ReviewsTmdb { get; set; } = new();

    public List<AvaliacaoDTO> ReviewsAplicacao { get; set; } = new();

    public List<FestivalDTO> Festivais { get; set; } = new();

    public string? Premios { get; set; }

    public List<SessaoDTO> Sessoes { get; set; } = new();

    public List<AcessoDTO> AcessosDisponiveis { get; set; } = new();

    public List<ResultadoPremioFestivalDTO> ResultadosPremiosPublicados { get; set; } = new();

    public bool PodeVer { get; set; }

    public bool PodeAvaliar { get; set; }

    // Compatibilidade com paginas antigas que usam UrlCartaz.
    public string UrlCartaz => CapaUrl;
}

public class TmdbReviewDTO
{
    public string Autor { get; set; } = string.Empty;

    public string Texto { get; set; } = string.Empty;

    public string? Url { get; set; }

    public DateTime? CriadaEm { get; set; }

    public double? Nota { get; set; }
}

public class PessoaFilmeDTO
{
    public int Id { get; set; }

    public int? TmdbPessoaId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? ImagemUrl { get; set; }

    public string Funcao { get; set; } = string.Empty;

    public string? Personagem { get; set; }

    public int Ordem { get; set; }
}

public class AtualizarVideoFilmeDTO
{
    public string? VideoProvider { get; set; }

    public string? VideoKey { get; set; }

    public string? VideoUrl { get; set; }

    public int? DuracaoVideoSegundos { get; set; }
}

public class AvaliacaoDTO
{
    public int Id { get; set; }

    public int FilmeId { get; set; }

    public string TituloFilme { get; set; } = string.Empty;

    public int UsuarioId { get; set; }

    public string NomeUsuario { get; set; } = string.Empty;

    public int Pontuacao { get; set; }

    public string Texto { get; set; } = string.Empty;

    public DateTime Data { get; set; }
}

public class CriarAvaliacaoDTO
{
    public int Pontuacao { get; set; }

    public string Texto { get; set; } = string.Empty;
}
