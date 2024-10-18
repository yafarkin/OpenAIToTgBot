namespace LMStudio.Api.Dto.ChatCompletion;

public partial record ChoiceDto
{
    public int Index { get; init; }

    public MessageDto Message { get; init; } = null!;
    //public object Logprobs { get; set; }
    public string? FinishReason { get; init; }
}