namespace SCManagement.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly SCManagement.Services.BackgroundWorkerService.BackgroundWorkerService _backgroundWorkerService;

        public NotificationService(SCManagement.Services.BackgroundWorkerService.BackgroundWorkerService backgroundWorkerService)
        {
            _backgroundWorkerService = backgroundWorkerService;
        }

        public async Task NotifyQuotaUpdate(int clubId)
        {
            _backgroundWorkerService.Enqueue(async () =>
            {
            });
        }
    }
}
