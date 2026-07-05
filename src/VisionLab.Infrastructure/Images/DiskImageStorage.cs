using System.Text.Json;
using Microsoft.Extensions.Options;
using VisionLab.Application.Images;
using VisionLab.Core.Images;

namespace VisionLab.Infrastructure.Images;

public sealed class DiskImageStorage : IImageStorage
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly ImageStorageOptions _options;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public DiskImageStorage(IOptions<ImageStorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<ImageAsset> SaveAsync(
        Stream imageStream,
        string originalFileName,
        string contentType,
        long sizeInBytes,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(imageStream);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(originalFileName);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(contentType);

        EnsureStorageDirectoryExists();

        var safeOriginalFileName = Path.GetFileName(originalFileName);
        var extension = Path.GetExtension(safeOriginalFileName);
        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var storedPath = Path.Combine(_options.RootPath, storedFileName);

        await using (var fileStream = File.Create(storedPath))
        {
            await imageStream.CopyToAsync(fileStream, cancellationToken);
        }

        var asset = ImageAsset.Create(
            safeOriginalFileName,
            storedFileName,
            contentType,
            sizeInBytes,
            DateTimeOffset.UtcNow);

        await AddToManifestAsync(asset, cancellationToken);

        return asset;
    }

    public async Task<IReadOnlyCollection<ImageAsset>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        EnsureStorageDirectoryExists();

        await _lock.WaitAsync(cancellationToken);

        try
        {
            var records = await ReadManifestUnsafeAsync(cancellationToken);

            return records
                .Select(record => record.ToDomain())
                .ToArray();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<ImageAsset?> GetByIdAsync(
        ImageAssetId id,
        CancellationToken cancellationToken = default)
    {
        EnsureStorageDirectoryExists();

        await _lock.WaitAsync(cancellationToken);

        try
        {
            var records = await ReadManifestUnsafeAsync(cancellationToken);

            var record = records.FirstOrDefault(x => x.Id == id.Value);

            return record?.ToDomain();
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task AddToManifestAsync(ImageAsset asset, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            var records = await ReadManifestUnsafeAsync(cancellationToken);
            records.Add(ImageAssetRecord.FromDomain(asset));

            await WriteManifestUnsafeAsync(records, cancellationToken);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<List<ImageAssetRecord>> ReadManifestUnsafeAsync(
        CancellationToken cancellationToken)
    {
        var manifestPath = GetManifestPath();

        if (!File.Exists(manifestPath))
        {
            return [];
        }

        await using var stream = File.OpenRead(manifestPath);

        var records = await JsonSerializer.DeserializeAsync<List<ImageAssetRecord>>(
            stream,
            JsonOptions,
            cancellationToken);

        return records ?? [];

    }

    private async Task WriteManifestUnsafeAsync(
        IReadOnlyCollection<ImageAssetRecord> records,
        CancellationToken cancellationToken)
    {
        var manifestPath = GetManifestPath();

        await using var stream = File.Create(manifestPath);
        await JsonSerializer.SerializeAsync(
            stream,
            records,
            JsonOptions,
            cancellationToken);
    }

    private string GetManifestPath()
    {
        return Path.Combine(_options.RootPath, "manifest.json");
    }

    private void EnsureStorageDirectoryExists()
    {
        Directory.CreateDirectory(_options.RootPath);
    }
}