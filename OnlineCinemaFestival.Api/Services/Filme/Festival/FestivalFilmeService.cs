using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class FestivalFilmeService : IFestivalFilmeService
{
    private readonly IFestivalFilmeRepository _festivalFilmeRepository;
    private readonly IFestivalRepository _festivalRepository;
    private readonly IFilmeRepository _filmeRepository;

    public FestivalFilmeService(
        IFestivalFilmeRepository festivalFilmeRepository,
        IFestivalRepository festivalRepository,
        IFilmeRepository filmeRepository
    )
    {
        _festivalFilmeRepository = festivalFilmeRepository;
        _festivalRepository = festivalRepository;
        _filmeRepository = filmeRepository;
    }

    public async Task<FestivalFilmeReadDTO> AssociarFilmeAsync(
        int festivalId,
        AssociarFilmeFestivalDTO dto
    )
    {
        var festival = await _festivalRepository.ObterPorIdAsync(festivalId);

        if (festival == null)
            throw new KeyNotFoundException("Festival nao encontrado.");

        var filme = await _filmeRepository.ObterPorIdAsync(dto.FilmeId);

        if (filme == null)
            throw new KeyNotFoundException("Filme nao encontrado.");

        var existeAssociacao = await _festivalFilmeRepository.ExisteAsync(
            festivalId,
            dto.FilmeId
        );

        if (existeAssociacao)
            throw new InvalidOperationException("Este filme ja esta associado a este festival.");

        var festivalFilme = new FestivalFilme
        {
            FestivalId = festivalId,
            FilmeId = dto.FilmeId,
            ElegivelPremiosPublico = dto.ElegivelPremiosPublico,
            Secao = NormalizarTextoOpcional(dto.Secao),
            Categoria = NormalizarTextoOpcional(dto.Categoria),
            DataAdicao = DateTime.UtcNow,
        };

        await _festivalFilmeRepository.AdicionarAsync(festivalFilme);
        await _festivalFilmeRepository.SaveChangesAsync();

        var associacao = await _festivalFilmeRepository.ObterAsync(festivalId, dto.FilmeId);
        return FestivalMapper.MapFestivalFilmeToReadDTO(associacao!);
    }

    public async Task RemoverFilmeAsync(int festivalId, int filmeId)
    {
        var festivalFilme = await _festivalFilmeRepository.ObterAsync(festivalId, filmeId);

        if (festivalFilme == null)
            throw new KeyNotFoundException("Associacao entre festival e filme nao encontrada.");

        _festivalFilmeRepository.Remove(festivalFilme);
        await _festivalFilmeRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<FilmeReadDTO>> ObterFilmesPorFestivalAsync(int festivalId)
    {
        var festival = await _festivalRepository.ObterPorIdAsync(festivalId);

        if (festival == null)
            throw new KeyNotFoundException("Festival nao encontrado.");

        var filmes = await _festivalFilmeRepository.ObterFilmesPorFestivalIdAsync(festivalId);
        return filmes.Select(FilmeMapper.MapToReadDTO);
    }

    public async Task<IEnumerable<FestivalFilmeReadDTO>> ObterAssociacoesPorFestivalAsync(
        int festivalId
    )
    {
        var festival = await _festivalRepository.ObterPorIdAsync(festivalId);

        if (festival == null)
            throw new KeyNotFoundException("Festival nao encontrado.");

        var associacoes = await _festivalFilmeRepository.ObterAssociacoesPorFestivalIdAsync(
            festivalId
        );

        return associacoes.Select(FestivalMapper.MapFestivalFilmeToReadDTO);
    }

    private static string? NormalizarTextoOpcional(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }
}
