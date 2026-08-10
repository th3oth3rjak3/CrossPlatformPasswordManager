using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using CrossPlatformPasswordManager.Core.Services;
using CrossPlatformPasswordManager.UI.Services;

using Functional;

namespace CrossPlatformPasswordManager.UI.ViewModels;

public partial class ManageBackupsViewModel : ViewModelBase
{
    private readonly DatabaseConnectionManager _connectionManager;
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    public partial ObservableCollection<BackupDisplayItem> Backups { get; set; } = [];

    [ObservableProperty]
    public partial ViewModelBase? ActiveOverlay { get; set; }

    public IAsyncRelayCommand CreateBackupCommand { get; }
    public IAsyncRelayCommand LoadBackupsCommand { get; }

    public ManageBackupsViewModel(DatabaseConnectionManager connectionManager, IAuthenticationService authService, INavigationService navigationService)
    {
        _connectionManager = connectionManager;
        _authService = authService;
        _navigationService = navigationService;

        CreateBackupCommand = new AsyncRelayCommand(CreateBackupAsync);
        LoadBackupsCommand = new AsyncRelayCommand(LoadBackupsAsync);

        _ = LoadBackupsAsync();
    }

    public async Task LoadBackupsAsync()
    {
        var result = await _connectionManager
            .GetAllBackupsAsync()
            .EffectOkAsync(async files =>
            {
                Backups.Clear();
                foreach (var file in files)
                {
                    Backups.Add(new BackupDisplayItem(
                        file,
                        onRestore: item => _ = PromptRestoreAsync(item),
                        onDelete: item => PromptDelete(item)
                    ));
                }
                await Task.CompletedTask;
            });
    }

    private async Task CreateBackupAsync()
    {
        var backupFile = await _connectionManager.CreateBackupAsync();
        await LoadBackupsAsync();
    }

    private async Task PromptRestoreAsync(BackupDisplayItem item)
    {
        var confirmDialog = new ConfirmDialogViewModel(
            title: "Restore Database Backup?",
            message: $"Are you sure you want to restore '{item.FileName}'? Your active vault will be locked and replaced with the restored backup.",
            confirmButtonText: "Restore & Lock Vault",
            severity: ConfirmDialogSeverity.Warning
        );

        confirmDialog.CloseRequested += async (confirmed) =>
        {
            if (confirmed)
            {
                // 1. Restore the SQLite database file
                await _connectionManager.RestoreFromBackupAsync(item.FullPath);

                // 2. Lock active session and clear stale AES keys from memory
                _authService.Logout();

                // 3. Reload state from newly restored SQLite file
                await _authService.ReloadAllAuthState();

                // 4. Force user back to Unlock Screen to re-authenticate with the restored password!
                _navigationService.NavigateBasedOnVaultSessionState();
            }

            ActiveOverlay = null;
        };

        ActiveOverlay = confirmDialog;
    }

    private void PromptDelete(BackupDisplayItem item)
    {
        var confirmDialog = new ConfirmDialogViewModel(
            title: "Delete Database Backup?",
            message: $"Are you sure you want to delete backup '{item.FileName}'? This action cannot be undone.",
            confirmButtonText: "Delete Backup",
            severity: ConfirmDialogSeverity.Danger);

        confirmDialog.CloseRequested += (confirmed) =>
        {
            if (confirmed)
            {
                if (item.File.Exists)
                {
                    item.File.Delete();
                    Backups.Remove(item);
                }
            }
            ActiveOverlay = null;
        };

        ActiveOverlay = confirmDialog;
    }
}
