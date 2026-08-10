using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

using CrossPlatformPasswordManager.UI.ViewModels;

namespace CrossPlatformPasswordManager.UI.Views;

public partial class MainWindow : Window
{
    // Throttle timestamp: only allow activity updates once per second
    private DateTime _lastActivityReported = DateTime.MinValue;

    public MainWindow()
    {
        InitializeComponent();
        Opened += OnWindowOpened;

        AddHandler(TappedEvent, OnUserActivity, RoutingStrategies.Tunnel, handledEventsToo: true);
        AddHandler(KeyDownEvent, OnUserActivity, RoutingStrategies.Tunnel, handledEventsToo: true);
        // Mouse / Pointer Movements (Throttled)
        AddHandler(PointerMovedEvent, OnPointerMovedActivity, RoutingStrategies.Tunnel, handledEventsToo: true);
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
        ReportUserActivity();
    }

    /// <summary>
    /// Handles continuous mouse movement events with a 1-second throttle.
    /// </summary>
    private void OnPointerMovedActivity(object? sender, PointerEventArgs e)
    {
        // Throttle check: ignore movement events if reported less than 1 second ago
        if ((DateTime.Now - _lastActivityReported).TotalSeconds >= 1)
        {
            ReportUserActivity();
        }
    }

    private void ReportUserActivity()
    {
        _lastActivityReported = DateTime.Now;

        if (DataContext is MainViewModel mainViewModel)
        {
            mainViewModel.RegisterUserActivity();
        }
    }
}