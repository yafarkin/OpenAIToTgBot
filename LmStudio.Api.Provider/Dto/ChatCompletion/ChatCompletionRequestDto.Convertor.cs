using Llm.Api.Dto;

namespace LMStudio.Api.Dto.ChatCompletion;

public partial record ChatCompletionRequestDto
{
    public static ChatCompletionRequestDto FromApi(RequestApiDto input)
    {
        var result = new ChatCompletionRequestDto
        {
            Model = input.Model,
            Messages = input.Messages.Select(MessageDto.FromApi)
        };

        return result;
    }
}