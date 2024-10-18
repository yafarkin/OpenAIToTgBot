namespace Llm.Api.Dto;

public record ResponseApiDto(IReadOnlyCollection<MessageApiDto> Messages);