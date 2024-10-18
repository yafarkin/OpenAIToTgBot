using Newtonsoft.Json;

namespace LMStudio.Api.Dto.Model;

public partial record ModelDataDto
{
    public string Id { get; init; } = null!;
    public string? Object { get; init; }

    [JsonProperty("owned_by")]
    public string? OwnedBy { get; init; }
}