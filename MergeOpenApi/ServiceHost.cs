using MergeOpenApi.Merge;

namespace MergeOpenApi;

public class ServiceHost : IHostedService
{
    private readonly IScheduler _scheduler;
    private readonly ILogger<ServiceHost> _logger;
    private TimeSpan _delay = TimeSpan.FromMinutes(5);
    private List<Task> _tasks = new List<Task>();
    private KestrelMetricServer _server;

    public ServiceHost(IScheduler scheduler, ILogger<ServiceHost> logger)
    {
        _scheduler = scheduler;
        _logger = logger;

        _server = new KestrelMetricServer(1402);
        _server.Start();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var scheduler = Task.Run(() => _scheduler.ExecuteWithDelay(cancellationToken, _delay)).ContinueWith(HandleTaskCancellation);
        _tasks.Add(scheduler);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Task.WaitAll(_tasks.ToArray(), TimeSpan.FromSeconds(15));
            
        return Task.CompletedTask;
    }

    private void HandleTaskCancellation(Task task)
    {
        if (!IsCancellationException(task.Exception))
        {
            _logger.Log(LogLevel.Error, "Service failed", task.Exception);

            throw task.Exception;
        }
    }

    private bool IsCancellationException(Exception exception)
    {
        if (exception is OperationCanceledException)
        {
            return true;
        }
            
        if (exception is AggregateException)
        {
            var aggregate = (AggregateException) exception;

            return aggregate.InnerExceptions.Any(IsCancellationException);
        }

        return false;
    }
}