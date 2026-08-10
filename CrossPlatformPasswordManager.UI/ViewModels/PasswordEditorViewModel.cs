using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using CrossPlatformPasswordManager.Core.Models;

namespace CrossPlatformPasswordManager.UI.ViewModels;

public partial class PasswordEditorViewModel : ViewModelBase
{
    public int? EditingId { get; }

    [ObservableProperty]
    public partial string SiteName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Username { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Password { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string? ErrorMessage { get; set; }

    public IRelayCommand SaveCommand { get; }
    public IRelayCommand CancelCommand { get; }

    public event Action<PasswordEntryWriteDto?>? SaveRequested;

    public PasswordEditorViewModel(PasswordEntryReadDto? existingEntry = null)
    {
        if (existingEntry != null)
        {
            EditingId = existingEntry.Id;
            SiteName = existingEntry.Site;
            Username = existingEntry.Username;
            Password = existingEntry.Password;
        }

        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
    }

    private void Save()
    {
        if (string.IsNullOrWhiteSpace(SiteName) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "All fields are required.";
            return;
        }

        var dto = new PasswordEntryWriteDto
        {
            Site = SiteName,
            Username = Username,
            Password = Password
        };

        ErrorMessage = null;
        SaveRequested?.Invoke(dto);
    }

    private void Cancel()
    {
        SaveRequested?.Invoke(null);
    }
}
