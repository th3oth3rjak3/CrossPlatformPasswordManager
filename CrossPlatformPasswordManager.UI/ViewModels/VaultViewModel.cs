using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using CrossPlatformPasswordManager.Core.Services;
using CrossPlatformPasswordManager.UI.Services;

using Microsoft.Extensions.DependencyInjection;

namespace CrossPlatformPasswordManager.UI.ViewModels;

public partial class VaultViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    public partial ViewModelBase? CurrentSubPage { get; set; }

    public IRelayCommand ShowAllPasswordsCommand { get; }
    public IRelayCommand ShowManageBackupsCommand { get; }
    public IRelayCommand ShowChangeMasterPasswordCommand { get; }
    public IRelayCommand LockVaultCommand { get; }

    public VaultViewModel(IAuthenticationService authService, INavigationService navigationService, IServiceProvider serviceProvider)
    {
        _authService = authService;
        _navigationService = navigationService;
        _serviceProvider = serviceProvider;

        ShowAllPasswordsCommand = new RelayCommand(ShowAllPasswords);
        ShowManageBackupsCommand = new RelayCommand(ShowManageBackups);
        ShowChangeMasterPasswordCommand = new RelayCommand(ShowChangeMasterPassword);
        LockVaultCommand = new RelayCommand(LockVault);

        // Default sub-page on vault unlock
        ShowAllPasswords();
    }

    private void ShowAllPasswords()
    {
        CurrentSubPage = _serviceProvider.GetRequiredService<VaultEntriesViewModel>();
    }

    private void ShowManageBackups()
    {
        CurrentSubPage = _serviceProvider.GetRequiredService<ManageBackupsViewModel>();
    }

    private void ShowChangeMasterPassword()
    {
        CurrentSubPage = _serviceProvider.GetRequiredService<SetMasterPasswordViewModel>();
    }

    private void LockVault()
    {
        _authService.Logout();
        _navigationService.NavigateBasedOnVaultSessionState();
    }
}
