using VisionLab.Core.Images;

namespace VisionLab.Application.Images;

public sealed class ImageAssetService: IImageAssetService
{
    private readonly IImageStorage _imageStorage;

    public ImageAssetService(IImageStorage imageStorage)
    {
        _imageStorage = imageStorage;
    }

    public Task<ImageAsset> UploadAsync(Stream imageStream, string originalFileName, string contentType, long sizeInBytes, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(imageStream);
        ArgumentException.ThrowIfNullOrWhiteSpace(originalFileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);

        if (sizeInBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sizeInBytes));
        }

        return _imageStorage.SaveAsync(
            imageStream,
            originalFileName,
            contentType,
            sizeInBytes,
            cancellationToken
         );
    }

    public Task<IReadOnlyCollection<ImageAsset>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _imageStorage.GetAllAsync(cancellationToken);
    }

    public Task<ImageAsset?> GetByIdAsync(
        ImageAssetId id, 
        CancellationToken cancellationToken = default)
    {
        return _imageStorage.GetByIdAsync(id, cancellationToken);
    }
}