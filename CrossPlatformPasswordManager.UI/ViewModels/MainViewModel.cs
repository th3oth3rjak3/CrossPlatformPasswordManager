using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossPlatformPasswordManager.Core.Services;
using CrossPlatformPasswordManager.UI.Services;

namespace CrossPlatformPasswordManager.UI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IAuthenticationService _authService;

    [ObservableProperty]
    public partial ViewModelBase? CurrentPage { get; set; }

    public IAsyncRelayCommand EvaluateAndNavigateCommand { get; }

    public MainViewModel(IAuthenticationService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
        _navigationService.NavigationRequested += OnNavigationRequested;
        EvaluateAndNavigateCommand = new AsyncRelayCommand(EvaluateAndNavigateAsync);
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
}
