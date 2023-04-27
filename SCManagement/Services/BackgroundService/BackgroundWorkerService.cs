using System.Collections.Concurrent;

namespace SCManagement.Services.BackgroundService
{
    public class BackgroundWorkerService : IHostedService
    {
        private readonly BlockingCollection<Func<IServiceProvider,Task>> _queue = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly ILogger<BackgroundWorkerService> _logger;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Background worker service constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }


        /// <summary>
        /// Starts a task to process the queue
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() => ProcessQueue(_cancellationTokenSource.Token));
            return Task.CompletedTask;
        }


        /// <summary>
        /// Stops the task to process the queue
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }


        /// <summary>
        /// Add the task to the processing queue
        /// </summary>
        /// <param name="job"></param>
        public void Enqueue(Func<IServiceProvider,Task> job)
        {
            _queue.Add(job);
        }


        /// <summary>
        /// Process the queue of tasks one by one
        /// Passes the service provider to the task so that it can resolve 
        /// the required dependencies inside the worker
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task ProcessQueue(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    var job = _queue.Take(cancellationToken);
                    _logger.LogInformation($"{DateTime.Now} - Starting background job...");
                    await job(_serviceProvider);
                    _logger.LogInformation($"{DateTime.Now} - Background job completed.");
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
