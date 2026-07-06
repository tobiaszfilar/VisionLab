using System.Net;

namespace VisionLab.Client;

public sealed class VisionLabApiException : Exception
{
    public VisionLabApiException(
        HttpStatusCode statusCode,
        string? responseBody)
        : base($"VisionLab API request failed with status code {(int)statusCode} ({statusCode}).")
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }

    public HttpStatusCode StatusCode { get; }

    public string? ResponseBody { get; }
}