namespace VisionLab.Core.Images;

public sealed class ImageAsset
{
    private ImageAsset(
        ImageAssetId id,
        string originalFileName,
        string storedFileName,
        string contentType,
        long sizeInBytes,
        DateTimeOffset createdAt)
    {
        Id = id;
        OriginalFileName = originalFileName;
        StoredFileName = storedFileName;
        ContentType = contentType;
        SizeInBytes = sizeInBytes;
        CreatedAt = createdAt;
    }

    public ImageAssetId Id { get; }
    public string OriginalFileName { get; }
    public string StoredFileName { get; }
    public string ContentType { get; }
    public long SizeInBytes { get; }
    public DateTimeOffset CreatedAt { get; }

    public static ImageAsset Create(
        string originalFileName,
        string storedFileName,
        string contentType,
        long sizeInBytes,
        DateTimeOffset createdAt)
    {
        return CreateInternal(
            ImageAssetId.New(),
            originalFileName,
            storedFileName,
            contentType,
            sizeInBytes,
            createdAt);
    }

    public static ImageAsset Restore(
        ImageAssetId id,
        string originalFileName,
        string storedFileName,
        string contentType,
        long sizeInBytes,
        DateTimeOffset createdAt)
    {
        if (id.Value == Guid.Empty)
        {
            throw new ArgumentException("Image asset id cannot be empty.", nameof(id));
        }

        return CreateInternal(
            id,
            originalFileName,
            storedFileName,
            contentType,
            sizeInBytes,
            createdAt);
    }

    private static ImageAsset CreateInternal(
        ImageAssetId id,
        string originalFileName,
        string storedFileName,
        string contentType,
        long sizeInBytes,
        DateTimeOffset createdAt)
    {
        ArgumentException.ThrowIfNullOrEmpty(originalFileName, nameof(originalFileName));
        ArgumentException.ThrowIfNullOrEmpty(storedFileName, nameof(storedFileName));
        ArgumentException.ThrowIfNullOrEmpty(contentType, nameof(contentType));
        if (sizeInBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sizeInBytes), "Image size must be greater than zero.");
        }

        return new ImageAsset(
            id,
            originalFileName,
            storedFileName,
            contentType,
            sizeInBytes,
            createdAt);
    }
}