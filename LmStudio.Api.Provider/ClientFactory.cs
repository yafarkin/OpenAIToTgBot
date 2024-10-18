using LMStudio.Api.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;

namespace LMStudio.Api;

public static class ClientFactory
{
    public static ILmStudioApi CreateClient(string? baseUrl)
    {
        var settings = new RefitSettings
        {
            ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            })
        };

        var client = RestService.For<ILmStudioApi>(new HttpClient
        {
            BaseAddress = new Uri(string.IsNullOrWhiteSpace(baseUrl) ? "http://localhost:1234/" : baseUrl),
            Timeout = TimeSpan.FromSeconds(3600)
        }, settings);

        //var httpClient = new HttpClient(new HttpLoggingHandler()) { BaseAddress = new Uri(baseUrl) };
        //var client = RestService.For<ILMStudioApi>(httpClient);

        return client;
    }
}