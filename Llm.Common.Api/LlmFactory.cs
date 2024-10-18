using System.Reflection;
using Llm.Api.Dto;
using Llm.Api.Interfaces;

namespace Llm.Api;

public static class LlmFactory
{
    private static void LoadProviderAssemblies()
    {
        var dir = Environment.CurrentDirectory;
        Console.WriteLine($"Looking at {dir}");

        var assemblyFiles = Directory.GetFiles(dir, "*.dll", SearchOption.TopDirectoryOnly)
            .Where(file => file.ToLower().Contains(".api.provider"))
            .ToArray();

        foreach (string assemblyFile in assemblyFiles)
        {
            try
            {
                var assembly = Assembly.LoadFrom(assemblyFile);
                Console.WriteLine($"Assembly loaded: {assembly.FullName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load assembly: {assemblyFile}. Error: {ex.Message}");
            }
        }
    }

    public static ILlmApi Create(string provider, LlmConfig? config)
    {
        LoadProviderAssemblies();

        var interfaceType = typeof(ILlmApi);

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (interfaceType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    var impl = Activator.CreateInstance(type) as ILlmApi;
                    if (impl?.Provider == provider)
                    {
                        impl.Initialize(config);
                        return impl;
                    }
                }
            }
        }

        throw new NotSupportedException($"Unknown provider: {provider}");
    }
}