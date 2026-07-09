using Microsoft.Extensions.Options;
using VisionLab.Infrastructure.Images;

namespace VisionLab.Tests.Infrastructure;

public sealed class DiskImageStorageTests : IDisposable
{
    private readonly string _rootPath;

    public DiskImageStorageTests()
    {
        _rootPath = Path.Combine(
            Path.GetTempPath(),
            "VisionLabTests",
            Guid.NewGuid().ToString("N"));
    }

    [Fact]
    public async Task SaveAsync_should_save_file_and_metada()
    {
        var storage = CreateStorage();

        await using var stream = new MemoryStream([1, 2, 3, 4]);

        var asset = await storage.SaveAsync(
            stream,
            "cat.png",
            "image/png",
            4,
            CancellationToken.None);

        var storedFilePath = Path.Combine(_rootPath, asset.StoredFileName);
        var manifestPath = Path.Combine(_rootPath, "manifest.json");

        Assert.True(File.Exists(storedFilePath));
        Assert.True(File.Exists(manifestPath));

        var assets = await storage.GetAllAsync(CancellationToken.None);
        var savedAsset = Assert.Single(assets);

        Assert.Equal(asset.Id, savedAsset.Id);
        Assert.Equal("cat.png", savedAsset.OriginalFileName);
        Assert.Equal("image/png", savedAsset.ContentType);
        Assert.Equal(4, savedAsset.SizeInBytes);
    }

    private DiskImageStorage CreateStorage()
    {
        var options = Options.Create(new ImageStorageOptions
        {
            RootPath = _rootPath
        });

        return new DiskImageStorage(options);
    }

    public void Dispose()
    {
        if (Directory.Exists(_rootPath))
        {
            Directory.Delete(_rootPath, recursive: true);
        }
    }
}