using CommunityToolkit.Mvvm.Input;

using CrossPlatformPasswordManager.Core.Models;
using CrossPlatformPasswordManager.Core.Services;

namespace CrossPlatformPasswordManager.UI.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authService;

    public ViewModelBase? CurrentPage { get; set => SetProperty(ref field, value); }

    public IAsyncRelayCommand EvaluateAndNavigateCommand { get; }

    public MainViewModel(IAuthenticationService authService)
    {
        _authService = authService;
        EvaluateAndNavigateCommand = new AsyncRelayCommand(EvaluateAndNavigateAsync);
    }

    /// <summary>
    /// Evaluates the current authentication state and updates the visible active page ViewModel.
    /// </summary>
    public async Task EvaluateAndNavigateAsync()
    {
        var state = await _authService.GetCurrentStateAsync();

        CurrentPage = state switch
        {
            AuthenticationState.SetMasterPasswordRequired => new SetMasterPasswordViewModel(),
            AuthenticationState.UnlockRequired => new UnlockVaultViewModel(),
            AuthenticationState.Authenticated => new VaultViewModel(),
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Unsupported authentication state.")
        };
    }
}
