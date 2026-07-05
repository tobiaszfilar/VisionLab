using VisionLab.Core.Images;

namespace VisionLab.Application.Images;

public interface IImageStorage
{
    Task<ImageAssetId> SaveAsync(
        Stream imageStream,
        string originalFileName,
        string contentType,
        long sizeInBytes,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyCollection<ImageAsset>> GetAllAsync(CancellationToken cancellationToken = default);
}