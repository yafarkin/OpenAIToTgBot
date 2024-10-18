using Newtonsoft.Json;

namespace LMStudio.Api.Dto.ChatCompletion;

public partial record ChatCompletionRequestDto
{
    public string Model { get; init; } = null!;
    public IEnumerable<MessageDto> Messages { get; init; } = null!;

    public double Temperature { get; init; } = 0.7;

    [JsonProperty("max_tokens")]
    public int MaxTokens { get; init; } = -1;
    public bool Stream { get; init; } = false;
}