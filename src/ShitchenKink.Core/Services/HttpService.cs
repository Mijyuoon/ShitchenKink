namespace ShitchenKink.Core.Services;

public class HttpService
{
    private readonly HttpClient _client;

    public HttpService(HttpClient client)
    {
        _client = client;
    }

    public async Task<Stream> DownloadFileAsync(Uri url)
    {
        EnsureHttpUri(url);

        var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStreamAsync();
    }

    public Task<Stream> DownloadFileAsync(string url) =>
        DownloadFileAsync(new Uri(url, UriKind.Absolute));

    private static void EnsureHttpUri(Uri uri)
    {
        if (uri.Scheme != "http" && uri.Scheme != "https")
        {
            throw new UriFormatException("only http and https URIs are supported");
        }
    }
}