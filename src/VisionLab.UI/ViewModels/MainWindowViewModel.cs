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

            await _apiClient.UploadImageAsync(
                pickedFile.Stream,
                pickedFile.FileName,
                pickedFile.ContentType);

            await RefreshImagesAsync();

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

    private async Task RefreshImagesAsync()
    {
        var images = await _apiClient.GetImagesAsync();

        Images.Clear();

        foreach (var image in images.OrderByDescending(x => x.CreatedAt))
        {
            Images.Add(image);
        }
    }
}