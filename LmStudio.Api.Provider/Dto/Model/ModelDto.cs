namespace LMStudio.Api.Dto.Model;

public partial record ModelDto
{
    public ICollection<ModelDataDto> Data { get; init; } = null!;
    public string? Object { get; init; }
}