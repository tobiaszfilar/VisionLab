using System.Net.Http.Headers;
using System.Net.Http.Json;
using VisionLab.Shared.Images;

namespace VisionLab.Client;

internal sealed class VisionLabApiClient : IVisionLabApiClient
{
    private readonly HttpClient _httpClient;

    public VisionLabApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<ImageAssetDto>> GetImagesAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            "api/images",
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        var images = await response.Content.ReadFromJsonAsync<ImageAssetDto[]>(
            cancellationToken);

        return images ?? [];
    }

    public async Task<ImageAssetDto?> GetImageByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"api/images/{id}",
            cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<ImageAssetDto>(
            cancellationToken);
    }

    public async Task<ImageAssetDto> UploadImageAsync(Stream imageStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(imageStream);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);

        using var form = new MultipartFormDataContent();
        using var fileContent = new StreamContent(imageStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        form.Add(
            fileContent,
            "file",
            fileName);

        using var response = await _httpClient.PostAsync(
            "api/images",
            form,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        var uploadedImage = await response.Content.ReadFromJsonAsync<ImageAssetDto>(
            cancellationToken);

        return uploadedImage
            ?? throw new InvalidOperationException("API returned empty response for uploaded image.");
    }

    public async Task<Stream?> GetImageContentAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"api/images/{id}/content",
            cancellationToken);
    
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    
        await EnsureSuccessAsync(response, cancellationToken);
    
        await using var responseStream = await response.Content.ReadAsStreamAsync(
            cancellationToken);
    
        var memoryStream = new MemoryStream();
    
        await responseStream.CopyToAsync(
            memoryStream,
            cancellationToken);
    
        memoryStream.Position = 0;
    
        return memoryStream;
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var responseBody = response.Content is null
            ? null
            : await response.Content.ReadAsStringAsync(cancellationToken);

        throw new VisionLabApiException(
            response.StatusCode,
            responseBody);
    }
}