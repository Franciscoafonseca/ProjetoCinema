using System.Text.Json.Serialization;
namespace OnlineCinemaFestival.Api.DTOs;

public class ComunidadeReadDto
{
    [JsonPropertyName("id")]
    public Guid PublicId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPublic { get; set; } 
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int MembersCount { get; set; }
    public int ComentariosCount { get; set; }
    public string? CodigoConvite {get;set;}
}