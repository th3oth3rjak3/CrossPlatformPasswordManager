using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using CrossPlatformPasswordManager.Core.Models;
using CrossPlatformPasswordManager.Core.Services;

namespace CrossPlatformPasswordManager.UI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authService;

    [ObservableProperty]
    public partial ViewModelBase? CurrentPage { get; set; }

    public IAsyncRelayCommand EvaluateAndNavigateCommand { get; }

    public MainViewModel(IAuthenticationService authService)
    {
        _authService = authService;
        EvaluateAndNavigateCommand = new AsyncRelayCommand(EvaluateAndNavigateAsync);
    }

    /// <summary>
    /// Evaluates the current authentication state and updates the visible active page ViewModel.
    /// </summary>
    private async Task EvaluateAndNavigateAsync()
    {
        try
        {
            var state = await _authService.GetCurrentStateAsync();

            CurrentPage = state switch
            {
                AuthenticationState.SetMasterPasswordRequired =>
                    new SetMasterPasswordViewModel(),

                AuthenticationState.UnlockRequired =>
                    new UnlockVaultViewModel(),

                AuthenticationState.Authenticated =>
                    new VaultViewModel(),

                _ => throw new ArgumentOutOfRangeException(
                    nameof(state),
                    state,
                    "Unsupported authentication state.")
            };
        }
        catch (Exception ex)
        {
            CurrentPage = new StartupErrorViewModel(ex);
        }
    }
}
