using Llm.Api.Dto;

namespace LMStudio.Api.Dto.ChatCompletion;

public partial record MessageDto
{
    public static MessageApiDto ToApi(MessageDto input)
    {
        var result = new MessageApiDto(Role: input.Role, Content: input.Content);

        return result;
    }

    public static MessageDto FromApi(MessageApiDto input)
    {
        var result = new MessageDto
        {
            Role = input.Role,
            Content = input.Content
        };

        return result;
    }
}