using Avalonia.Media.Imaging;
using System.Collections.ObjectModel;
using System.Windows.Input;
using VisionLab.Client;
using VisionLab.Shared.Images;
using VisionLab.UI.Commands;
using VisionLab.UI.Services;

namespace VisionLab.UI.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly IVisionLabApiClient _apiClient;
    private readonly IFilePickerService _filePickerService;

    private bool _isBusy;
    private string _statusMessage = "Ready";

    private ImageAssetDto? _selectedImage;
    private Bitmap? _previewImage;
    private CancellationTokenSource? _previewCancellationTokenSource;

    public MainWindowViewModel(
        IVisionLabApiClient apiClient,
        IFilePickerService filePickerService)
    {
        _apiClient = apiClient;
        _filePickerService = filePickerService;

        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        UploadCommand = new AsyncRelayCommand(UploadAsync);
    }

    public ObservableCollection<ImageAssetDto> Images { get; } = [];

    public ICommand RefreshCommand { get; }

    public ICommand UploadCommand { get; }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public ImageAssetDto? SelectedImage
    {
        get => _selectedImage;
        set
        {
            if (ReferenceEquals(_selectedImage, value))
            {
                return;
            }

            _selectedImage = value;
            OnPropertyChanged();

            _ = LoadPreviewAsync(value);
        }
    }

    public Bitmap? PreviewImage
    {
        get => _previewImage;
        private set
        {
            if (ReferenceEquals(_previewImage, value))
            {
                return;
            }

            var oldImage = _previewImage;

            _previewImage = value;
            OnPropertyChanged();

            oldImage?.Dispose();
        }
    }

    private async Task RefreshAsync()
    {
        IsBusy = true;
        StatusMessage = "Loading images...";

        try
        {
            await RefreshImagesAsync();

            StatusMessage = $"Loaded {Images.Count} image(s).";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task UploadAsync()
    {
        IsBusy = true;
        StatusMessage = "Selecting image...";

        try
        {
            await using var pickedFile = await _filePickerService.PickImageAsync();

            if (pickedFile is null)
            {
                StatusMessage = "Upload cancelled.";
                return;
            }

            StatusMessage = $"Uploading {pickedFile.FileName}...";

            var uploadedImage = await _apiClient.UploadImageAsync(
                pickedFile.Stream,
                pickedFile.FileName,
                pickedFile.ContentType);

            await RefreshImagesAsync(uploadedImage.Id);

            StatusMessage = $"Uploaded {pickedFile.FileName}.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RefreshImagesAsync(Guid? preferredSelectedImageId = null)
    {
        var selectedImageId = preferredSelectedImageId ?? SelectedImage?.Id;

        var images = await _apiClient.GetImagesAsync();

        Images.Clear();

        foreach (var image in images.OrderByDescending(x => x.CreatedAt))
        {
            Images.Add(image);
        }

        if (selectedImageId is not null)
        {
            SelectedImage = Images.FirstOrDefault(x => x.Id == selectedImageId.Value);
        }
        else
        {
            SelectedImage = null;
        }
    }

    private async Task LoadPreviewAsync(ImageAssetDto? image)
    {
        var previousCancellationTokenSource = _previewCancellationTokenSource;

        _previewCancellationTokenSource = new CancellationTokenSource();

        previousCancellationTokenSource?.Cancel();
        previousCancellationTokenSource?.Dispose();

        if (image is null)
        {
            PreviewImage = null;
            return;
        }

        var cancellationToken = _previewCancellationTokenSource.Token;

        try
        {
            StatusMessage = $"Loading preview for {image.OriginalFileName}...";

            await using var stream = await _apiClient.GetImageContentAsync(
                image.Id,
                cancellationToken);

            if (stream is null)
            {
                PreviewImage = null;
                StatusMessage = "Preview not found.";
                return;
            }

            var bitmap = new Bitmap(stream);

            if (cancellationToken.IsCancellationRequested)
            {
                bitmap.Dispose();
                return;
            }

            PreviewImage = bitmap;
            StatusMessage = $"Preview loaded: {image.OriginalFileName}";
        }
        catch (OperationCanceledException)
        {
            // Ignore cancelled preview loads.
        }
        catch (Exception ex)
        {
            PreviewImage = null;
            StatusMessage = $"Preview error: {ex.Message}";
        }
    }
}