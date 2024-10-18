using Llm.Api.Dto;

namespace LMStudio.Api.Dto.ChatCompletion;

public partial record ChatCompletionResponseDto
{
    public static ResponseApiDto ToApi(ChatCompletionResponseDto input)
    {
        var result = new ResponseApiDto(Messages: input.Choices.Select(ChoiceDto.ToApi).ToList());

        return result;
    }
}