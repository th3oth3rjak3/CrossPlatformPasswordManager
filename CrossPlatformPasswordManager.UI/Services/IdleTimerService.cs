using Avalonia.Threading;

namespace CrossPlatformPasswordManager.UI.Services;

public partial class IdleTimerService : IDisposable
{
    private DispatcherTimer _timer;
    private DateTime _lastActivity = DateTime.Now;
    private readonly int _timeoutMinutes;

    public event Action? OnIdle;
    public event Action? OnTick;

    public TimeSpan TimeRemaining =>
        TimeSpan.FromMinutes(_timeoutMinutes) - (DateTime.Now - _lastActivity);

    public void SetLastActivity() =>
        _lastActivity = DateTime.Now;

    public IdleTimerService(int timeoutMinutes = 5)
    {
        _timeoutMinutes = timeoutMinutes;
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += TimerTick;
    }

    public void StartTimer() => _timer.Start();

    public void StopTimer() => _timer.Stop();

    private void TimerTick(object? sender, EventArgs e)
    {
        OnTick?.Invoke();
        if ((DateTime.Now - _lastActivity).TotalMinutes >= _timeoutMinutes)
        {
            OnIdle?.Invoke();
        }
    }

    public void Dispose()
    {
        _timer.Stop();
        GC.SuppressFinalize(this);
    }
}