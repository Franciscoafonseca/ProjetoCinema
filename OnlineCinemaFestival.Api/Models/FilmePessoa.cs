namespace OnlineCinemaFestival.Api.Models;

public class FilmePessoa
{
    public int FilmeId { get; set; }

    public Filme Filme { get; set; } = null!;

    public int PessoaId { get; set; }

    public Pessoa Pessoa { get; set; } = null!;

    public FuncaoPessoaFilme Funcao { get; set; }

    public string? Personagem { get; set; }

    public int Ordem { get; set; }
}
