using VisionLab.Core.Images;
namespace VisionLab.Infrastructure.Images;

internal sealed class ImageAssetRecord
{
    public Guid Id { get; init; }

    public string OriginalFileName { get; init; } = string.Empty;

    public string StoredFileName { get; init; } = string.Empty;

    public string ContentType { get; init; } = string.Empty;

    public long SizeInBytes { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public static ImageAssetRecord FromDomain(ImageAsset asset)
    {
        return new ImageAssetRecord
        {
            Id = asset.Id.Value,
            OriginalFileName = asset.OriginalFileName,
            StoredFileName = asset.StoredFileName,
            ContentType = asset.ContentType,
            SizeInBytes = asset.SizeInBytes,
            CreatedAt = asset.CreatedAt
        };
    }

    public ImageAsset ToDomain()
    {
        return ImageAsset.Restore(
            id: new ImageAssetId(Id),
            originalFileName: OriginalFileName,
            storedFileName: StoredFileName,
            contentType: ContentType,
            sizeInBytes: SizeInBytes,
            createdAt: CreatedAt);
    }
}