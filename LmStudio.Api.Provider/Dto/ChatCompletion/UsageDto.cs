namespace LMStudio.Api.Dto.ChatCompletion;

public record UsageDto
{
    public int PromptTokens { get; init; }
    public int CompletionTokens { get; init; }
    public int TotalTokens { get; init; }
}