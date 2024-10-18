namespace Llm.Api.Dto;

public record RequestApiDto(string Model, IReadOnlyCollection<MessageApiDto> Messages);