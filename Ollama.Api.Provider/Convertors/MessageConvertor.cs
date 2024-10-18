using Llm.Api.Dto;
using OllamaSharp.Models.Chat;

namespace Ollama.Api.Convertors;

public static class MessageConvertor
{
    private static ChatRole ToApi(string input)
    {
        switch (input.ToLower())
        {
            case "user":
                return ChatRole.User;
            case "system":
                return ChatRole.System;
            case "assistant":
                return ChatRole.Assistant;
            default:
                throw new NotSupportedException($"Unknown role: {input}");
        }
    }

    private static string FromApi(ChatRole input)
    {
        var result = input.ToString();

        return result;
    }

    public static Message ToApi(MessageApiDto input)
    {
        var result = new Message(role: ToApi(input.Role), content: input.Content);

        return result;
    }

    public static MessageApiDto FromApi(Message input)
    {
        var result = new MessageApiDto(Role: FromApi(input.Role ?? ChatRole.User),
            Content: input.Content ?? string.Empty);

        return result;
    }
}