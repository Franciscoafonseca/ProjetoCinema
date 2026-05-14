using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services.Acesso;

public class BilheteSessaoValidacaoStrategy : IEstrategiaValidacaoAcesso
{
    private readonly ISessaoRepository _sessaoRepository;

    public BilheteSessaoValidacaoStrategy(ISessaoRepository sessaoRepository)
    {
        _sessaoRepository = sessaoRepository;
    }

    public TipoAcesso Tipo => TipoAcesso.BilheteSessao;

    public string Nome => "Bilhete de sessão";

    public string Descricao => "Bilhete válido para uma sessão específica.";

    public async Task ValidarAsync(AcessoCreateDto dto)
    {
        if (!dto.SessaoId.HasValue)
            throw new ArgumentException("Um bilhete de sessão precisa de SessaoId.");

        var sessao = await _sessaoRepository.GetByIdAsync(dto.SessaoId.Value);

        if (sessao == null)
            throw new KeyNotFoundException("Sessão não encontrada.");
    }
}
