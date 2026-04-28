namespace OnlineCinemaFestival.Api.Models
{
    public class Comentario
    {
        public int Id {get; set;}
        public int FilmeId { get; set; }
        public Filme Filme {get; set;} // para obter os outros dados do filme
        // Até fazerem a autenticação, tenho de usar o id do usuario
        // Quando tiver pronto, mudar para o token de autenticação
        public int? UsuarioId { get; set; }
        public string Texto { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public bool Reportado { get; set; } = false; // para reportar comentários inapropriados
        public bool Visivel { get; set; } = true; // para ocultar comentários reportados

    }
}