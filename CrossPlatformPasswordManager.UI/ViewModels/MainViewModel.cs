using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using CrossPlatformPasswordManager.Core.Services;
using CrossPlatformPasswordManager.UI.Services;

namespace CrossPlatformPasswordManager.UI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IAuthenticationService _authService;
    private readonly IdleTimerService _idleTimerService;

    [ObservableProperty]
    public partial ViewModelBase? CurrentPage { get; set; }

    [ObservableProperty]
    public partial string TimeRemainingText { get; set; } = "Time Remaining: --:--";

    public IAsyncRelayCommand EvaluateAndNavigateCommand { get; }

    public MainViewModel(IAuthenticationService authService, INavigationService navigationService, IdleTimerService idleTimerService)
    {
        _authService = authService;
        _idleTimerService = idleTimerService;
        _idleTimerService.OnIdle += OnIdleTimeout;
        _idleTimerService.OnTick += OnTimerTick;
        _navigationService = navigationService;
        _navigationService.NavigationRequested += OnNavigationRequested;
        EvaluateAndNavigateCommand = new AsyncRelayCommand(EvaluateAndNavigateAsync);
    }

    public void RegisterUserActivity()
    {
        _idleTimerService.SetLastActivity();
    }

    public void OnTimerTick()
    {
        // Must update UI property on the Avalonia UI Thread
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var remaining = _idleTimerService.TimeRemaining;
            if (remaining < TimeSpan.Zero)
            {
                remaining = TimeSpan.Zero;
            }
            TimeRemainingText = $"Time Remaining: {remaining:mm\\:ss}";
        });
    }

    private void OnNavigationRequested(ViewModelBase viewModel)
    {
        CurrentPage = viewModel;
    }

    /// <summary>
    /// Evaluates the current authentication state and updates the visible active page ViewModel.
    /// </summary>
    private async Task EvaluateAndNavigateAsync()
    {
        try
        {
            var init = await _authService.ReloadAllAuthState();
            init.Unwrap();
            _navigationService.NavigateBasedOnVaultSessionState();
        }
        catch (Exception ex)
        {
            CurrentPage = new StartupErrorViewModel(ex);
        }
    }

    private void OnIdleTimeout()
    {
        // Safely execute UI navigation on the Avalonia UI Thread
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            _authService.Logout();
            _idleTimerService.StopTimer();
            _navigationService.NavigateBasedOnVaultSessionState();
        });
    }
}
