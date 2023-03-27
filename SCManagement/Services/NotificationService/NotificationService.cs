namespace SCManagement.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly BackgroundWorkerService _backgroundWorkerService;

        public NotificationService(BackgroundWorkerService backgroundWorkerService)
        {
            _backgroundWorkerService = backgroundWorkerService;
        }

        public void NotifyQuotaUpdate(int clubId)
        {
            _backgroundWorkerService.Enqueue(async () =>
            {
                Console.WriteLine("AQUIIIIIIIIIIIIIIIIII");
                await Task.Delay(10000);
            });
        }
    }
}
