using Llm.Api.Dto;

namespace LMStudio.Api.Dto.ChatCompletion;

public partial record ChoiceDto
{
    public static MessageApiDto ToApi(ChoiceDto input)
    {
        var result = MessageDto.ToApi(input.Message);

        return result;
    }
}