using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using CrossPlatformPasswordManager.Core.Services;
using CrossPlatformPasswordManager.UI.Services;

using Functional;

namespace CrossPlatformPasswordManager.UI.ViewModels;

public partial class SetMasterPasswordViewModel(
    MasterPasswordService masterPasswordService,
    INavigationService navigationService,
    IdleTimerService idleTimerService,
    IAuthenticationService authService
) : ViewModelBase
{
    [ObservableProperty]
    public partial string MasterPassword { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ConfirmPassword { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string? ValidationMessage { get; set; }

    [RelayCommand]
    private async Task SetMasterPassword()
    {
        ValidationMessage = null;

        if (string.IsNullOrEmpty(MasterPassword))
        {
            ValidationMessage = "Please enter a master password.";
            return;
        }

        if (MasterPassword != ConfirmPassword)
        {
            ValidationMessage = "The passwords do not match.";
            return;
        }

        await masterPasswordService.SetMasterPassword(MasterPassword)
            .EffectAsync(
                () =>
                {
                    ValidationMessage = authService.Login(MasterPassword);

                    if (!string.IsNullOrWhiteSpace(ValidationMessage))
                    {
                        return;
                    }

                    idleTimerService.StartTimer();
                    navigationService.NavigateBasedOnVaultSessionState();

                },
                err => ValidationMessage = err.Message);
    }
}