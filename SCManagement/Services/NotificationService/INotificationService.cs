using SCManagement.Models;
using SCManagement.Services.ClubService.Models;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.PlansService.Models;

namespace SCManagement.Services.NotificationService
{
    public interface INotificationService
    {
        public void NotifyQuotaUpdate(int clubId, ClubPaymentSettings newValues); // 27
        public void NotifyPlansCreate(IEnumerable<Plan> plans); // 9 and 12
        public void NotifyPlanEdit(Plan plan); // 11 and 14
        public void NotifyPlanDeleted(Plan plan); // 10 and 13
        public void NotifyGoalCreate(Goal goal); // 15
        public void NotifyGoalEdited(Goal goal); // 17
        public void NotifyGoalDeleted(Goal goal); // 16
        public void NotifyGoalCompleted(Goal goal); // 18
        public void NotifyTeamAdded(Team team, ICollection<string> userIds); // 7
        public void NotifyTeam_Removed(Team team, string userIds); // 8
        public void NotifyEventCreate(Event eve); // 1
        public void NotifyEventEdit(Event eve); // 2
        public void NotifyEventDeleted(Event eve); // 3
        public void NotifyEventJoined(EventEnroll eventEnroll, bool needsPayment); // 4
        public void NotifyEventLeft(EventEnroll eventEnroll, bool missingPayment); // 5
        public void NotifyAthletesNumberAlmostFull(int clubId, ClubSlots slots); // 20
        public void NotifyPaymentLate(Payment payment); // 21
        public void NotifyPaymentReceived(int payId); // 22
        public void NotifySubscriptionStarted(int subId); // 23
        public void NotifySubscriptionRenewed(int subId); // 24
        public void NotifySubscriptionExpired(ICollection<int> subIds); // 25
        public void NotifySubscriptionCanceled(Subscription subscription); // 26
        public void NotifyPlanDiscontinued(int productId); // 19
        public void NotifySubscriptionRenewTime(ICollection<int> subIds); // 28
    }
}
