using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using VisionLab.Client;
using VisionLab.UI.Services;
using VisionLab.UI.ViewModels;
using VisionLab.UI.Views;

namespace VisionLab.UI;

public partial class App : Application
{
    public IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            base.OnFrameworkInitializationCompleted();
            return;
        }

        Services = ConfigureServices(desktop);

        desktop.MainWindow = new MainWindow
        {
            DataContext = Services.GetRequiredService<MainWindowViewModel>()
        };

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider ConfigureServices(
        IClassicDesktopStyleApplicationLifetime desktop)
    {
        var services = new ServiceCollection();

        services.AddSingleton(desktop);

        services.AddVisionLabClient(options =>
        {
            options.BaseAddress = new Uri("http://localhost:5056");
        });

        services.AddSingleton<IFilePickerService, AvaloniaFilePickerService>();

        services.AddTransient<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }
}