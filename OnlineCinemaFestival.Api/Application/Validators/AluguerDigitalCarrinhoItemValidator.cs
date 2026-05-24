using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Common.Errors;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public sealed class AluguerDigitalCarrinhoItemValidator : CarrinhoItemValidatorBase
{
    private readonly IFilmeRepository _filmeRepository;
    private readonly IAcessoRepository _acessoRepository;

    public AluguerDigitalCarrinhoItemValidator(
        IFilmeRepository filmeRepository,
        IAcessoRepository acessoRepository
    )
    {
        _filmeRepository = filmeRepository;
        _acessoRepository = acessoRepository;
    }

    public override TipoAcesso Tipo => TipoAcesso.AluguerDigital;

    public override void ValidarPedido(CarrinhoItemCreateDTO dto)
    {
        if (dto.FilmeId is null)
            throw new RegraNegocioException("FilmeId e obrigatorio para aluguer digital.");

        GarantirSemAlvosExtras(dto, permiteFilme: true);
    }

    public override async Task ValidarAlvoAsync(CarrinhoItemCreateDTO dto)
    {
        if (await _filmeRepository.ObterPorIdAsync(dto.FilmeId!.Value) == null)
            throw new RecursoNaoEncontradoException("Filme nao encontrado.");
    }

    public override Task<Acesso?> ObterAcessoAtivoAsync(CarrinhoItemCreateDTO dto)
    {
        return _acessoRepository.ObterAluguerDigitalAtivoAsync(dto.FilmeId!.Value);
    }

    public override void ValidarAcesso(Acesso acesso)
    {
        if (acesso.FilmeId == null || acesso.Filme == null)
            throw new ConflitoDominioException("Aluguer digital sem filme associado.");

        if (acesso.DuracaoHoras is null or <= 0)
            throw new ConflitoDominioException("Aluguer digital com duracao invalida.");
    }
}
