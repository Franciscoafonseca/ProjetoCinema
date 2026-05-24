using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public interface ICarrinhoService
{
    Task<CarrinhoDTO> ObterAsync();
    Task<CarrinhoResumoDTO> ObterResumoAsync();
    Task<CarrinhoValidacaoDTO> ValidarAsync();
    Task<CarrinhoDTO> AdicionarItemAsync(int acessoId, int quantidade = 1);
    Task<CarrinhoDTO> AdicionarBilheteSessaoAsync(int sessaoId, int quantidade = 1);
    Task<CarrinhoDTO> AdicionarPasseDiarioAsync(int festivalId, DateTime dataPasse);
    Task<CarrinhoDTO> AdicionarPasseCompletoAsync(int festivalId);
    Task<CarrinhoDTO> AdicionarAluguerDigitalAsync(int filmeId);
    Task<CarrinhoDTO> AtualizarQuantidadeAsync(int itemId, int quantidade);
    Task RemoverItemAsync(int itemId);
    Task LimparAsync();
}
