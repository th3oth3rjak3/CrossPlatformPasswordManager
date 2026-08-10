using CrossPlatformPasswordManager.Core.Models;
using CrossPlatformPasswordManager.UI.ViewModels;

namespace CrossPlatformPasswordManager.UI.Services;

public interface INavigationService
{
    public event Action<ViewModelBase> NavigationRequested;
    public void Navigate<TViewModel>() where TViewModel : ViewModelBase;
    public void NavigateHome();
}