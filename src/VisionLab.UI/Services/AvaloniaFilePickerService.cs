using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace VisionLab.UI.Services;

public sealed class AvaloniaFilePickerService : IFilePickerService
{
    private readonly IClassicDesktopStyleApplicationLifetime _desktopLifetime;

    public AvaloniaFilePickerService(
        IClassicDesktopStyleApplicationLifetime desktopLifetime)
    {
        _desktopLifetime = desktopLifetime;
    }

    public async Task<PickedImageFile?> PickImageAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var window = _desktopLifetime.MainWindow;

        if (window is null)
        {
            return null;
        }

        var files = await window.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "Select image",
                AllowMultiple = false,
                FileTypeFilter =
                [
                    FilePickerFileTypes.ImageAll
                ]
            });

        var file = files.Count > 0
            ? files[0]
            : null;

        if (file is null)
        {
            return null;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var stream = await file.OpenReadAsync();

        return new PickedImageFile(
            file.Name,
            GuessContentType(file.Name),
            stream);
    }

    private static string GuessContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName)
            .ToLowerInvariant();

        return extension switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".bmp" => "image/bmp",
            ".gif" => "image/gif",
            ".tif" or ".tiff" => "image/tiff",
            ".webp" => "image/webp",
            _ => "image/unknown"
        };
    }
}