using Microsoft.AspNetCore.Http;

namespace OnlineCinemaFestival.Api.Application.DTOs;

public sealed class UploadFotoPerfilRequest
{
    public IFormFile Foto { get; set; } = default!;
}