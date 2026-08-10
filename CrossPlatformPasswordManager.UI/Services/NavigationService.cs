using CrossPlatformPasswordManager.Core.Models;
using CrossPlatformPasswordManager.Core.Services;
using CrossPlatformPasswordManager.UI.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace CrossPlatformPasswordManager.UI.Services;

public sealed class NavigationService(IServiceProvider serviceProvider, IAuthenticationService authService, IdleTimerService idleTimerService)
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
                idleTimerService.StopTimer();
                Navigate<SetMasterPasswordViewModel>();
                break;

            case AuthenticationState.UnlockRequired:
                idleTimerService.StopTimer();
                Navigate<UnlockVaultViewModel>();
                break;

            case AuthenticationState.Authenticated:
                idleTimerService.StartTimer();
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