using VisionLab.Shared.Images;

namespace VisionLab.Client;

public interface IVisionLabApiClient
{
    Task<IReadOnlyCollection<ImageAssetDto>> GetImagesAsync(
        CancellationToken cancellationToken = default);

    Task<ImageAssetDto?> GetImageByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    Task<ImageAssetDto> UploadImageAsync(
        Stream imageStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default
    );

    Task<Stream?> GetImageContentAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}