using Avalonia.Controls;

using CrossPlatformPasswordManager.UI.ViewModels;

namespace CrossPlatformPasswordManager.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Opened += OnWindowOpened;
    }

    private async void OnWindowOpened(object? sender, EventArgs e)
    {
        if (DataContext is MainViewModel mainViewModel)
        {
            await mainViewModel.EvaluateAndNavigateAsync();
        }
    }
}
