using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using CrossPlatformPasswordManager.Core.Models;
using CrossPlatformPasswordManager.Core.Services;
using CrossPlatformPasswordManager.UI.ViewModels;
using CrossPlatformPasswordManager.UI.Views;

using Microsoft.Extensions.DependencyInjection;

namespace CrossPlatformPasswordManager.UI;

public partial class App : Application
{
    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        Services = serviceCollection.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Core State & Services
        services.AddSingleton<VaultSession>();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddTransient<SetMasterPasswordViewModel>();
        services.AddTransient<UnlockVaultViewModel>();
        services.AddTransient<VaultViewModel>();
    }
}
