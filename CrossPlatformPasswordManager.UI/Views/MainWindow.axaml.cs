using Avalonia.Controls;
using Avalonia.Interactivity;

using CrossPlatformPasswordManager.UI.Services;
using CrossPlatformPasswordManager.UI.ViewModels;

namespace CrossPlatformPasswordManager.UI.Views;

public partial class MainWindow : Window
{
    private readonly IdleTimerService _idleTimerService;

    public MainWindow(IdleTimerService idleTimerService)
    {
        InitializeComponent();
        _idleTimerService = idleTimerService;
        Opened += OnWindowOpened;

        AddHandler(TappedEvent, OnUserActivity, RoutingStrategies.Tunnel, handledEventsToo: true);
        AddHandler(KeyDownEvent, OnUserActivity, RoutingStrategies.Tunnel, handledEventsToo: true);
    }

    private void OnWindowOpened(object? sender, EventArgs e)
    {
        if (DataContext is MainViewModel mainViewModel)
        {
            mainViewModel.EvaluateAndNavigateCommand.Execute(null);
        }
    }

    private void OnUserActivity(object? sender, RoutedEventArgs e)
    {
        _idleTimerService.SetLastActivity();
    }
}