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
using Microsoft.Extensions.Logging;

using Serilog;

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

        // Logger for initial startup information.
        var logger = Services.GetRequiredService<ILogger<App>>();

        try
        {
            logger.LogInformation("=========================================");
            logger.LogInformation(" Starting CrossPlatformPasswordManager");
            logger.LogInformation(" Application Data Path: {Path}", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PasswordManager"));
            logger.LogInformation("=========================================");

            logger.LogInformation("[STARTUP] 1. Migrating Database...");
            MigrateDatabase(Services);

            logger.LogInformation("[STARTUP] 2. Loading Vault Session...");
            var session = LoadVaultSession(Services);
            Services.GetRequiredService<VaultSession>().Effect(state =>
            {
                state.CopyFrom(session);
            });

            logger.LogInformation("[STARTUP] 3. Resolving MainViewModel...");
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = Services.GetRequiredService<MainViewModel>()
                };
            }

            logger.LogInformation("[STARTUP] 4. Initialization Complete!");
            base.OnFrameworkInitializationCompleted();
        }
        catch (Exception ex)
        {
            logger.LogError($"[CRITICAL STARTUP ERROR] {ex}");
            throw;
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Determine path to Application Data logs directory
        var appDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PasswordManager",
            "logs"
        );
        Directory.CreateDirectory(appDataFolder);

        var logFilePath = Path.Combine(appDataFolder, "app-.log");

        // Configure Serilog to write to Daily Rolling Files & Terminal
        var serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
            .WriteTo.Console()
            .WriteTo.Debug()
            .CreateLogger();

        // Logging Configuration for Terminal (stdout/stderr)
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(serilogLogger);
        });

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
            _ = options.UseSqlite($"Data Source={dbPath}");
        });

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddTransient<SetMasterPasswordViewModel>();
        services.AddTransient<UnlockVaultViewModel>();
        services.AddTransient<VaultViewModel>();
        services.AddTransient<StartupErrorViewModel>();
        services.AddTransient<VaultEntriesViewModel>();
        services.AddTransient<ManageBackupsViewModel>();
    }

    private static VaultSession LoadVaultSession(IServiceProvider services)
    {
        var dbContextFactory = services.GetRequiredService<IDbContextFactory<PasswordManagerContext>>();
        using var context = dbContextFactory.CreateDbContext();
        var masterPw = context.MasterPasswords.OrderBy(x => x.Id).First();
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
