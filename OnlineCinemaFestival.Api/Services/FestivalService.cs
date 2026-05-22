using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

/// <summary>
/// Serviço responsável pela gestão dos festivais.
/// Contém operações de consulta, criação, atualização e remoção de festivais.
/// </summary>
public class FestivalService : IFestivalService
{
    private readonly IFestivalRepository _repository;
    private readonly IAcessoRepository _acessoRepository;

    /// <summary>
    /// Inicializa uma nova instância do serviço de festivais.
    /// </summary>
    /// <param name="repository">
    /// Repositório responsável pelo acesso aos dados dos festivais.
    /// </param>
    public FestivalService(IFestivalRepository repository, IAcessoRepository acessoRepository)
    {
        _repository = repository;
        _acessoRepository = acessoRepository;
    }

    /// <summary>
    /// Obtém todos os festivais existentes no sistema.
    /// </summary>
    /// <returns>Lista de festivais convertidos para DTOs de leitura.</returns>
    public async Task<IEnumerable<FestivalReadDTO>> ObterTodosAsync()
    {
        // Obtém todos os festivais guardados na base de dados.
        var festivals = await _repository.ObterTodosAsync();

        // Converte as entidades Festival para DTOs antes de devolver a resposta.
        return festivals.Select(FestivalMapper.MapToReadDTO);
    }

    /// <summary>
    /// Obtém um festival específico através do seu identificador.
    /// </summary>
    /// <param name="id">Identificador do festival.</param>
    /// <returns>
    /// O festival correspondente ao identificador indicado, ou null caso não exista.
    /// </returns>
    public async Task<FestivalReadDTO?> ObterPorIdAsync(int id)
    {
        // Procura o festival pelo seu identificador.
        var festival = await _repository.ObterDetalhePorIdAsync(id);

        if (festival == null)
            return null;

        // Converte a entidade encontrada para DTO de leitura.
        return FestivalMapper.MapToReadDTO(festival);
    }

    /// <summary>
    /// Cria um novo festival no sistema.
    /// </summary>
    /// <param name="dto">Dados necessários para a criação do festival.</param>
    /// <returns>Festival criado, convertido para DTO de leitura.</returns>
    /// <exception cref="ArgumentException">
    /// Lançada quando os dados do festival são inválidos.
    /// </exception>
    public async Task<FestivalReadDTO> CriarAsync(FestivalCreateDTO dto)
    {
        // Valida os dados principais antes de criar o festival.
        ValidateFestivalData(dto.Name, dto.StartDate, dto.EndDate);

        // Converte o DTO de criação para uma entidade Festival.
        var festival = FestivalMapper.MapFromCreateDTO(dto);

        // Guarda o novo festival na base de dados.
        await _repository.AddAsync(festival);
        await _repository.SaveChangesAsync();
        await GarantirAcessosFestivalAsync(festival);

        // Devolve o festival criado em formato DTO.
        return FestivalMapper.MapToReadDTO(festival);
    }

    /// <summary>
    /// Atualiza os dados de um festival existente.
    /// </summary>
    /// <param name="id">Identificador do festival a atualizar.</param>
    /// <param name="dto">Novos dados do festival.</param>
    /// <exception cref="ArgumentException">
    /// Lançada quando os dados fornecidos são inválidos.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Lançada quando o festival não é encontrado.
    /// </exception>
    public async Task AtualizarAsync(int id, FestivalUpdateDTO dto)
    {
        // Valida os dados recebidos antes da atualização.
        ValidateFestivalData(dto.Name, dto.StartDate, dto.EndDate);

        // Procura o festival existente.
        var festival = await _repository.ObterPorIdAsync(id);

        if (festival == null)
            throw new KeyNotFoundException("Festival não encontrado.");

        // Atualiza a entidade existente com os dados recebidos no DTO.
        FestivalMapper.MapToExistingFestival(dto, festival);

        // Guarda as alterações na base de dados.
        await _repository.SaveChangesAsync();
    }

    /// <summary>
    /// Remove um festival existente do sistema.
    /// </summary>
    /// <param name="id">Identificador do festival a remover.</param>
    /// <exception cref="KeyNotFoundException">
    /// Lançada quando o festival não é encontrado.
    /// </exception>
    public async Task EliminarAsync(int id)
    {
        // Procura o festival pelo seu identificador.
        var festival = await _repository.ObterPorIdAsync(id);

        if (festival == null)
            throw new KeyNotFoundException("Festival não encontrado.");

        // Remove o festival do repositório.
        _repository.Remove(festival);

        // Confirma a remoção na base de dados.
        await _repository.SaveChangesAsync();
    }

    /// <summary>
    /// Valida os dados principais de um festival.
    /// </summary>
    /// <param name="name">Nome do festival.</param>
    /// <param name="startDate">Data de início do festival.</param>
    /// <param name="endDate">Data de fim do festival.</param>
    /// <exception cref="ArgumentException">
    /// Lançada quando o nome é inválido ou quando a data de fim é anterior à data de início.
    /// </exception>
    private static void ValidateFestivalData(string name, DateTime startDate, DateTime endDate)
    {
        // Garante que o festival tem um nome válido.
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome do festival é obrigatório.");

        // Garante que o intervalo temporal do festival é válido.
        if (endDate < startDate)
            throw new ArgumentException("A data de fim não pode ser anterior à data de início.");
    }

    private async Task GarantirAcessosFestivalAsync(Festival festival)
    {
        var acessos = new List<Acesso>();

        if (
            await _acessoRepository.GetAtivoParaCarrinhoAsync(
                TipoAcesso.PasseCompleto,
                festival.Id,
                null,
                null,
                null
            ) == null
        )
        {
            acessos.Add(
                new Acesso
                {
                    Nome = $"Passe Completo - {festival.Name}",
                    Descricao = "Passe valido para todo o festival.",
                    Tipo = TipoAcesso.PasseCompleto,
                    Preco = 24.99m,
                    FestivalId = festival.Id,
                    IsAtivo = true,
                    CriadoEm = DateTime.UtcNow,
                }
            );
        }

        var totalDias = Math.Max(1, (festival.EndDate.Date - festival.StartDate.Date).Days + 1);
        for (var i = 0; i < totalDias; i++)
        {
            var dia = festival.StartDate.Date.AddDays(i);

            if (
                await _acessoRepository.GetAtivoParaCarrinhoAsync(
                    TipoAcesso.PasseDiario,
                    festival.Id,
                    null,
                    null,
                    dia
                ) != null
            )
                continue;

            acessos.Add(
                new Acesso
                {
                    Nome = $"Passe Diario - {festival.Name} - {dia:dd/MM/yyyy}",
                    Descricao = "Passe valido para todas as sessoes de um dia do festival.",
                    Tipo = TipoAcesso.PasseDiario,
                    Preco = 9.99m,
                    FestivalId = festival.Id,
                    DataAcesso = dia,
                    IsAtivo = true,
                    CriadoEm = DateTime.UtcNow,
                }
            );
        }

        if (acessos.Count == 0)
            return;

        await _acessoRepository.AddManyAsync(acessos);
        await _acessoRepository.SaveChangesAsync();
    }
}
