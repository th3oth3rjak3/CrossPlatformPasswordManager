using System.Diagnostics;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using CrossPlatformPasswordManager.Core.Context;
using CrossPlatformPasswordManager.Core.Models;
using CrossPlatformPasswordManager.Core.Services;
using CrossPlatformPasswordManager.UI.Services;
using CrossPlatformPasswordManager.UI.ViewModels;
using CrossPlatformPasswordManager.UI.Views;

using Functional;

using Microsoft.EntityFrameworkCore;
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

        MigrateDatabase(Services);
        var session = LoadVaultSession(Services);
        Services.GetRequiredService<VaultSession>().Effect(state =>
        {
            state.CopyFrom(session);
        });

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var idleTimerService = Services.GetRequiredService<IdleTimerService>();
            desktop.MainWindow = new MainWindow(idleTimerService)
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
        services.AddSingleton<DatabaseConnectionManager>();
        services.AddSingleton<MasterPasswordService>();
        services.AddSingleton<PasswordEntryService>();
        services.AddSingleton<IdleTimerService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddDbContextFactory<PasswordManagerContext>((sp, options) =>
        {
            var connectionManager = sp.GetRequiredService<DatabaseConnectionManager>();
            var dbPath = connectionManager.DatabasePath;
            Debug.WriteLine(dbPath);
            Console.WriteLine(dbPath);
            _ = options.UseSqlite($"Data Source={dbPath}");
        });

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddTransient<SetMasterPasswordViewModel>();
        services.AddTransient<UnlockVaultViewModel>();
        services.AddTransient<VaultViewModel>();
        services.AddTransient<StartupErrorViewModel>();
    }

    private static VaultSession LoadVaultSession(IServiceProvider services)
    {
        var dbContextFactory = services.GetRequiredService<IDbContextFactory<PasswordManagerContext>>();
        using var context = dbContextFactory.CreateDbContext();
        var masterPw = context.MasterPasswords.First();
        return new VaultSession
        {
            IsLoggedIn = false,
            MasterPasswordHash = masterPw.PasswordHash,
            KeyDerivationSalt = masterPw.KeyDerivationSalt,
        };
    }

    private static void MigrateDatabase(IServiceProvider services)
    {
        var dbContextFactory = services.GetRequiredService<IDbContextFactory<PasswordManagerContext>>();
        using var db = dbContextFactory.CreateDbContext();
        db.Database.Migrate();
    }
}
