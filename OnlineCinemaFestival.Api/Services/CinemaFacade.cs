using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using ModelAcesso = OnlineCinemaFestival.Api.Models.Acesso;

namespace OnlineCinemaFestival.Api.Services;

public class CinemaFacade : ICinemaFacade
{
    private readonly ICompraService _compraService;
    private readonly ICompraValidator _validator;
    private readonly ICompraHistoricoService _historicoService;
    private readonly IAcessoVisualizacaoService _acessoVisualizacaoService;

    public CinemaFacade(
        ICompraService compraService,
        ICompraValidator validator,
        ICompraHistoricoService historicoService,
        IAcessoVisualizacaoService acessoVisualizacaoService
    )
    {
        _compraService = compraService;
        _validator = validator;
        _historicoService = historicoService;
        _acessoVisualizacaoService = acessoVisualizacaoService;
    }

    public async Task ComprarItens(int utilizadorId, List<CompraItemDto> itens)
    {
        _validator.Validar(new CompraRequest { UtilizadorId = utilizadorId, Itens = itens });

        var resultado = await _compraService.FinalizarProcessoCompraAsync(utilizadorId, itens);

        await _historicoService.RegistarAsync(
            utilizadorId,
            resultado.Total,
            resultado.PontosGanhos,
            resultado.Itens
        );
    }

    public bool VerificarPermissaoAssistir(ModelAcesso acesso)
    {
        return _acessoVisualizacaoService.PodeVisualizar(acesso);
    }
}
