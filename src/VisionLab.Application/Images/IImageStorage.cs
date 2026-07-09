using VisionLab.Core.Images;

namespace VisionLab.Application.Images;

public interface IImageStorage
{
    Task<ImageAsset> SaveAsync(
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

    Task<Stream?> OpenReadAsync(
        ImageAssetId id,
        CancellationToken cancellationToken = default);
}