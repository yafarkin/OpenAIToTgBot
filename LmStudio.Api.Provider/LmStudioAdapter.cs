using Llm.Api.Dto;
using Llm.Api.Interfaces;
using LMStudio.Api.Dto.ChatCompletion;
using LMStudio.Api.Dto.Model;
using LMStudio.Api.Interfaces;

namespace LMStudio.Api;

public class LmStudioAdapter : ILlmApi
{
    public string Provider => "lmstudio";

    private ILmStudioApi _api = null!;
    private bool _initialized;

    public void Initialize(LlmConfig? config)
    {
        _api = ClientFactory.CreateClient(config?.BaseUrl);
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

        var models = await _api.GetModelsAsync(cancellationToken);
        var result = ModelDto.ToApi(models);

        return result;
    }

    public async Task<ResponseApiDto> PromptAsync(RequestApiDto requestApiDto, CancellationToken cancellationToken)
    {
        CheckInitialized();

        var apiRequest = ChatCompletionRequestDto.FromApi(requestApiDto);
        var apiResponse = await _api.ChatCompletionAsync(apiRequest, cancellationToken);

        var response = ChatCompletionResponseDto.ToApi(apiResponse);
        return response;
    }
}