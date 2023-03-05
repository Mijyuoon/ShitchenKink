namespace ShitchenKink.Core.Data;

public class HttpClientConfig
{
    public const string Path = "HttpClient";

    public TimeSpan Timeout { get; init; }

    public TimeSpan RetryWait { get; init; }

    public int RetryCount { get; init; }
}