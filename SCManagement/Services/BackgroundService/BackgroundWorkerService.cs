using System.Collections.Concurrent;

namespace SCManagement.Services.BackgroundService
{
    public class BackgroundWorkerService : IHostedService
    {
        private readonly BlockingCollection<Func<IServiceProvider,Task>> _queue = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly ILogger<BackgroundWorkerService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() => ProcessQueue(_cancellationTokenSource.Token));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }

        public void Enqueue(Func<IServiceProvider,Task> job)
        {
            _queue.Add(job);
        }

        private async Task ProcessQueue(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    var job = _queue.Take(cancellationToken);
                    _logger.LogCritical($"{DateTime.Now} - Starting background job...");
                    await job(_serviceProvider);
                    _logger.LogCritical($"{DateTime.Now} - Background job completed.");
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    // Swallow the exception and continue processing the queue
                    _logger.LogWarning("Background job cancelled.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing background job.");
                }
            }
        }
    }

}
