using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class PremioFestivalService : IPremioFestivalService
{
    private readonly AppDbContext _db;

    public PremioFestivalService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PremioFestivalReadDTO> CriarPremioAsync(
        int festivalId,
        CriarPremioFestivalDTO dto
    )
    {
        ValidarDadosPremio(dto);

        var festivalExiste = await _db.Festivals.AnyAsync(f => f.Id == festivalId);

        if (!festivalExiste)
            throw new KeyNotFoundException("Festival nao encontrado.");

        var premio = new PremioFestival
        {
            FestivalId = festivalId,
            Nome = dto.Nome.Trim(),
            Descricao = dto.Descricao.Trim(),
            DataAberturaVotacao = dto.DataAberturaVotacao.ToUniversalTime(),
            DataFechoVotacao = dto.DataFechoVotacao.ToUniversalTime(),
            EstadoPremio = EstadoPremio.Rascunho,
        };

        await _db.PremiosFestival.AddAsync(premio);
        await _db.SaveChangesAsync();

        return PremioFestivalMapper.MapToReadDTO(premio);
    }

    public async Task<PremioFestivalReadDTO> AbrirVotacaoAsync(int premioFestivalId)
    {
        var premio = await ObterPremioAsync(premioFestivalId);

        if (premio.EstadoPremio == EstadoPremio.Publicado)
            throw new InvalidOperationException("Resultados ja publicados para este premio.");

        if (premio.DataFechoVotacao <= DateTime.UtcNow)
            throw new InvalidOperationException("Nao e possivel abrir uma votacao ja terminada.");

        premio.EstadoPremio = EstadoPremio.Aberto;
        await _db.SaveChangesAsync();

        return PremioFestivalMapper.MapToReadDTO(premio);
    }

    public async Task VotarAsync(int premioFestivalId, int filmeId, int utilizadorId)
    {
        var premio = await ObterPremioAsync(premioFestivalId);
        ValidarVotacaoAberta(premio);

        var filmeElegivel = await _db.FestivalFilmes.AnyAsync(ff =>
            ff.FestivalId == premio.FestivalId
            && ff.FilmeId == filmeId
            && ff.ElegivelPremiosPublico
        );

        if (!filmeElegivel)
            throw new InvalidOperationException(
                "O filme nao pertence ao festival ou nao esta elegivel para premios."
            );

        var jaVotou = await _db.VotosPremiosFestival.AnyAsync(v =>
            v.PremioFestivalId == premioFestivalId && v.UtilizadorId == utilizadorId
        );

        if (jaVotou)
            throw new InvalidOperationException("Ja votaste neste premio.");

        await _db.VotosPremiosFestival.AddAsync(
            new VotoPremioFestival
            {
                PremioFestivalId = premioFestivalId,
                FestivalId = premio.FestivalId,
                FilmeId = filmeId,
                UtilizadorId = utilizadorId,
                DataVoto = DateTime.UtcNow,
            }
        );

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("Ja votaste neste premio.");
        }
    }

    public async Task<PremioFestivalReadDTO> FecharVotacaoAsync(int premioFestivalId)
    {
        var premio = await ObterPremioAsync(premioFestivalId);

        if (premio.EstadoPremio == EstadoPremio.Publicado)
            throw new InvalidOperationException("Resultados ja publicados para este premio.");

        premio.EstadoPremio = EstadoPremio.Fechado;
        await _db.SaveChangesAsync();

        return PremioFestivalMapper.MapToReadDTO(premio);
    }

    public async Task<ResultadoPremioFestivalDTO> PublicarResultadosAsync(
        int premioFestivalId,
        int publicadoPorUtilizadorId
    )
    {
        var premio = await ObterPremioComResultadoAsync(premioFestivalId);

        if (premio.EstadoPremio != EstadoPremio.Fechado)
            throw new InvalidOperationException("Fecha a votacao antes de publicar resultados.");

        var vencedor = await _db
            .VotosPremiosFestival.Where(v => v.PremioFestivalId == premioFestivalId)
            .GroupBy(v => v.FilmeId)
            .Select(g => new { FilmeId = g.Key, Total = g.Count() })
            .OrderByDescending(g => g.Total)
            .ThenBy(g => g.FilmeId)
            .FirstOrDefaultAsync();

        if (vencedor == null)
            throw new InvalidOperationException("Nao existem votos para publicar resultados.");

        var resultado =
            premio.Resultado
            ?? new ResultadoPremioFestival { PremioFestivalId = premioFestivalId };

        resultado.FilmeIdVencedor = vencedor.FilmeId;
        resultado.TotalVotos = vencedor.Total;
        resultado.PublicadoEm = DateTime.UtcNow;
        resultado.PublicadoPorUtilizadorId = publicadoPorUtilizadorId;

        if (premio.Resultado == null)
            await _db.ResultadosPremiosFestival.AddAsync(resultado);

        premio.EstadoPremio = EstadoPremio.Publicado;
        await _db.SaveChangesAsync();

        return (
            await _db
                .ResultadosPremiosFestival.Include(r => r.PremioFestival)
                    .ThenInclude(p => p.Festival)
                .Include(r => r.FilmeVencedor)
                .FirstAsync(r => r.PremioFestivalId == premioFestivalId)
        ).ToResultadoDto();
    }

    public async Task<IEnumerable<ResultadoPremioFestivalDTO>> ObterResultadosPublicosAsync(
        int? festivalId = null,
        int? filmeId = null
    )
    {
        var query = _db
            .ResultadosPremiosFestival.Include(r => r.PremioFestival)
                .ThenInclude(p => p.Festival)
            .Include(r => r.FilmeVencedor)
            .Where(r => r.PremioFestival.EstadoPremio == EstadoPremio.Publicado);

        if (festivalId.HasValue)
            query = query.Where(r => r.PremioFestival.FestivalId == festivalId.Value);

        if (filmeId.HasValue)
            query = query.Where(r => r.FilmeIdVencedor == filmeId.Value);

        var resultados = await query
            .OrderByDescending(r => r.PublicadoEm)
            .ThenBy(r => r.PremioFestival.Nome)
            .AsNoTracking()
            .ToListAsync();

        return resultados.Select(PremioFestivalMapper.MapResultadoToDTO);
    }

    public async Task<IEnumerable<PremioFestivalReadDTO>> ObterPremiosPorFestivalAsync(
        int festivalId,
        bool incluirRascunhos
    )
    {
        var festivalExiste = await _db.Festivals.AnyAsync(f => f.Id == festivalId);

        if (!festivalExiste)
            throw new KeyNotFoundException("Festival nao encontrado.");

        var query = _db.PremiosFestival.Where(p => p.FestivalId == festivalId);

        if (!incluirRascunhos)
            query = query.Where(p => p.EstadoPremio != EstadoPremio.Rascunho);

        var premios = await query
            .OrderBy(p => p.DataAberturaVotacao)
            .ThenBy(p => p.Nome)
            .AsNoTracking()
            .ToListAsync();

        return premios.Select(PremioFestivalMapper.MapToReadDTO);
    }

    private async Task<PremioFestival> ObterPremioAsync(int premioFestivalId)
    {
        return await _db.PremiosFestival.FirstOrDefaultAsync(p => p.Id == premioFestivalId)
            ?? throw new KeyNotFoundException("Premio nao encontrado.");
    }

    private async Task<PremioFestival> ObterPremioComResultadoAsync(int premioFestivalId)
    {
        return await _db
                .PremiosFestival.Include(p => p.Resultado)
                .FirstOrDefaultAsync(p => p.Id == premioFestivalId)
            ?? throw new KeyNotFoundException("Premio nao encontrado.");
    }

    private static void ValidarDadosPremio(CriarPremioFestivalDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("O nome do premio e obrigatorio.");

        if (dto.DataFechoVotacao.ToUniversalTime() <= dto.DataAberturaVotacao.ToUniversalTime())
            throw new ArgumentException("A data de fecho deve ser posterior a data de abertura.");
    }

    private static void ValidarVotacaoAberta(PremioFestival premio)
    {
        var agora = DateTime.UtcNow;

        if (premio.EstadoPremio != EstadoPremio.Aberto)
            throw new InvalidOperationException("A votacao nao esta aberta.");

        if (agora < premio.DataAberturaVotacao || agora > premio.DataFechoVotacao)
            throw new InvalidOperationException("A votacao esta fora do periodo permitido.");
    }
}

internal static class ResultadoPremioFestivalExtensions
{
    public static ResultadoPremioFestivalDTO ToResultadoDto(this ResultadoPremioFestival resultado)
    {
        return PremioFestivalMapper.MapResultadoToDTO(resultado);
    }
}
