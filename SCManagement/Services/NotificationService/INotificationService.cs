using SCManagement.Models;

namespace SCManagement.Services.NotificationService
{
    public interface INotificationService
    {
        public void NotifyQuotaUpdate(int clubId, ClubPaymentSettings newValues);
    }
}
