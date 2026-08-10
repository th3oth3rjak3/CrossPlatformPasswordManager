using CommunityToolkit.Mvvm.Input;

namespace CrossPlatformPasswordManager.UI.ViewModels;

public partial class ConfirmDeleteViewModel : ViewModelBase
{
    public string SiteName { get; }

    public IRelayCommand ConfirmCommand { get; }
    public IRelayCommand CancelCommand { get; }

    public event Action<bool>? CloseRequested;

    public ConfirmDeleteViewModel(string siteName)
    {
        SiteName = siteName;

        ConfirmCommand = new RelayCommand(() => CloseRequested?.Invoke(true));
        CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(false));
    }
}
