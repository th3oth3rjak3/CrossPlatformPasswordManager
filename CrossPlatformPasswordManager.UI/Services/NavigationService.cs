using CrossPlatformPasswordManager.Core.Models;
using CrossPlatformPasswordManager.Core.Services;
using CrossPlatformPasswordManager.UI.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace CrossPlatformPasswordManager.UI.Services;

public sealed class NavigationService(IServiceProvider serviceProvider, IAuthenticationService authService)
    : INavigationService
{
    public event Action<ViewModelBase>? NavigationRequested;

    public void Navigate<TViewModel>() where TViewModel : ViewModelBase
    {
        var viewModel = serviceProvider.GetRequiredService<TViewModel>();
        NavigationRequested?.Invoke(viewModel);
    }

    public void NavigateBasedOnVaultSessionState()
    {
        var state = authService.GetCurrentState();

        switch (state)
        {
            case AuthenticationState.SetMasterPasswordRequired:
                Navigate<SetMasterPasswordViewModel>();
                break;

            case AuthenticationState.UnlockRequired:
                Navigate<UnlockVaultViewModel>();
                break;

            case AuthenticationState.Authenticated:
                Navigate<VaultViewModel>();
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(state),
                    state,
                    "Unsupported authentication state.");
        }
    }
}