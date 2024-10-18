using Llm.Api.Dto;
using OllamaSharp.Models.Chat;

namespace Ollama.Api.Convertors;

public static class ChatConvertor
{
    public static ChatRequest ToApi(RequestApiDto input)
    {
        var result = new ChatRequest
        {
            Model = input.Model,
            Messages = input.Messages.Select(MessageConvertor.ToApi).ToList(),
            Stream = false
        };

        return result;
    }

    public static ResponseApiDto FromApi(IEnumerable<Message> input)
    {
        var result = new ResponseApiDto(Messages: input.Select(MessageConvertor.FromApi).ToList());

        return result;
    }
}