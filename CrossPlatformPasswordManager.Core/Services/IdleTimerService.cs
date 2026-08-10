namespace CrossPlatformPasswordManager.Core.Services;

public partial class IdleTimerService(int timeoutMinutes = 5) : IDisposable
{
    private Timer? _timer;
    private DateTime _lastActivity = DateTime.Now;

    public event Action? OnIdle;
    public event Action? OnTick;

    public TimeSpan TimeRemaining =>
        TimeSpan.FromMinutes(timeoutMinutes) - (DateTime.Now - _lastActivity);

    public void SetLastActivity() =>
        _lastActivity = DateTime.Now;

    public void StartTimer()
    {
        StopTimer();
        _timer = new Timer(_ =>
        {
            var idleTime = DateTime.Now - _lastActivity;
            OnTick?.Invoke();
            if (idleTime.TotalMinutes >= timeoutMinutes)
            {
                OnIdle?.Invoke();
            }
        }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    public void StopTimer()
    {
        _timer?.Dispose();
        _timer = null;
    }

    public void Dispose()
    {
        _timer?.Dispose();
        GC.SuppressFinalize(this);
    }
}