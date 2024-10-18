using Llm.Api.Dto;
using Llm.Api.Interfaces;
using Ollama.Api.Convertors;
using OllamaSharp;
using OllamaSharp.Models.Chat;

namespace Ollama.Api;

public class OllamaAdapter : ILlmApi
{
    public string Provider => "ollama";

    private OllamaApiClient _api = null!;
    private bool _initialized;

    public void Initialize(LlmConfig? config)
    {
        _api = new OllamaApiClient(new Uri(string.IsNullOrWhiteSpace(config?.BaseUrl) ? "http://localhost:11434/" : config.BaseUrl));
        _initialized = true;
    }

    private void CheckInitialized()
    {
        if (_initialized)
        {
            return;
        }

        throw new InvalidOperationException("Provider not initialized");
    }

    public async Task<IReadOnlyCollection<ModelApiDto>> GetModelsAsync(CancellationToken cancellationToken)
    {
        CheckInitialized();

        var models = await _api.ListLocalModels(cancellationToken);
        var result = models.Select(ModelConvertor.ToApi).ToList();

        return result;
    }

    public async Task<ResponseApiDto> PromptAsync(RequestApiDto requestApiDto, CancellationToken cancellationToken)
    {
        CheckInitialized();

        var request = ChatConvertor.ToApi(requestApiDto);
        var response = await _api.SendChat(request, streamer => { }, cancellationToken);
        response = response.Skip(requestApiDto.Messages.Count);

        var result = ChatConvertor.FromApi(response);

        return result;
    }
}