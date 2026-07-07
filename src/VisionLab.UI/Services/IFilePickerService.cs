namespace VisionLab.UI.Services;

public interface IFilePickerService
{
    Task<PickedImageFile?> PickImageAsync(
        CancellationToken cancellationToken = default);
}