using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComprasController : ControllerBase
{
    private readonly CinemaFacade _cinemaFacade;

    // Injetamos a Facade para respeitar os padrões de desenho
    public ComprasController(CinemaFacade cinemaFacade)
    {
        _cinemaFacade = cinemaFacade;
    }

    // Endpoint para o Carrinho do Blazor enviar a compra[cite: 1, 2]
    [HttpPost("finalizar")]
    public async Task<IActionResult> FinalizarCompra([FromBody] CompraRequest request)
    {
        if (request == null)
        {
            return BadRequest("Pedido invalido.");
        }

        if (string.IsNullOrWhiteSpace(request.UtilizadorId))
        {
            return BadRequest("UtilizadorId e obrigatorio.");
        }

        if (request.Itens.Count == 0)
        {
            return BadRequest("O carrinho está vazio.");
        }

        foreach (var item in request.Itens)
        {
            if (item.Tipo == TipoAcesso.BilheteUnico || item.Tipo == TipoAcesso.AluguerDigital)
            {
                if (!item.FilmeId.HasValue)
                {
                    return BadRequest("FilmeId e obrigatorio para BilheteUnico e AluguerDigital.");
                }
            }

            if (item.Tipo == TipoAcesso.BilheteUnico && !item.SessaoId.HasValue)
            {
                return BadRequest("SessaoId e obrigatorio para BilheteUnico.");
            }
        }

        try
        {
            // A Facade trata de tudo: Strategy, Observer e Repositories
            await _cinemaFacade.ComprarItens(request.UtilizadorId, request.Itens);

            return Ok(new { mensagem = "Compra concluída com sucesso!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao processar compra: {ex.Message}");
        }
    }

    // Endpoint para o utilizador ver o seu histórico no Perfil.razor
    [HttpGet("historico/{utilizadorId}")]
    public async Task<IActionResult> ObterHistorico(string utilizadorId)
    {
        // Aqui chamarias a Facade ou um Repository para ir à DB[cite: 1, 2]
        return Ok("Lista de compras do utilizador");
    }
}
