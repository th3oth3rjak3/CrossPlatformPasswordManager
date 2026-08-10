using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using CrossPlatformPasswordManager.Core.Models;

namespace CrossPlatformPasswordManager.UI.ViewModels;

public partial class PasswordEntryDisplayItem : ViewModelBase
{
    public PasswordEntry RawEntry { get; }

    [ObservableProperty]
    public partial bool IsPasswordVisible { get; set; }

    public int Id => RawEntry.Id;
    public string SiteName => RawEntry.SiteName;
    public string Username => RawEntry.Username;
    public string PlaintextPassword { get; }

    public string DisplayPassword => IsPasswordVisible ? PlaintextPassword : "••••••••••••";

    public IRelayCommand ToggleShowPasswordCommand { get; }
    public IAsyncRelayCommand CopyPasswordCommand { get; }

    public PasswordEntryDisplayItem(PasswordEntry rawEntry, string plaintextPassword)
    {
        RawEntry = rawEntry;
        PlaintextPassword = plaintextPassword;

        ToggleShowPasswordCommand = new RelayCommand(ToggleShowPassword);
        CopyPasswordCommand = new AsyncRelayCommand(CopyPasswordAsync);
    }

    private void ToggleShowPassword()
    {
        IsPasswordVisible = !IsPasswordVisible;
        OnPropertyChanged(nameof(DisplayPassword));
    }

    private async Task CopyPasswordAsync()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var clipboard = desktop.MainWindow?.Clipboard;
            if (clipboard != null)
            {
                await clipboard.SetTextAsync(PlaintextPassword);
            }
        }
    }
}
