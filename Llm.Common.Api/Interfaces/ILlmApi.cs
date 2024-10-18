using Llm.Api.Dto;

namespace Llm.Api.Interfaces;

public interface ILlmApi
{
    string Provider { get; }

    void Initialize(LlmConfig? config);

    Task<IReadOnlyCollection<ModelApiDto>> GetModelsAsync(CancellationToken cancellationToken);
    Task<ResponseApiDto> PromptAsync(RequestApiDto requestApiDto, CancellationToken cancellationToken);
}