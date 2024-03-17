namespace CryptoLooser.Cli;

internal class PeriodicPerformanceCounter(
    TimeSpan period,
    Action<double> onPeriod)
{
    private readonly TimeSpan _period = period;
    private readonly Action<double> _onPeriod = onPeriod;
    private int _count;

    public void Increment()
    {
        Interlocked.Increment(ref _count);
    }

    public async Task Start(CancellationToken cancellationToken = default)
    {
        var periodicTimer = new PeriodicTimer(_period);

        try
        {
            while (
                !cancellationToken.IsCancellationRequested &&
                await periodicTimer.WaitForNextTickAsync(cancellationToken))
            {
                var count = Interlocked.Exchange(ref _count, 0);
                var countPerSecond = count / _period.TotalSeconds;

                _onPeriod(countPerSecond);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}
