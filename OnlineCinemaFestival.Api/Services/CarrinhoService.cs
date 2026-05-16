using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class CarrinhoService : ICarrinhoService
{
    private const int QuantidadeMaxima = 99;

    private readonly ICarrinhoRepository _carrinhoRepository;
    private readonly IAcessoRepository _acessoRepository;
    private readonly IAcessoUtilizadorRepository _acessoUtilizadorRepository;
    private readonly ISessaoRepository _sessaoRepository;
    private readonly IFestivalRepository _festivalRepository;
    private readonly IFilmeRepository _filmeRepository;

    public CarrinhoService(
        ICarrinhoRepository carrinhoRepository,
        IAcessoRepository acessoRepository,
        IAcessoUtilizadorRepository acessoUtilizadorRepository,
        ISessaoRepository sessaoRepository,
        IFestivalRepository festivalRepository,
        IFilmeRepository filmeRepository
    )
    {
        _carrinhoRepository = carrinhoRepository;
        _acessoRepository = acessoRepository;
        _acessoUtilizadorRepository = acessoUtilizadorRepository;
        _sessaoRepository = sessaoRepository;
        _festivalRepository = festivalRepository;
        _filmeRepository = filmeRepository;
    }

    public async Task<CarrinhoReadDTO> ObterCarrinhoAsync(int utilizadorId)
    {
        var carrinho = await _carrinhoRepository.ObterOuCriarPorUtilizadorIdAsync(utilizadorId);

        return CarrinhoMapper.MapToReadDTO(carrinho);
    }

    public async Task<CarrinhoReadDTO> AdicionarItemAsync(
        int utilizadorId,
        AdicionarItemCarrinhoDTO dto
    )
    {
        if (dto.AcessoId <= 0)
            throw new ArgumentException("O acesso indicado e invalido.");

        var acesso = await _acessoRepository.ObterPorIdAsync(dto.AcessoId);

        if (acesso == null)
            throw new KeyNotFoundException("Acesso nao encontrado.");

        return await AdicionarAcessoAoCarrinhoAsync(utilizadorId, acesso, dto.Quantidade);
    }

    public async Task<CarrinhoReadDTO> AdicionarItemAsync(
        int utilizadorId,
        CarrinhoItemCreateDTO dto
    )
    {
        ValidarPedidoCriacao(dto);
        await ValidarAlvoPedidoCriacaoAsync(dto);

        var acesso = await _acessoRepository.GetAtivoParaCarrinhoAsync(
            dto.TipoAcesso,
            dto.FestivalId,
            dto.FilmeId,
            dto.SessaoId,
            dto.DataPasse
        );

        if (acesso == null)
            throw new KeyNotFoundException(
                "Nao existe um acesso ativo configurado para este item."
            );

        return await AdicionarAcessoAoCarrinhoAsync(utilizadorId, acesso, dto.Quantidade);
    }

    public async Task<CarrinhoReadDTO> AtualizarItemAsync(
        int utilizadorId,
        int itemId,
        CarrinhoItemUpdateDTO dto
    )
    {
        if (dto.Quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");

        if (dto.Quantidade > QuantidadeMaxima)
            throw new ArgumentException($"A quantidade nao pode exceder {QuantidadeMaxima}.");

        var carrinho = await _carrinhoRepository.ObterPorUtilizadorIdAsync(utilizadorId);

        if (carrinho == null)
            throw new KeyNotFoundException("Carrinho nao encontrado.");

        var item = await _carrinhoRepository.ObterItemAsync(carrinho.Id, itemId);

        if (item == null)
            throw new KeyNotFoundException("Item nao encontrado no carrinho.");

        if (item.Acesso.Tipo != TipoAcesso.BilheteSessao && dto.Quantidade != 1)
            throw new InvalidOperationException(
                "Apenas bilhetes de sessao permitem quantidade superior a 1."
            );

        item.Quantidade = dto.Quantidade;
        carrinho.AtualizadoEm = DateTime.UtcNow;

        await _carrinhoRepository.SaveChangesAsync();

        var atualizado = await _carrinhoRepository.ObterPorUtilizadorIdAsync(utilizadorId);
        return CarrinhoMapper.MapToReadDTO(atualizado!);
    }

    public async Task RemoverItemAsync(int utilizadorId, int itemId)
    {
        var carrinho = await _carrinhoRepository.ObterPorUtilizadorIdAsync(utilizadorId);

        if (carrinho == null)
            throw new KeyNotFoundException("Carrinho nao encontrado.");

        var item = await _carrinhoRepository.ObterItemAsync(carrinho.Id, itemId);

        if (item == null)
            throw new KeyNotFoundException("Item nao encontrado no carrinho.");

        _carrinhoRepository.RemoveItem(item);

        carrinho.AtualizadoEm = DateTime.UtcNow;

        await _carrinhoRepository.SaveChangesAsync();
    }

    public async Task LimparCarrinhoAsync(int utilizadorId)
    {
        var carrinho = await _carrinhoRepository.ObterPorUtilizadorIdAsync(utilizadorId);

        if (carrinho == null)
            return;

        _carrinhoRepository.RemoveItems(carrinho.Itens);

        carrinho.AtualizadoEm = DateTime.UtcNow;

        await _carrinhoRepository.SaveChangesAsync();
    }

    public async Task<CarrinhoValidacaoDTO> ValidarCarrinhoAsync(int utilizadorId)
    {
        var carrinho = await _carrinhoRepository.ObterPorUtilizadorIdAsync(utilizadorId);
        var resultado = new CarrinhoValidacaoDTO();

        if (carrinho == null || !carrinho.Itens.Any())
        {
            resultado.Avisos.Add("O carrinho esta vazio.");
            return resultado;
        }

        foreach (var item in carrinho.Itens)
        {
            try
            {
                ValidarAcessoParaCarrinho(item.Acesso, item.Quantidade);
                await ValidarAcessoJaCompradoAsync(utilizadorId, item.Acesso);
            }
            catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
            {
                resultado.Erros.Add(
                    new CarrinhoErroValidacaoDTO
                    {
                        ItemId = item.Id,
                        Campo = item.Acesso.Tipo.ToString(),
                        Mensagem = ex.Message,
                    }
                );
            }
        }

        resultado.Total = carrinho.Itens.Sum(i => i.PrecoUnitario * i.Quantidade);

        return resultado;
    }

    public async Task<CarrinhoResumoDTO> ObterResumoAsync(int utilizadorId)
    {
        var carrinho = await _carrinhoRepository.ObterOuCriarPorUtilizadorIdAsync(utilizadorId);
        var dto = CarrinhoMapper.MapToReadDTO(carrinho);

        return new CarrinhoResumoDTO
        {
            CarrinhoId = dto.Id,
            NumeroItens = dto.Itens.Sum(i => i.Quantidade),
            Subtotal = dto.Total,
            Itens = dto.Itens,
        };
    }

    private async Task<CarrinhoReadDTO> AdicionarAcessoAoCarrinhoAsync(
        int utilizadorId,
        Acesso acesso,
        int quantidade
    )
    {
        ValidarAcessoParaCarrinho(acesso, quantidade);
        await ValidarAcessoJaCompradoAsync(utilizadorId, acesso);

        var carrinho = await _carrinhoRepository.ObterOuCriarPorUtilizadorIdAsync(utilizadorId);
        var itemExistente = await _carrinhoRepository.ObterItemPorAcessoAsync(carrinho.Id, acesso.Id);

        if (itemExistente != null)
        {
            if (acesso.Tipo != TipoAcesso.BilheteSessao)
                throw new InvalidOperationException("Este acesso ja esta no carrinho.");

            if (itemExistente.Quantidade + quantidade > QuantidadeMaxima)
                throw new InvalidOperationException(
                    $"A quantidade total deste bilhete nao pode exceder {QuantidadeMaxima}."
                );

            itemExistente.Quantidade += quantidade;
            carrinho.AtualizadoEm = DateTime.UtcNow;
            await _carrinhoRepository.SaveChangesAsync();

            var carrinhoComItemAtualizado = await _carrinhoRepository.ObterPorUtilizadorIdAsync(
                utilizadorId
            );
            return CarrinhoMapper.MapToReadDTO(carrinhoComItemAtualizado!);
        }

        var item = new CarrinhoItem
        {
            CarrinhoId = carrinho.Id,
            AcessoId = acesso.Id,
            PrecoUnitario = acesso.Preco,
            Quantidade = quantidade,
            DataAdicao = DateTime.UtcNow,
        };

        await _carrinhoRepository.AddItemAsync(item);

        carrinho.AtualizadoEm = DateTime.UtcNow;

        await _carrinhoRepository.SaveChangesAsync();

        var carrinhoAtualizado = await _carrinhoRepository.ObterPorUtilizadorIdAsync(utilizadorId);

        return CarrinhoMapper.MapToReadDTO(carrinhoAtualizado!);
    }

    private async Task ValidarAcessoJaCompradoAsync(int utilizadorId, Acesso acesso)
    {
        var jaPossuiAcesso = await _acessoUtilizadorRepository.ExisteAcessoAtivoAsync(
            utilizadorId,
            acesso.Id,
            DateTime.UtcNow
        );

        if (jaPossuiAcesso)
            throw new InvalidOperationException("O utilizador ja possui este acesso ativo.");
    }

    private static void ValidarPedidoCriacao(CarrinhoItemCreateDTO dto)
    {
        if (dto.Quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");

        if (dto.Quantidade > QuantidadeMaxima)
            throw new ArgumentException($"A quantidade nao pode exceder {QuantidadeMaxima}.");

        switch (dto.TipoAcesso)
        {
            case TipoAcesso.BilheteSessao:
                if (dto.SessaoId is null)
                    throw new ArgumentException("SessaoId e obrigatorio para bilhete de sessao.");
                if (
                    dto.FestivalId is not null
                    || dto.FilmeId is not null
                    || dto.DataPasse is not null
                )
                    throw new ArgumentException(
                        "Bilhete de sessao deve indicar apenas SessaoId como alvo."
                    );
                break;

            case TipoAcesso.PasseDiario:
                if (dto.FestivalId is null || dto.DataPasse is null)
                    throw new ArgumentException(
                        "FestivalId e DataPasse sao obrigatorios para passe diario."
                    );
                if (dto.SessaoId is not null || dto.FilmeId is not null)
                    throw new ArgumentException(
                        "Passe diario deve indicar apenas FestivalId e DataPasse como alvo."
                    );
                break;

            case TipoAcesso.PasseCompleto:
                if (dto.FestivalId is null)
                    throw new ArgumentException("FestivalId e obrigatorio para passe completo.");
                if (
                    dto.SessaoId is not null
                    || dto.FilmeId is not null
                    || dto.DataPasse is not null
                )
                    throw new ArgumentException(
                        "Passe completo deve indicar apenas FestivalId como alvo."
                    );
                break;

            case TipoAcesso.AluguerDigital:
                if (dto.FilmeId is null)
                    throw new ArgumentException("FilmeId e obrigatorio para aluguer digital.");
                if (dto.SessaoId is not null || dto.FestivalId is not null || dto.DataPasse is not null)
                    throw new ArgumentException(
                        "Aluguer digital deve indicar apenas FilmeId como alvo."
                    );
                break;

            default:
                throw new ArgumentException("Tipo de acesso invalido.");
        }
    }

    private async Task ValidarAlvoPedidoCriacaoAsync(CarrinhoItemCreateDTO dto)
    {
        var agora = DateTime.UtcNow;

        switch (dto.TipoAcesso)
        {
            case TipoAcesso.BilheteSessao:
                var sessao = await _sessaoRepository.ObterPorIdAsync(dto.SessaoId!.Value);

                if (sessao == null)
                    throw new KeyNotFoundException("Sessao nao encontrada.");

                if (sessao.Fim <= agora)
                    throw new InvalidOperationException("A sessao ja terminou.");
                break;

            case TipoAcesso.PasseDiario:
                var festivalDiario = await _festivalRepository.ObterPorIdAsync(dto.FestivalId!.Value);

                if (festivalDiario == null)
                    throw new KeyNotFoundException("Festival nao encontrado.");

                if (
                    dto.DataPasse!.Value.Date < festivalDiario.StartDate.Date
                    || dto.DataPasse.Value.Date > festivalDiario.EndDate.Date
                )
                    throw new InvalidOperationException(
                        "A data do passe diario tem de estar dentro do periodo do festival."
                    );

                if (dto.DataPasse.Value.Date < agora.Date)
                    throw new InvalidOperationException(
                        "A data do passe diario nao pode estar no passado."
                    );
                break;

            case TipoAcesso.PasseCompleto:
                var festivalCompleto = await _festivalRepository.ObterPorIdAsync(
                    dto.FestivalId!.Value
                );

                if (festivalCompleto == null)
                    throw new KeyNotFoundException("Festival nao encontrado.");

                if (festivalCompleto.EndDate < agora)
                    throw new InvalidOperationException("O festival ja terminou.");
                break;

            case TipoAcesso.AluguerDigital:
                var filme = await _filmeRepository.ObterPorIdAsync(dto.FilmeId!.Value);

                if (filme == null)
                    throw new KeyNotFoundException("Filme nao encontrado.");
                break;
        }
    }

    private static void ValidarAcessoParaCarrinho(Acesso acesso, int quantidade)
    {
        if (!acesso.IsAtivo)
            throw new InvalidOperationException("Este acesso nao esta disponivel para compra.");

        if (quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");

        if (quantidade > QuantidadeMaxima)
            throw new ArgumentException($"A quantidade nao pode exceder {QuantidadeMaxima}.");

        if (acesso.Tipo != TipoAcesso.BilheteSessao && quantidade != 1)
            throw new InvalidOperationException(
                "Apenas bilhetes de sessao permitem quantidade superior a 1."
            );

        var agora = DateTime.UtcNow;

        switch (acesso.Tipo)
        {
            case TipoAcesso.BilheteSessao:
                if (acesso.SessaoId == null || acesso.Sessao == null)
                    throw new InvalidOperationException("Bilhete de sessao sem sessao associada.");

                if (acesso.Sessao.Fim <= agora)
                    throw new InvalidOperationException("A sessao ja terminou.");
                break;

            case TipoAcesso.PasseDiario:
                if (
                    acesso.FestivalId == null
                    || acesso.Festival == null
                    || acesso.DataAcesso == null
                )
                    throw new InvalidOperationException(
                        "Passe diario sem festival ou data de acesso associada."
                    );

                if (
                    acesso.DataAcesso.Value.Date < acesso.Festival.StartDate.Date
                    || acesso.DataAcesso.Value.Date > acesso.Festival.EndDate.Date
                )
                    throw new InvalidOperationException(
                        "A data do passe diario tem de estar dentro do periodo do festival."
                    );

                if (acesso.DataAcesso.Value.Date < agora.Date)
                    throw new InvalidOperationException(
                        "A data do passe diario nao pode estar no passado."
                    );
                break;

            case TipoAcesso.PasseCompleto:
                if (acesso.FestivalId == null || acesso.Festival == null)
                    throw new InvalidOperationException("Passe completo sem festival associado.");

                if (acesso.Festival.EndDate < agora)
                    throw new InvalidOperationException("O festival ja terminou.");
                break;

            case TipoAcesso.AluguerDigital:
                if (acesso.FilmeId == null || acesso.Filme == null)
                    throw new InvalidOperationException("Aluguer digital sem filme associado.");

                if (acesso.DuracaoHoras.GetValueOrDefault(48) <= 0)
                    throw new InvalidOperationException("Aluguer digital com duracao invalida.");
                break;

            default:
                throw new InvalidOperationException("Tipo de acesso nao suportado pelo carrinho.");
        }
    }
}
