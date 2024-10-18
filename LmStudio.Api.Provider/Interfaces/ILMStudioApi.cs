using LMStudio.Api.Dto.ChatCompletion;
using LMStudio.Api.Dto.Model;
using Refit;

namespace LMStudio.Api.Interfaces;

public interface ILmStudioApi
{
    [Get("/v1/models")]
    Task<ModelDto> GetModelsAsync(CancellationToken cancellationToken);

    [Post("/v1/chat/completions")]
    Task<ChatCompletionResponseDto> ChatCompletionAsync(ChatCompletionRequestDto request, CancellationToken cancellationToken);
}