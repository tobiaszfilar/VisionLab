using VisionLab.Core.Images;

namespace VisionLab.Tests.Core;

public sealed class ImageAssetTests
{
    [Fact]
    public void Create_shuld_create_valid_image_asset()
    {
        var createdAt = DateTimeOffset.UtcNow;
        var asset = ImageAsset.Create(
            originalFileName: "cat.png",
            storedFileName: "abc123.png",
            contentType: "image/png",
            sizeInBytes: 1024,
            createdAt: createdAt);

        Assert.NotEqual(Guid.Empty, asset.Id.Value);
        Assert.Equal("cat.png", asset.OriginalFileName);
        Assert.Equal("abc123.png", asset.StoredFileName);
        Assert.Equal("image/png", asset.ContentType);
        Assert.Equal(1024, asset.SizeInBytes);
        Assert.Equal(createdAt, asset.CreatedAt);
    }

    [Fact]
    public void Create_should_throw_when_size_is_zero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            ImageAsset.Create(
                originalFileName: "cat.png",
                storedFileName: "abc123.png",
                contentType: "image/png",
                sizeInBytes: 0,
                createdAt: DateTimeOffset.UtcNow);
        });
    }
}