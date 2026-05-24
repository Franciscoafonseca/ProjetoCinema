using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Tests.Support.Builders;

public sealed class PagamentoBuilder
{
    private int _id = 1;
    private string _referencia = "PG-CMP-TESTE-0001";
    private string? _entidade;
    private decimal _valor;
    private string _metodo = MetodosPagamento.CartaoCredito;
    private EstadoPagamento _estado = EstadoPagamento.Aprovado;
    private DateTime _criadoEm = new(2026, 5, 23, 12, 0, 0, DateTimeKind.Utc);
    private DateTime? _processadoEm;
    private string? _mensagem;

    public PagamentoBuilder ComId(int id) { _id = id; return this; }
    public PagamentoBuilder ComReferencia(string referencia) { _referencia = referencia; return this; }
    public PagamentoBuilder ComEntidade(string entidade) { _entidade = entidade; return this; }
    public PagamentoBuilder ComValor(decimal valor) { _valor = valor; return this; }
    public PagamentoBuilder ComMetodo(string metodo) { _metodo = metodo; return this; }
    public PagamentoBuilder NoEstado(EstadoPagamento estado) { _estado = estado; return this; }
    public PagamentoBuilder CriadoEm(DateTime data) { _criadoEm = data; return this; }
    public PagamentoBuilder ProcessadoEm(DateTime data) { _processadoEm = data; return this; }
    public PagamentoBuilder ComMensagem(string mensagem) { _mensagem = mensagem; return this; }

    public PagamentoBuilder ComoMultibancoPendente(string entidade = "12345", string referencia = "123456789")
    {
        _metodo = MetodosPagamento.ReferenciaMultibanco;
        _estado = EstadoPagamento.Pendente;
        _entidade = entidade;
        _referencia = referencia;
        return this;
    }

    public PagamentoBuilder ComoCartaoAprovado()
    {
        _metodo = MetodosPagamento.CartaoCredito;
        _estado = EstadoPagamento.Aprovado;
        _processadoEm = _criadoEm;
        return this;
    }

    public Pagamento Build() => new()
    {
        Id = _id,
        Referencia = _referencia,
        Entidade = _entidade,
        Valor = _valor,
        Metodo = _metodo,
        Estado = _estado,
        CriadoEm = _criadoEm,
        ProcessadoEm = _processadoEm,
        Mensagem = _mensagem,
    };

    public static implicit operator Pagamento(PagamentoBuilder builder) => builder.Build();
}
