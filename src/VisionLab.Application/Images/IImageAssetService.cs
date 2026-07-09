using VisionLab.Core.Images;

namespace VisionLab.Application.Images;

public interface IImageAssetService
{
    Task<ImageAsset> UploadAsync(
        Stream imageStream,
        string originalFileName,
        string contentType,
        long sizeInBytes,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyCollection<ImageAsset>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ImageAsset?> GetByIdAsync(
        ImageAssetId id,
        CancellationToken cancellationToken = default);

    Task<ImageAssetContent?> GetContentAsync(
        ImageAssetId id,
        CancellationToken cancellationToken = default);
}