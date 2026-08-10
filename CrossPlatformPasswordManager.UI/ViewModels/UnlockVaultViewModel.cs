using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using CrossPlatformPasswordManager.Core.Services;
using CrossPlatformPasswordManager.UI.Services;

namespace CrossPlatformPasswordManager.UI.ViewModels;

public partial class UnlockVaultViewModel(
    IAuthenticationService authenticationService,
    INavigationService navigationService
) : ViewModelBase
{
    [ObservableProperty]
    public partial string MasterPassword { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string? ValidationMessage { get; set; }

    [RelayCommand]
    private void UnlockVault()
    {
        ValidationMessage = null;

        if (string.IsNullOrEmpty(MasterPassword))
        {
            ValidationMessage = "Please enter your master password.";
            return;
        }

        ValidationMessage = authenticationService.Login(MasterPassword);

        if (!string.IsNullOrEmpty(ValidationMessage))
        {
            return;
        }

        navigationService.NavigateBasedOnVaultSessionState();
    }
}
