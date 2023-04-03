using SCManagement.Models;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.PlansService.Models;

namespace SCManagement.Services.NotificationService
{
    public interface INotificationService
    {
        public void NotifyQuotaUpdate(int clubId, ClubPaymentSettings newValues);
        public void NotifyPlansCreate(IEnumerable<Plan> plans);
        public void NotifyPlanEdit(Plan plan);
        public void NotifyPlanDeleted(Plan plan);
        public void NotifyGoalCreate(Goal goal);
        public void NotifyGoalEdited(Goal goal);
        public void NotifyGoalDeleted(Goal goal);
        public void NotifyGoalCompleted(Goal goal);
        public void NotifyTeamAdded(Team team, ICollection<string> userIds);
        public void NotifyTeam_Removed(Team team, string userIds);
        public void NotifyEventCreate(Event eve);
        public void NotifyEventEdit(Event eve);
        public void NotifyEventDeleted(Event eve);
        public void NotifyAthletesNumberAlmostFull(Product product);
        public void NotifyPaymentLate(Payment payment);
        public void NotifyPaymentReceived(Payment payment);
        public void NotifySubscriptionCanceled(Subscription subscription);
        public void NotifySubscriptionExpired(Subscription subscription);
        public void NotifySubscriptionExtended(Subscription subscription);
        public void NotifySubscriptionRenewed(Subscription subscription);
        public void NotifySubscriptionStarted(Subscription subscription);
        //public void NotifyPlanDescontinued();

    }
}
