using System.Net;

namespace CoEditor.Client.Rest;

public class ServiceCallFailedException : Exception
{
    public ServiceCallFailedException(HttpMethod method, string url, HttpStatusCode statusCode) : base(
        $"{method} {url} failed: {statusCode}")
    {
    }

    public ServiceCallFailedException(HttpMethod method, string url) : base($"{method} {url} failed: No response")
    {
    }
}
