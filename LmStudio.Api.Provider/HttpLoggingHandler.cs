using System.Net.Http.Headers;

namespace LMStudio.Api;

public class HttpLoggingHandler : DelegatingHandler
{
    private readonly string? _token;

    public HttpLoggingHandler(HttpMessageHandler? innerHandler = null, string? token = null)
        : base(innerHandler ?? new HttpClientHandler())
    {
        _token = token;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var req = request;
        var id = Guid.NewGuid().ToString();
        var msg = $"[{id} -   Request]";

        Console.WriteLine($"{msg}========Start==========");
        Console.WriteLine($"{msg} {req.Method} {req.RequestUri?.PathAndQuery} {req.RequestUri?.Scheme}/{req.Version}");
        Console.WriteLine($"{msg} Host: {req.RequestUri?.Scheme}://{req.RequestUri?.Host}");

        if (!string.IsNullOrWhiteSpace(_token))
        {
            req.Headers.Add("Authorization", _token);
        }

        foreach (var header in req.Headers)
        {
            Console.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
        }

        if (req.Content != null)
        {
            foreach (var header in req.Content.Headers)
            {
                Console.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (req.Content is StringContent || IsTextBasedContentType(req.Headers) || IsTextBasedContentType(req.Content.Headers))
            {
                var result = await req.Content.ReadAsStringAsync(cancellationToken);

                Console.WriteLine($"{msg} Content:");
                Console.WriteLine($"{msg} {result}");
            }
        }

        var start = DateTime.Now;

        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        var end = DateTime.Now;

        Console.WriteLine($"{msg} Duration: {end - start}");
        Console.WriteLine($"{msg}==========End==========");

        msg = $"[{id} - Response]";
        Console.WriteLine($"{msg}=========Start=========");

        var resp = response;

        Console.WriteLine($"{msg} {req.RequestUri?.Scheme.ToUpper()}/{resp.Version} {(int)resp.StatusCode} {resp.ReasonPhrase}");

        foreach (var header in resp.Headers)
        {
            Console.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
        }

        foreach (var header in resp.Content.Headers)
        {
            Console.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
        }

        if (resp.Content is StringContent || IsTextBasedContentType(resp.Headers) || IsTextBasedContentType(resp.Content.Headers))
        {
            start = DateTime.Now;
            var result = await resp.Content.ReadAsStringAsync(cancellationToken);
            end = DateTime.Now;

            Console.WriteLine($"{msg} Content:");
            Console.WriteLine($"{msg} {result}");
            Console.WriteLine($"{msg} Duration: {end - start}");
        }

        Console.WriteLine($"{msg}==========End==========");
        return response;
    }

    readonly string[] types = new[] { "html", "text", "xml", "json", "txt", "x-www-form-urlencoded" };

    private bool IsTextBasedContentType(HttpHeaders headers)
    {
        if (!headers.TryGetValues("Content-Type", out var values))
        {
            return false;
        }

        var header = string.Join(" ", values).ToLowerInvariant();

        return types.Any(t => header.Contains(t));
    }
}