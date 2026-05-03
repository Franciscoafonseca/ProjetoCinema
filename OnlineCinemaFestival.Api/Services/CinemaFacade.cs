using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class CinemaFacade
{
    private readonly CompraService _compraService;
    // Aqui injetarias os repositórios que já criaste
    // private readonly IAcessoRepository _acessoRepo; 

    public CinemaFacade(CompraService compraService)
    {
        _compraService = compraService;
    }

    // Este é o único método que o Controller precisa de conhecer
    public async Task ComprarItens(string utilizadorId, List<CompraItemDto> itens)
    {
        // 1. Processa a lógica de negócio (Strategy + Observer)
        await _compraService.FinalizarProcessoCompra(utilizadorId, itens);

        // 2. Aqui a Facade coordenaria a gravação final na DB usando os teus Repositories
    }

    public bool VerificarPermissaoAssistir(Acesso acesso)
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