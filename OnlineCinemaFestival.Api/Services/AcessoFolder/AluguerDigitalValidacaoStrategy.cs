using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services.AcessosFolder;

public class AluguerDigitalValidacaoStrategy : IEstrategiaValidacaoAcesso
{
    private readonly IFilmeRepository _filmeRepository;

    public AluguerDigitalValidacaoStrategy(IFilmeRepository filmeRepository)
    {
        _filmeRepository = filmeRepository;
    }

    public TipoAcesso Tipo => TipoAcesso.AluguerDigital;

    public string Nome => "Aluguer digital";

    public string Descricao =>
        "Aluguer digital de um filme durante uma janela temporal, por exemplo 48 horas.";

    public async Task ValidarAsync(AcessoCreateDto dto)
    {
        if (!dto.FilmeId.HasValue)
            throw new ArgumentException("Um aluguer digital precisa de FilmeId.");

        var filme = await _filmeRepository.GetByIdAsync(dto.FilmeId.Value);

        if (filme == null)
            throw new KeyNotFoundException("Filme não encontrado.");

        if (dto.DuracaoHoras.HasValue && dto.DuracaoHoras.Value <= 0)
            throw new ArgumentException("A duração do aluguer deve ser superior a zero horas.");
    }
}
