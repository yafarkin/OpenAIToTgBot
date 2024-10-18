using Newtonsoft.Json;

namespace LMStudio.Api.Dto.ChatCompletion;

public partial record ChatCompletionResponseDto
{
    public string Id { get; init; } = null!;
    public string Object { get; init; } = null!;
    public long Created { get; init; }
    public string Model { get; init; } = null!;

    [JsonProperty("system_fingerprint")]
    public string SystemFingerPrint { get; init; } = null!;

    public ICollection<ChoiceDto> Choices { get; init; } = null!;
    public UsageDto? Usage { get; init; }
}