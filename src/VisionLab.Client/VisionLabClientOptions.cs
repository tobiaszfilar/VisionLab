namespace VisionLab.Client;

public sealed class VisionLabClientOptions
{
    public Uri BaseAddress { get; set; } = new("http://localhost:5056");
}