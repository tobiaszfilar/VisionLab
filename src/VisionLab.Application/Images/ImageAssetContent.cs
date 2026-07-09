namespace VisionLab.Application.Images;

public sealed class ImageAssetContent
{
    public ImageAssetContent(
        string fileName,
        string contentType,
        Stream stream)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);
        ArgumentNullException.ThrowIfNull(stream);

        FileName = fileName;
        ContentType = contentType;
        Stream = stream;
    }

    public string FileName { get; }

    public string ContentType { get; }

    public Stream Stream { get; }
}