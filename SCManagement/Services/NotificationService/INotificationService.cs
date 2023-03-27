namespace SCManagement.Services.NotificationService
{
    public interface INotificationService
    {
        public Task NotifyQuotaUpdate(int clubId);
    }
}
