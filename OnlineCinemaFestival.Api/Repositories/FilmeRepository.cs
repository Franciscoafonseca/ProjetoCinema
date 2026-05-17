using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public class FilmeRepository : IFilmeRepository
{
    private readonly AppDbContext _context;

    public FilmeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Filme>> ObterTodosAsync()
    {
        return await _context
            .Filmes.Include(f => f.FilmeGeneros)
                .ThenInclude(fg => fg.Genero)
            .Include(f => f.PessoasDoFilme)
                .ThenInclude(fp => fp.Pessoa)
            .Include(f => f.Avaliacoes)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Filme?> ObterPorIdAsync(int id)
    {
        return await _context.Filmes.FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Filme?> ObterDetalhePorIdAsync(int id)
    {
        return await _context
            .Filmes.AsSplitQuery()
            .Include(f => f.FilmeGeneros)
                .ThenInclude(fg => fg.Genero)
            .Include(f => f.PessoasDoFilme)
                .ThenInclude(fp => fp.Pessoa)
            .Include(f => f.Avaliacoes)
                .ThenInclude(a => a.Usuario)
            .Include(f => f.FestivalFilmes)
                .ThenInclude(ff => ff.Festival)
            .Include(f => f.SessoesDoFilme)
                .ThenInclude(sf => sf.Sessao)
                    .ThenInclude(s => s.Festival)
            .Include(f => f.Acessos)
            .Include(f => f.ResultadosPremiosFestival)
                .ThenInclude(r => r.PremioFestival)
                    .ThenInclude(p => p.Festival)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Filme?> ObterPorTmdbIdAsync(int tmdbId)
    {
        return await _context
            .Filmes.Include(f => f.FilmeGeneros)
                .ThenInclude(fg => fg.Genero)
            .Include(f => f.PessoasDoFilme)
                .ThenInclude(fp => fp.Pessoa)
            .Include(f => f.Avaliacoes)
            .FirstOrDefaultAsync(f => f.TmdbId == tmdbId);
    }

    public async Task<List<Filme>> ObterPrincipaisAsync(int quantidade)
    {
        return await _context
            .Filmes.Include(f => f.FilmeGeneros)
                .ThenInclude(fg => fg.Genero)
            .Include(f => f.PessoasDoFilme)
                .ThenInclude(fp => fp.Pessoa)
            .Include(f => f.Avaliacoes)
            .OrderByDescending(f => f.AvaliacaoTmdb ?? 0)
            .ThenBy(f => f.Titulo)
            .Take(quantidade)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Genero> ObterOuCriarGeneroAsync(string nome)
    {
        var nomeNormalizado = nome.Trim();
        var genero = await _context.Generos.FirstOrDefaultAsync(g => g.Name == nomeNormalizado);

        if (genero != null)
            return genero;

        genero = new Genero { Name = nomeNormalizado, CreatedAt = DateTime.UtcNow };
        await _context.Generos.AddAsync(genero);
        return genero;
    }

    public async Task<Pessoa> ObterOuCriarPessoaAsync(
        int? tmdbPessoaId,
        string nome,
        string? imagemUrl
    )
    {
        var nomeNormalizado = nome.Trim();

        var pessoa = tmdbPessoaId.HasValue
            ? await _context.Pessoas.FirstOrDefaultAsync(p => p.TmdbPessoaId == tmdbPessoaId.Value)
            : null;

        pessoa ??= await _context.Pessoas.FirstOrDefaultAsync(p => p.Nome == nomeNormalizado);

        if (pessoa != null)
        {
            if (
                string.IsNullOrWhiteSpace(pessoa.ImagemUrl) && !string.IsNullOrWhiteSpace(imagemUrl)
            )
                pessoa.ImagemUrl = imagemUrl;

            if (!pessoa.TmdbPessoaId.HasValue && tmdbPessoaId.HasValue)
                pessoa.TmdbPessoaId = tmdbPessoaId.Value;

            return pessoa;
        }

        pessoa = new Pessoa
        {
            TmdbPessoaId = tmdbPessoaId,
            Nome = nomeNormalizado,
            ImagemUrl = imagemUrl,
        };

        await _context.Pessoas.AddAsync(pessoa);
        return pessoa;
    }

    public async Task<bool> UtilizadorViuFilmeAsync(int utilizadorId, int filmeId)
    {
        return await _context.Visualizacoes.AnyAsync(v =>
            v.UtilizadorId == utilizadorId && v.FilmeId == filmeId
        );
    }

    public async Task<Avaliacao?> ObterAvaliacaoAsync(int utilizadorId, int filmeId)
    {
        return await _context.Avaliacoes.FirstOrDefaultAsync(a =>
            a.UsuarioId == utilizadorId && a.FilmeId == filmeId
        );
    }

    public async Task AddAvaliacaoAsync(Avaliacao avaliacao)
    {
        await _context.Avaliacoes.AddAsync(avaliacao);
    }

    public async Task AddAsync(Filme filme)
    {
        await _context.Filmes.AddAsync(filme);
    }

    public void AtualizarVideo(
        Filme filme,
        string? provider,
        string? key,
        string? url,
        int? duracaoSegundos
    )
    {
        filme.VideoProvider = provider;
        filme.VideoKey = key;
        filme.VideoUrl = url;
        filme.TrailerUrl = url;
        filme.DuracaoVideoSegundos = duracaoSegundos;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
