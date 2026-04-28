namespace OnlineCinemaFestival.Api.DTOs
{
    public class ComentarioReadDto
    {
        public int Id {get; set;}
        public int FilmeId { get; set; }
        // Até fazerem a autenticação, tenho de usar o id do usuario
        // Quando tiver pronto, mudar para o token de autenticação
        public int? UsuarioId { get; set; }
        public string Texto { get; set; }
        public DateTime Data { get; set; }
    }
}