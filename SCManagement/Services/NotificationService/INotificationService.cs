using SCManagement.Models;
using SCManagement.Services.PlansService.Models;

namespace SCManagement.Services.NotificationService
{
    public interface INotificationService
    {
        public void NotifyQuotaUpdate(int clubId, ClubPaymentSettings newValues);
        public void NotifyPlansCreate(IEnumerable<Plan> plans);
    }
}
