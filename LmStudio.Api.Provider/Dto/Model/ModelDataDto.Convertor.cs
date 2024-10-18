namespace LMStudio.Api.Dto.Model;

public partial record ModelDataDto
{
    public static Llm.Api.Dto.ModelApiDto ToApi(ModelDataDto input)
    {
        var result = new Llm.Api.Dto.ModelApiDto(input.Id);

        return result;
    }
}