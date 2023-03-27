using System.Collections.Concurrent;

namespace SCManagement.Services.BackgroundWorkerService
{
    public class BackgroundWorkerService : IHostedService
    {
        private readonly BlockingCollection<Func<Task>> _queue = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();

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
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Func<Task> job = _queue.Take(cancellationToken);
                    await job();
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    // Swallow the exception and exit the loop
                }
            }
        }
    }

}
