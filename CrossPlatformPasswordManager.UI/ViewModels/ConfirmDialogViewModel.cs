using CommunityToolkit.Mvvm.Input;

namespace CrossPlatformPasswordManager.UI.ViewModels;

public enum ConfirmDialogSeverity
{
    Primary,
    Warning,
    Danger
}

public partial class ConfirmDialogViewModel : ViewModelBase
{
    public string Title { get; }
    public string Message { get; }
    public string ConfirmButtonText { get; }
    public string CancelButtonText { get; }

    // Boolean helpers for Avalonia Class toggles
    public bool IsDanger => Severity == ConfirmDialogSeverity.Danger;
    public bool IsWarning => Severity == ConfirmDialogSeverity.Warning;
    public bool IsPrimary => Severity == ConfirmDialogSeverity.Primary;

    public ConfirmDialogSeverity Severity { get; }
    public IRelayCommand ConfirmCommand { get; }
    public IRelayCommand CancelCommand { get; }

    public event Action<bool>? CloseRequested;

    public ConfirmDialogViewModel(
        string title,
        string message,
        string confirmButtonText = "Confirm",
        string cancelButtonText = "Cancel",
        ConfirmDialogSeverity severity = ConfirmDialogSeverity.Primary)
    {
        Title = title;
        Message = message;
        ConfirmButtonText = confirmButtonText;
        CancelButtonText = cancelButtonText;
        Severity = severity;

        ConfirmCommand = new RelayCommand(() => CloseRequested?.Invoke(true));
        CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(false));
    }
}