using Microsoft.AspNetCore.Http;

namespace OnlineCinemaFestival.Api.Application.DTOs;

public sealed class UploadImagemComunidadeRequest
{
    public IFormFile Imagem { get; set; } = default!;
}