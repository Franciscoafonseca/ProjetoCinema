/* * ATENÇÃO QUE EU SÓ COPIEI O MOPDELO DE FILME DO REPOSITORIO ANTIGO
    PORQUE EU PRECISAVA QUE EXISTISSEUM ID DOS FILMES, DEPOIS QUEM FIZER ESTA
    PARTE QUE FAÇA ISTO DIREITO
 */


namespace OnlineCinemaFestival.Api.Models;

public class Filme
{
    public int Id { get; set; }

    public string Titulo { get; set; } = "";
    public string Sinopse { get; set; } = "";
    public string Genero { get; set; } = "";
    public string Realizador { get; set; } = "";

    public string PosterUrl { get; set; } = "";
    public string TrailerUrl { get; set; } = "";

    public double Popularidade { get; set; }
    public double ClassificacaoMedia { get; set; }
}