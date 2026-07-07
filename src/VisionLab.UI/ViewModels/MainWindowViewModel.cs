using System.Collections.ObjectModel;
using System.Windows.Input;
using VisionLab.Client;
using VisionLab.Shared.Images;
using VisionLab.UI.Commands;

namespace VisionLab.UI.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly IVisionLabApiClient _apiClient;

    private bool _isBusy;
    private string _statusMessage = "Ready";

    public MainWindowViewModel(IVisionLabApiClient apiClient)
    {
        _apiClient = apiClient;

        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
    }

    public ObservableCollection<ImageAssetDto> Images { get; } = [];

    public ICommand RefreshCommand { get; }

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
            var images = await _apiClient.GetImagesAsync();

            Images.Clear();

            foreach (var image in images.OrderByDescending(x => x.CreatedAt))
            {
                Images.Add(image);
            }

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
}