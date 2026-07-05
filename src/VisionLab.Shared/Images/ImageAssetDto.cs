namespace VisionLab.Shared.Images;

public sealed record ImageAssetDto(
    Guid Id,
    string OriginalFileName,
    string StoredFileName,
    string ContentType,
    long SizeInBytes,
    DateTimeOffset CreatedAt);