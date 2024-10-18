using Llm.Api.Dto;
using OllamaSharp.Models;

namespace Ollama.Api.Convertors;

public static class ModelConvertor
{
    public static ModelApiDto ToApi(Model input)
    {
        var result = new ModelApiDto(Name: input.Name);

        return result;
    }
}