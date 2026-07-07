namespace VisionLab.UI.Services;

public sealed class PickedImageFile : IAsyncDisposable
{
    public PickedImageFile(
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

    public ValueTask DisposeAsync()
    {
        return Stream.DisposeAsync();
    }
}