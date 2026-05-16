using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComprasController : ControllerBase
{
    private readonly ICinemaFacade _cinemaFacade;
    private readonly ICompraRepository _compraRepository;

    // Injetamos a Facade para respeitar os padrões de desenho
    public ComprasController(ICinemaFacade cinemaFacade, ICompraRepository compraRepository)
    {
        _cinemaFacade = cinemaFacade;
        _compraRepository = compraRepository;
    }

    // Endpoint para o Carrinho do Blazor enviar a compra[cite: 1, 2]
    [HttpPost("finalizar")]
    public async Task<IActionResult> FinalizarCompra([FromBody] CompraRequest request)
    {
        // A Facade trata de tudo: Strategy, Observer e Repositories
        await _cinemaFacade.ComprarItens(request.UtilizadorId, request.Itens);

        return Ok(new { mensagem = "Compra concluida com sucesso!" });
    }

    // Endpoint para o utilizador ver o seu histórico no Perfil.razor
    [HttpGet("historico/{utilizadorId}")]
    public async Task<IActionResult> ObterHistorico(string utilizadorId)
    {
        var compras = await _compraRepository.GetByUtilizadorAsync(utilizadorId);
        var dto = compras.Select(c => new CompraHistoricoReadDto
        {
            Id = c.Id,
            UtilizadorId = c.UtilizadorId,
            Data = c.Data,
            Total = c.Total,
            PontosGanhos = c.PontosGanhos,
            Itens = c.Itens.Select(i => new CompraHistoricoItemReadDto
            {
                Id = i.Id,
                FilmeId = i.FilmeId,
                SessaoId = i.SessaoId,
                TipoAcesso = (int)i.TipoAcesso,
                PrecoPago = i.PrecoPago,
                Validade = i.Validade
            }).ToList()
        });

        return Ok(dto);
    }
}
