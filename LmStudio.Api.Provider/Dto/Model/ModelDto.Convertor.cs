namespace LMStudio.Api.Dto.Model;

public partial record ModelDto
{
    public static IReadOnlyCollection<Llm.Api.Dto.ModelApiDto> ToApi(ModelDto input)
    {
        var result = new List<Llm.Api.Dto.ModelApiDto>(input.Data.Select(ModelDataDto.ToApi));

        return result;
    }
}