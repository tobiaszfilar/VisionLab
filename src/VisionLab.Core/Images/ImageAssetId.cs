namespace VisionLab.Core.Images;

public readonly record struct ImageAssetId(Guid Value)
{
    public static ImageAssetId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString("N");
}
