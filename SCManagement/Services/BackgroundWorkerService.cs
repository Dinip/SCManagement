using System.Collections.Concurrent;
using SCManagement.Services.CronJobService;

namespace SCManagement.Services
{
    public class BackgroundWorkerService : IHostedService
    {
        private readonly BlockingCollection<Func<Task>> _queue = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly ILogger<BackgroundWorkerService> _logger;
        
        public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger)
        {
            _logger = logger;
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

        public void Enqueue(Func<Task> job)
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
                    _logger.LogCritical("Starting background job...");
                    await job();
                    _logger.LogCritical("Background job completed.");
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    // Swallow the exception and continue processing the queue
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing background job.");
                }
            }
        }
    }

}
