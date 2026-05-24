using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class AcessoAutomaticoService : IAcessoAutomaticoService
{
    private readonly IAcessoRepository _acessoRepository;
    private readonly IFilmeRepository _filmeRepository;
    private readonly ISessaoRepository _sessaoRepository;
    private readonly IAcessoAutomaticoFactory _factory;

    public AcessoAutomaticoService(
        IAcessoRepository acessoRepository,
        IFilmeRepository filmeRepository,
        ISessaoRepository sessaoRepository,
        IAcessoAutomaticoFactory factory
    )
    {
        _acessoRepository = acessoRepository;
        _filmeRepository = filmeRepository;
        _sessaoRepository = sessaoRepository;
        _factory = factory;
    }

    public async Task GarantirParaFilmeAsync(int filmeId)
    {
        var filme = await _filmeRepository.ObterDetalhePorIdAsync(filmeId);
        if (filme == null)
            return;

        var acessos = new List<Acesso>();
        await AdicionarAluguerSeNecessarioAsync(acessos, filme);

        foreach (var festival in await _filmeRepository.ObterFestivaisDoFilmeAsync(filmeId))
            await AdicionarPassesFestivalSeNecessarioAsync(acessos, festival);

        foreach (var sessao in await _filmeRepository.ObterSessoesDoFilmeAsync(filmeId))
            await AdicionarBilheteSessaoSeNecessarioAsync(acessos, sessao);

        await GuardarAsync(acessos);
    }

    public async Task GarantirParaFestivalAsync(Festival festival)
    {
        var acessos = new List<Acesso>();
        await AdicionarPassesFestivalSeNecessarioAsync(acessos, festival);
        await GuardarAsync(acessos);
    }

    public async Task GarantirParaSessaoAsync(int sessaoId)
    {
        var sessao = await _sessaoRepository.ObterPorIdAsync(sessaoId);
        if (sessao == null)
            return;

        var acessos = new List<Acesso>();
        await AdicionarBilheteSessaoSeNecessarioAsync(acessos, sessao);
        await GuardarAsync(acessos);
    }

    public async Task GarantirParaAssociacaoAsync(Festival festival, Filme filme)
    {
        var acessos = new List<Acesso>();
        await AdicionarAluguerSeNecessarioAsync(acessos, filme);
        await AdicionarPassesFestivalSeNecessarioAsync(acessos, festival);
        await GuardarAsync(acessos);
    }

    private async Task AdicionarAluguerSeNecessarioAsync(List<Acesso> acessos, Filme filme)
    {
        var existente = await _acessoRepository.ObterAluguerDigitalAtivoAsync(filme.Id);

        if (existente == null)
            acessos.Add(_factory.CriarAluguerDigital(filme));
    }

    private async Task AdicionarPassesFestivalSeNecessarioAsync(
        List<Acesso> acessos,
        Festival festival
    )
    {
        var passeCompleto = await _acessoRepository.ObterPasseCompletoAtivoAsync(festival.Id);

        if (passeCompleto == null)
            acessos.Add(_factory.CriarPasseCompleto(festival));

        foreach (var dia in DiasDoFestival(festival))
        {
            var passeDiario = await _acessoRepository.ObterPasseDiarioAtivoAsync(
                festival.Id,
                dia
            );

            if (passeDiario == null)
                acessos.Add(_factory.CriarPasseDiario(festival, dia));
        }
    }

    private async Task AdicionarBilheteSessaoSeNecessarioAsync(
        List<Acesso> acessos,
        Sessao sessao
    )
    {
        var existente = await _acessoRepository.ObterBilheteSessaoAtivoAsync(sessao.Id);

        if (existente == null)
            acessos.Add(_factory.CriarBilheteSessao(sessao));
    }

    private async Task GuardarAsync(List<Acesso> acessos)
    {
        if (acessos.Count == 0)
            return;

        await _acessoRepository.AddManyAsync(acessos);
        await _acessoRepository.SaveChangesAsync();
    }

    private static IEnumerable<DateTime> DiasDoFestival(Festival festival)
    {
        var totalDias = Math.Max(1, (festival.EndDate.Date - festival.StartDate.Date).Days + 1);

        for (var i = 0; i < totalDias; i++)
            yield return festival.StartDate.Date.AddDays(i);
    }
}
