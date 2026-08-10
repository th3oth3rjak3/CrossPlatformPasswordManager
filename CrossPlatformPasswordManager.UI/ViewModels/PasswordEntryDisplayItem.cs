using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CrossPlatformPasswordManager.UI.ViewModels;

public partial class PasswordEntryDisplayItem : ViewModelBase
{
    private static int _clipboardCopyToken = 0;

    public int Id { get; }
    public string SiteName { get; }
    public string Username { get; }
    public string PlaintextPassword { get; }

    [ObservableProperty]
    public partial bool IsPasswordVisible { get; set; }

    public string DisplayPassword => IsPasswordVisible ? PlaintextPassword : "••••••••••••";

    public IRelayCommand ToggleShowPasswordCommand { get; }
    public IAsyncRelayCommand CopyPasswordCommand { get; }
    public IAsyncRelayCommand CopyUsernameCommand { get; }
    public IRelayCommand EditCommand { get; }
    public IRelayCommand DeleteCommand { get; }

    public PasswordEntryDisplayItem(
        int id,
        string siteName,
        string username,
        string plaintextPassword,
        Action<PasswordEntryDisplayItem> onEdit,
        Action<PasswordEntryDisplayItem> onDelete)
    {
        Id = id;
        SiteName = siteName;
        Username = username;
        PlaintextPassword = plaintextPassword;

        ToggleShowPasswordCommand = new RelayCommand(ToggleShowPassword);
        CopyPasswordCommand = new AsyncRelayCommand(CopyPasswordAsync);
        CopyUsernameCommand = new AsyncRelayCommand(CopyUsernameAsync);
        EditCommand = new RelayCommand(() => onEdit(this));
        DeleteCommand = new RelayCommand(() => onDelete(this));
    }

    private void ToggleShowPassword()
    {
        IsPasswordVisible = !IsPasswordVisible;
        OnPropertyChanged(nameof(DisplayPassword));
    }

    private async Task CopyPasswordAsync()
    {
        await CopyToClipboardWithAutoClearAsync(PlaintextPassword, timeoutSeconds: 30);
    }

    private async Task CopyUsernameAsync()
    {
        await CopyToClipboardWithAutoClearAsync(Username, timeoutSeconds: 30);
    }

    private static async Task CopyToClipboardWithAutoClearAsync(string text, int timeoutSeconds = 30)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var clipboard = desktop.MainWindow?.Clipboard;
            if (clipboard == null)
                return;

            // Unique ID for this specific copy operation
            var currentToken = System.Threading.Interlocked.Increment(ref _clipboardCopyToken);

            // 1. Copy text to system clipboard immediately
            await clipboard.SetTextAsync(text);

            // 2. Fire-and-forget background timer (does not block caller)
            _ = AutoClearAfterDelayAsync();

            async Task AutoClearAfterDelayAsync()
            {
                // Non-blocking 30-second delay
                await Task.Delay(TimeSpan.FromSeconds(timeoutSeconds));

                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    // Check 1: Ensure no newer copy was performed in our app
                    if (_clipboardCopyToken != currentToken)
                        return;

                    // Check 2: Ensure user hasn't copied external text in the meantime
                    var currentClipboardText = await clipboard.TryGetTextAsync();
                    if (currentClipboardText == text)
                    {
                        await clipboard.SetTextAsync(string.Empty);
                    }
                });
            }
        }
    }
}
