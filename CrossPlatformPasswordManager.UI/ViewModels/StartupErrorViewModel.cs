using CommunityToolkit.Mvvm.ComponentModel;

namespace CrossPlatformPasswordManager.UI.ViewModels;

public partial class StartupErrorViewModel(Exception exception) : ViewModelBase
{
    public static string Title => "Unable to Start Password Manager";

    public string Message { get; } = exception.Message;
}