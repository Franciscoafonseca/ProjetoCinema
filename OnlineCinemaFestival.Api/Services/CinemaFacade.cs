using OnlineCinemaFestival.Api.DTOs;
using ModelAcesso = OnlineCinemaFestival.Api.Models.Acesso;
using OnlineCinemaFestival.Api.Models;
namespace OnlineCinemaFestival.Api.Services;

public class CinemaFacade : ICinemaFacade
{
    private readonly ICompraService _compraService;
    private readonly ICompraValidator _validator;
    private readonly ICompraHistoricoService _historicoService;
    // Aqui injetarias os repositórios que já criaste
    // private readonly IAcessoRepository _acessoRepo; 

    public CinemaFacade(
        ICompraService compraService,
        ICompraValidator validator,
        ICompraHistoricoService historicoService)
    {
        _compraService = compraService;
        _validator = validator;
        _historicoService = historicoService;
    }

    // Este é o único método que o Controller precisa de conhecer
    public async Task ComprarItens(string utilizadorId, List<CompraItemDto> itens)
    {
        _validator.Validar(new CompraRequest { UtilizadorId = utilizadorId, Itens = itens });

        // 1. Processa a lógica de negócio (Strategy + Observer)
        var resultado = await _compraService.FinalizarProcessoCompra(utilizadorId, itens);

        // 2. Registo de historico (pode ser expandido para persistencia real)
        await _historicoService.RegistarAsync(
            utilizadorId,
            resultado.Total,
            resultado.PontosGanhos,
            resultado.Itens);
    }

    public bool VerificarPermissaoAssistir(ModelAcesso acesso)
    {
        if (acesso.Tipo == TipoAcesso.AluguerDigital)
        {
            // Usa o Decorator para validar a janela de 48h
            var decorador = new Aluguer48hDecorator(acesso);
            return decorador.TemAcesso();
        }

        return true; // Outros tipos têm lógica simplificada
    }
}