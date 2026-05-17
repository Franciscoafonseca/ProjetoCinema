using System.Text.Json.Serialization;

namespace OnlineCinemaFestival.Api.DTOs;

public class TmdbVideosResponseDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("results")]
    public List<TmdbVideoDTO> Results { get; set; } = new();
}

public class TmdbVideoDTO
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("site")]
    public string Site { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("official")]
    public bool Official { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
