namespace LMStudio.Api.Dto.ChatCompletion;

public partial record MessageDto
{
    public string Role { get; init; } = null!;
    public string Content { get; init; } = null!;
}