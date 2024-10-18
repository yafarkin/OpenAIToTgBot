using Llm.Api.Dto;

namespace OpenAIToTgBot.Settings;

public record LlmSettings
{
    public string Provider { get; init; } = null!;
    public string? Model { get; init; }
    public bool SaveHistory { get; init; }

    public LlmConfig? Config { get; init; }
}